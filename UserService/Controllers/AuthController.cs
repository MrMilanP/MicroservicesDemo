using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserMicroservice.Data;

using UserMicroservice.Models;

using MicroservicesShared.Configuration;
using UserMicroservice.Repositories;
using UserMicroservice.Services;


namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly UserDbContext _context;
        //private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IUserService userService, SigningCredentials signingCredentials, JwtSettings jwtSettings)
        {
            //_context = context;
            //_configuration = configuration;
            _userService = userService;
            _signingCredentials = signingCredentials;
            _jwtSettings = jwtSettings;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)

        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null || user.Password != password)
                    return Unauthorized(new { message = "Invalid credentials" });

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim("UserId", user.Id.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: _signingCredentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                Console.WriteLine($"Generated Token: {tokenString}");

                return Ok(new { Token = tokenString });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }
    }
}