using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WormReads.Models.ViewModels
{
    public class ProductVM
    {
        public Product product { get; set; }
        [ValidateNever]
        public SelectList Categories { get; set; }
    }
}
