using UserMicroservice.Models;
using UserMicroservice.Repositories;

namespace UserMicroservice.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<bool> AddUserAsync(User user)
        {
            // Ovde možeš dodati dodatnu logiku, kao što je validacija.
            await _userRepository.AddUserAsync(user);
            return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
            return true;
        }
    }
}