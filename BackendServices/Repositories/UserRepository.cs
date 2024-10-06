using BackendServices.Configurations;
using BackendServices.Models;
using Microsoft.Extensions.Options;

namespace BackendServices;

// Repositories/UserRepository.cs
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IOptions<MongoDBSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _users = database.GetCollection<User>("Users");
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return await _users.Find(u => u.Email == email).AnyAsync() == false;
    }

    public async Task DeleteUserAsync(string userId)
    {
        await _users.DeleteOneAsync(u => u.Id == userId);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _users.Find(user => true).ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
    }
}
