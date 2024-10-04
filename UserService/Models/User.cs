using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }  // Primarni ključ
        public string Name { get; set; }
        public string Email { get; set; }
        // Dodaj ostala svojstva po potrebi
    }
}
