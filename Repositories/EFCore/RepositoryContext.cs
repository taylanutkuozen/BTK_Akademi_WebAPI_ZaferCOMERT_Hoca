using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.EFCore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
namespace Repositories.EFCore
{
    public class RepositoryContext : IdentityDbContext<User>//IdentityUser
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);//Önemlidir.Migrationları uygun bir şekilde oluşması içindir.
            /*modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            Bunlar yerine aşağıdaki gibi kodu yazabiliriz.*/
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}