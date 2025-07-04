using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookstore.DataAccess.ViewModels
{
    public class UserViewModel
    {
       public ApplicationUser User { get; set; }
       public SelectList Companies { get; set; }
       public SelectList Roles { get; set; }
    }
}
