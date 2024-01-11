using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities.DataTransferObjects
{
    public abstract record BookDtoForManipulation
    {
        [Required(ErrorMessage ="Title is required field")]
        [MinLength(2,ErrorMessage ="Title must contain at least 2 characters")]
        [MaxLength(100,ErrorMessage ="Title must consist of at maximum 100 characters")]
        public string Title { get; init; }
        [Required(ErrorMessage ="Price is required field")]
        [Range(10,350)]
        public decimal Price { get; init; }
    }
}