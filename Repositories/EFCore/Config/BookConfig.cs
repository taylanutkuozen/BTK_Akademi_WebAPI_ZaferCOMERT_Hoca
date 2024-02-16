using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories.EFCore.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasData(
                new Book { BookID = 1,  Title = "Karagöz ve Hacivat", Price = 75 },
                new Book { BookID = 2, Title = "Mesnevi", Price = 150 },
                new Book { BookID = 3,  Title = "Incognito", Price = 125 }
                );
        }
    }
}
