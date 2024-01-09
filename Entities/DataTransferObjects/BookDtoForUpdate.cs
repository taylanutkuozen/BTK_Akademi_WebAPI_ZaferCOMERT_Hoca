using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities.DataTransferObjects
{
    public record BookDtoForUpdate(int BookID,string Title, decimal Price);
    /*{
        /*public int BookID { get; init; }
        public string TitleDto { get; init; }
        public decimal PriceDto { get; init; }
        Tanımlandığı yerde değeri verilecek ve sonrasında değişmeyecek --> init
    }*/
}
/*Data Transfer Objects
 readonly olmalıdır.
 immutable olmalıdır.
 LINQ
 Ref Type
 Ctor(DTO)*/