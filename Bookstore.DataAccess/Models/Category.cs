using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.DataAccess.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
