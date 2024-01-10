using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities.DataTransferObjects
{
    /*[Serializable] açık yani {get;set;} olarak tanımladık property'leri BookDtoForUpdate aksine*/
    public record BookDto
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}