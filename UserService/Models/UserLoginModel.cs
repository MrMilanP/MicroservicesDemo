using System.ComponentModel.DataAnnotations;

namespace UserMicroservice.Models
{
    public class UserLoginModel
    {
  
        public string? Email { get; set; }
      
        public string? Password { get; set; }
    }
}
