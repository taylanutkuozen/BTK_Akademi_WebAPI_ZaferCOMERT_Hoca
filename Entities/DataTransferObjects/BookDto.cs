using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities.DataTransferObjects
{
    /*[Serializable] açık olması için {get;set;} olarak tanımladık property'leri*/
    public record BookDto
    {
        public int BookID { get; init; }
        public string Title { get; init; }
        public decimal Price { get; init; }
    }
}