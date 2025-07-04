using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.DataAccess.Models
{
    public class ShoppingCart : IEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }


        [ValidateNever]
        public Product Product { get; set; } = null!;

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        public string ApplicationUserId { get; set; } = null!;


        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [NotMapped]
        public decimal Price { get; set; }
    }
}
