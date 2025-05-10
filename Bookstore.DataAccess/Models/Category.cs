using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.DataAccess.Models
{
    public class Category
    {
        public int Id { get; set; }


        [MaxLength(30)]
        public string Name { get; set; } = null!;


        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage ="Range Must Be Between 1 and 100")]
        public int DisplayOrder { get; set; }
    }
}
