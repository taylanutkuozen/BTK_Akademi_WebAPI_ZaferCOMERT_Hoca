using Microsoft.EntityFrameworkCore;
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
    }
}