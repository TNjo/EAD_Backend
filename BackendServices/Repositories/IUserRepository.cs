using BackendServices.Models;

namespace BackendServices;

// Repositories/IUserRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository
{
    Task<User> GetUserByEmailAsync(string email);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<bool> IsEmailUniqueAsync(string email);
    Task DeleteUserAsync(string userId);
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(string id);
}
