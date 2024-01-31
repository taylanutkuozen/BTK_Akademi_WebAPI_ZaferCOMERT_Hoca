using Entities.DataTransferObjects;
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
    }
}