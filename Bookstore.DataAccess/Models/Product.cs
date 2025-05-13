using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
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


        //Foreing key
        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category? Category { get; set; }


        [ValidateNever]
        public string ImageUrl { get; set; } = null!;



        /*
         -- Removed to the Product service layer to make the model clean of logic
        public bool IsEquivalentTo(Product other)
        {
            if (other == null) return false;

            return string.Equals(Title, other.Title, StringComparison.OrdinalIgnoreCase)
                && Author == other.Author
                && Description == other.Description
                && ISBN == other.ISBN
                && ListPrice == other.ListPrice
                && Price == other.Price
                && Price50 == other.Price50
                && Price100 == other.Price100;
        }
        */

    }
}
