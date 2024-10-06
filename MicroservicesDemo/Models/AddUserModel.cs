using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using UserMicroservice.Models;

namespace MicroservicesDemo.Models
{
    public class AddUserModel : PageModel
    {
        [BindProperty]
        public User user { get; set; } = new User();

        public void OnGet()
        {
        }
    }
}
