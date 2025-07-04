
using Bookstore.DataAccess.Models;

namespace Bookstore.DataAccess.ViewModels
{
    public class OrderViewModel
    {
        public Order Order {  get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
