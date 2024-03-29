﻿using AspNetCoreRateLimit;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilters;
using Presentation.Controllers;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
namespace WebAPI.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services,IConfiguration configuration)=> 
            services.AddDbContext<RepositoryContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
            /*hangi type'ı genişletmek istiyor isek this ile genişletmek gerekiyor ancak parametre kullanılmayacaktır.*/
            //Generic olarak DbContext inherit olan context class eklememiz gerekiyor. IoC DbContext tanımı yapmış olundu=Bir DbContext ihtiyacımız olduğunda bir injection yaptığımız zaman somut haline ulaşmak ve db ye bağlanırken hata olmayacaktır. *Register-Container*
        public static void ConfigureRepositoryManager(this IServiceCollection services)=>
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        /*AddScoped her kullanıcıya özel olarak bu nesne üretilsin.*/
        public static void ConfigureServicesManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager,ServiceManager>();
        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerManager>();
        /*Logger Servis için bir extension metot. AddSingleton yani statik bir üye varmış gibi düşüneceğiz. logger bir kez üretilecek herkes aynı nesneyi kullanacak. IoC de hatırlanırsa bir nesnenin yaşam döngüsüne karar verebiliriz.*/
        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
            //IoC tarafından projede sağlanacaktır.
            services.AddSingleton<LogFilterAttribute>();
            services.AddScoped<ValidateMediaTypeAttribute>();
        }
        public static void ConfigureCors(this IServiceCollection service)
        {
            service.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", 
                    builder=>builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")
                );
            });
        }
        /*(Cors)Cross origin resource share=Kökenler arası kaynak paylaşımı*/
        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        }
        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemJsonOutputFormatter = config.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();
                if (systemJsonOutputFormatter is not null)
                {
                    systemJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.hateoas+json");
                    systemJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+json");
                }
                var xmlOutputFormatters = config.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();
                if(xmlOutputFormatters is not null)
                 {
                    xmlOutputFormatters.SupportedMediaTypes
                   .Add("application/vnd.btkakademi.hateoas+xml");
                    xmlOutputFormatters.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+xml");
                 }
            });
        }
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt=>
            {
                opt.ReportApiVersions = true;
                /*Api versiyon bilgisi response header kısmına ekledik*/
                opt.AssumeDefaultVersionWhenUnspecified = true;
                /*Eğer kullanıcı herhangi bir versiyon talep etmez Api default version dönecek*/
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                /*Default ne olduğunu söylediğimiz parametre*/
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                opt.Conventions.Controller<BooksController>()
                .HasApiVersion(new ApiVersion(1,0));
                opt.Conventions.Controller<BooksV2Controller>()
                .HasDeprecatedApiVersion(new ApiVersion(2,0));
                /*Attribute yerine configure aşamasında version bilgileri ekledik.*/
            });
        }
        public static void ConfigureResponseCaching(this IServiceCollection services) => services.AddResponseCaching();/*IoC kaydı*/
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) => services.AddHttpCacheHeaders
            (
            expirationOpt =>
            {
                expirationOpt.MaxAge = 90;
                expirationOpt.CacheLocation = Marvin.Cache.Headers.CacheLocation.Public;
                /*Headers içerisindeki bilgileri değiştirmiş oluyoruz. Api yeniden kaynak oluşturduğunda Cache'ten cevap vermemeye başladı.*/
            },
            validationOpt=>
            {
                validationOpt.MustRevalidate = false;/*Yeniden validate olma zorunluluğu olmasın. true yaparsak Headers içerisinde Cache-Control'de must-revalidate görülecektir.*/
            }
            );
        /*ValidationModel için kullanıyoruz.*/
        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>()
            {
                new RateLimitRule()
                {
                    Endpoint="*",
                    Limit=60,
                    Period="1m"
                }
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(opt=>
            {
                    opt.Password.RequireDigit = true;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequiredLength = 6;

                    opt.User.RequireUniqueEmail = true;
            })
               .AddEntityFrameworkStores<RepositoryContext>()
               .AddDefaultTokenProviders();
            /*User=IdentityUser*/
        }
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];
            services.AddAuthentication(opt=>
            {
                opt.DefaultAuthenticateScheme =JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;/*Authentication için default şemalar*/
            }).AddJwtBearer(options=>
                        options.TokenValidationParameters=new TokenValidationParameters
                            {
                                ValidateIssuer=true,/*Key'i kim üretti ise doğrula*/
                                ValidateAudience=true,/*Audience=Geçerli bir alıcı mı değil mi doğrula*/
                                ValidateLifetime=true,
                                ValidateIssuerSigningKey=true,/*Anahtar için doğrulama*/
                                ValidIssuer = jwtSettings["validIssuer"],
                                ValidAudience = jwtSettings["validAudience"],
                                IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                                /*İlgili Middleware kaydolma işlemi yapıyoruz.(IoC kaydı yaptık.) 
                                 * Kullanılacak şemayı belirtiyoruz yani Token kullanacağız.
                                 Son olarak Token'ı doğrulayacak parametreleri hazırladık.*/
                            }
                        );
        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "BTK Akademi", 
                    Version = "v1" ,
                    Description="BTK Akademi ASP.NET Core Web API",
                    TermsOfService=new Uri("https://www.btkakademi.gov.tr/"),
                    Contact = new OpenApiContact
                    {
                        Name="Taylan Utku OZEN",
                        Email="taylanutku.ozen@gmail.com"
                        //Url=new Uri("....")
                    }
                });
                s.SwaggerDoc("v2", new OpenApiInfo { Title = "BTK Akademi", Version = "v2" });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer"
                        },
                        new List<string>()
                    }
                });
            });
        }
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IAuthenticationService, AuthenticationManager>();
        }
    }
}