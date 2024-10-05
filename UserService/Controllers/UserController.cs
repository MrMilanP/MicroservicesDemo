using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Data;
using UserMicroservice.Models;
using UserMicroservice.Services;

namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Zaštiti sve rute u kontroleru
    public class UserController : ControllerBase
    {
        // Privatno polje koje sadrzi UserService instancu ubrizganu preko Dependency Injection-a.
        // Koristi se za pristup poslovnoj logici, sto je bolje nego direktno raditi sa bazom jer omogucava bolju podelu odgovornosti.
        //private readonly UserDbContext _context;
        private readonly IUserService _userService;

        //public UserController(UserDbContext context)
        //{
        //    _context = context;
        //}
        public UserController(IUserService userService, UserDbContext context)
        {
            _userService = userService;
        }

        [HttpGet]
        //public ActionResult<IEnumerable<User>> GetUsers()
        //{
        //    return _context.Users.ToList();
        //}
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userService.GetAllUsersAsync().Result;
            return Ok(users);
        }

        [HttpPost]
        //public ActionResult<User> CreateUser(User user)
        //{
        //    _context.Users.Add(user);
        //    _context.SaveChanges();
        //    return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        //}
        public async Task<IActionResult> AddUser(User user)
        {
            var result = await _userService.AddUserAsync(user);
            if (result)
                return Ok();

            return BadRequest("Error adding user");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var result = await _userService.UpdateUserAsync(user);
            if (result)
                return Ok();

            return BadRequest("Error updating user");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
                return Ok();

            return NotFound();
        }
    }
}