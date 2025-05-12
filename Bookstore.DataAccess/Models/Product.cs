using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bookstore.DataAccess.Models
{
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string ISBN { get; set; } = null!;

        [Display(Name = "List Price")]
        [Range(1,1000)]
        public decimal ListPrice { get; set; }


        [Display(Name = "Price 1-50")]
        [Range(1, 1000)]
        public decimal Price { get; set; }


        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public decimal Price50 { get; set; }


        [Display(Name = "Price for 100")]
        [Range(1, 1000)]
        public decimal Price100 { get; set; }

        /*
        //Foreing key
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category? Category { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; } = null!;
        */
    }
}
