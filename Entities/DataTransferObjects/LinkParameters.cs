using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
namespace Entities.DataTransferObjects
{
    public record LinkParameters /*record kullanma sebebi=Presentation Layer(Controller) Services katmanına veriyi taşırken verinin değişmezliğine garanti etmek için bir Dto kullanılması gereklidir.*/
    {
        public BookParameters BookParameters { get; init; } 
        /*init=>immutable(değişmezlik) için kullanıldı.*/
        public HttpContext HttpContext { get; init; }
    }
}