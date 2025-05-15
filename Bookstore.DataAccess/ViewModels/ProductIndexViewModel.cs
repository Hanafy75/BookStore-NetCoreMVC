using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.ViewModels
{
    public class ProductIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public decimal ListPrice { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
