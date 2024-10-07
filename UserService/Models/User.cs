using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace UserMicroservice.Models
{
    public class User
    {
        [SwaggerSchema(ReadOnly = true, Description = "ID korisnika")]
        public int Id { get; set; }

        [SwaggerSchema(Description = "Ime korisnika")]
        public string? Name { get; set; }

        [SwaggerSchema(Description = "Email korisnika")]
        public string? Email { get; set; }

        [SwaggerSchema(Description = "Lozinka korisnika")]
        public string? Password { get; set; }
    }
}
