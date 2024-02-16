using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        //Bir referans tanımı : Collection navigation property-Bire çok ilişki kurulumu(Foreign Key)
        //public ICollection<Book> Books { get; set;}
    }
}