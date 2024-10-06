using MongoDB.Driver;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BackendServices.Configurations;
using BackendServices.Models;
using BCrypt.Net;
using Microsoft.Extensions.Options;

namespace BackendServices.Services
{
    // public class UserService
    // {
    //     private readonly IMongoCollection<User> _users;
    //
    //     public UserService(IOptions<MongoDBSettings> settings)
    //     {
    //         var client = new MongoClient(settings.Value.ConnectionString);
    //         var database = client.GetDatabase(settings.Value.DatabaseName);
    //         _users = database.GetCollection<User>("Users");
    //     }
    //
    //     public async Task<User> GetUserByEmailAsync(string email)
    //     {
    //         return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
    //     }
    //
    //     public async Task CreateUserAsync(User user)
    //     {
    //         await _users.InsertOneAsync(user);
    //     }
    //
    //     public async Task UpdateUserAsync(User user)
    //     {
    //         await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    //     }
    //
    //     public async Task<bool> IsEmailUniqueAsync(string email)
    //     {
    //         return await _users.Find(u => u.Email == email).AnyAsync() == false;
    //     }
    //     // Deletion Of User
    //     public async Task DeleteUserAsync(string userId)
    //     {
    //         await _users.DeleteOneAsync(u => u.Id == userId);
    //     }
    //
    //     public static string EncryptPassword(string password)
    //     {
    //         return BCrypt.Net.BCrypt.HashPassword(password);
    //     }
    //
    //     public static bool VerifyPassword(string password, string hashedPassword)
    //     {
    //         return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    //     }
    //
    //     public async Task<List<User>> GetAllUsersAsync()
    //     {
    //         // Fetch all users from the Users collection
    //         return await _users.Find(user => true).ToListAsync();
    //     }
    //
    //     public async Task<User> GetUserByIdAsync(string id)
    //     {
    //         // Fetch a single user by their MongoDB ObjectId
    //         return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
    //     }
    //
    //
    // }
    
    
    // Services/UserService.cs
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task CreateUserAsync(User user)
        {
            await _userRepository.CreateUserAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return await _userRepository.IsEmailUniqueAsync(email);
        }

        public async Task DeleteUserAsync(string userId)
        {
            await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public static string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        
        // Notify vendor about an order
        public async Task NotifyVendorAsync(string vendorEmail, string message)
        {
            // Implement notification logic (email, SMS, or push notification)
        }

        // Notify customer about the order status change
        public async Task NotifyCustomerAsync(string customerId, string message)
        {
            // Implement notification logic
        }
    }

    
}

