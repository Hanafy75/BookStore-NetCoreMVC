using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Bookstore.Common.Validation;

namespace Bookstore.DataAccess.Models
{
    public class Category : IEntity
    {
        public int Id { get; set; }


        [MaxLength(30)]
        [RegularExpression(@"^(?!\d*$).+", ErrorMessage = "Category Name Must Contain Letters")]
        public string Name { get; set; } = null!;


        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage ="Range Must Be Between 1 and 100")]
        [NotEqual("Name")]
        public int DisplayOrder { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
