using Bookstore.Common.Validation;
using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace Bookstore.DataAccess.ViewModels
{
    public class ProductEditViewModel
    {
        public Product product { get; set; } = null!;
        public SelectList? CategoryList { get; set; }


        [AllowedExtensions(new[] { ".jpg",".png" , ".webp" }, ErrorMessage = "Please upload only JPG, PNG, or WebP files.")]
        public IFormFile? ImageFile { get; set; } = null!;
    }
}
