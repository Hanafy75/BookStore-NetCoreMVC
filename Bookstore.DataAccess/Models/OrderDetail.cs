using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace Bookstore.DataAccess.Models
{
    public class OrderDetail : IEntity
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }


        [ValidateNever]
        public Order? Order { get; set; }


        [Required]
        public int ProductId { get; set; }

        [ValidateNever]
        public Product? Product { get; set; }

        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}