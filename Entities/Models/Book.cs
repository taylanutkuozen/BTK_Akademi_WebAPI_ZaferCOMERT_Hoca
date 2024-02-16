using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        //Referans type : Navigation Property
        public Category Category { get; set; }
    }
}