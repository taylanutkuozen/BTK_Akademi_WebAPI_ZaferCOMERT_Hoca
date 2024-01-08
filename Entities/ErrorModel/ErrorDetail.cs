using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Entities.ErrorModel
{
    public class ErrorDetail
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        /*? anlamı=Null check. yani bu değer null olabilir.*/
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
/*Nesne Serialize edildiğinde
{
    message:"...",
    statusCode:200
} bu şekilde olur.
 */