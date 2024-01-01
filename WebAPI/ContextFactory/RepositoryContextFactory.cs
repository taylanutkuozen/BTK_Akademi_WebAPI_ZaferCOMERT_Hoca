using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repositories.EFCore;

namespace WebAPI.ContextFactory
{
    public class RepositoryContextFactory
        : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            //configurationBuilder
            var configuration=new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            //DbContextOptionsBuilder
            var builder = new DbContextOptionsBuilder<RepositoryContext>()
                .UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                prj=>prj.MigrationsAssembly("WebAPI"));/*Migrationslar WebAPI içerisinde oluşsun*/
            return new RepositoryContext(builder.Options);
        }/*appsettings.json içerisinden DB connection için gereklidir.
          DB Connection connection string ifadesine ulaşmak için gereklidir.*/
    }
}