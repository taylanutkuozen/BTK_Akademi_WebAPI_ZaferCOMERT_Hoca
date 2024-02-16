using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities.DataTransferObjects
{
    public record BookDtoForInsertion : BookDtoForManipulation
    {
        [Required(ErrorMessage ="CategoryID is required")]
        public int CategoryID { get; init; }
    }
}