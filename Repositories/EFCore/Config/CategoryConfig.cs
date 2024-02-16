using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories.EFCore.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryID); // PrimaryKey
            builder.Property(c=>c.CategoryName).IsRequired();
            builder.HasData(new Category()
            {
                CategoryID = 1,
                CategoryName = "Computer Science"
            },
            new Category()
            {
                CategoryID=2,
                CategoryName="Network"
            },
            new Category()
            {
                CategoryID=3,
                CategoryName="Database Management System"
            }
            );
        }
    }
}