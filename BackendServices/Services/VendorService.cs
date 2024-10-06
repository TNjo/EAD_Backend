using MongoDB.Driver;
using System.Threading.Tasks;
using BackendServices.Configurations;
using BackendServices.Models;
using BackendServices.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace BackendServices.Services
{
    // public class VendorService
    // {
    //     private readonly IMongoCollection<Vendor> _vendors;
    //     private readonly UserService _userService; // Used to sync between Vendor and User collections
    //
    //     public VendorService(IOptions<MongoDBSettings> mongoSettings, UserService userService)
    //     {
    //         var client = new MongoClient(mongoSettings.Value.ConnectionString);
    //         var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
    //         _vendors = database.GetCollection<Vendor>("Vendors");
    //         _userService = userService;
    //     }
    //
    //     public async Task<Vendor> GetVendorByEmailAsync(string email)
    //     {
    //         return await _vendors.Find(v => v.Email == email).FirstOrDefaultAsync();
    //     }
    //
    //     public async Task CreateVendorAsync(Vendor vendor)
    //     {
    //         await _vendors.InsertOneAsync(vendor);
    //         // Create an entry in the User collection for login purposes
    //         var newUser = new User
    //         {
    //             Email = vendor.Email,
    //             Password = vendor.Password,  // Should be encrypted
    //             Role = "Vendor",
    //             Status = 1
    //         };
    //         await _userService.CreateUserAsync(newUser);
    //     }
    //
    //     public async Task UpdateVendorAsync(Vendor vendor)
    //     {
    //         await _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
    //         // Sync the changes to the User collection (for email/password updates)
    //         var user = await _userService.GetUserByEmailAsync(vendor.Email);
    //         if (user != null)
    //         {
    //             user.Email = vendor.Email;
    //             user.Password = vendor.Password;
    //             await _userService.UpdateUserAsync(user);
    //         }
    //     }
    //
    //     public async Task DeleteVendorAsync(string email)
    //     {
    //         await _vendors.DeleteOneAsync(v => v.Email == email);
    //         // Delete from User collection as well
    //         var user = await _userService.GetUserByEmailAsync(email);
    //         if (user != null)
    //         {
    //             await _userService.DeleteUserAsync(user.Id);
    //         }
    //     }
    //
    //     
    //
    //     public async Task<List<Vendor>> GetVendorsAsync()
    //     {
    //         return await _vendors.Find(_ => true).ToListAsync();  // List all vendors
    //     }
    //
    //     // public async Task AddCommentAsync(string vendorId, string comment, int rank, string userId)
    //     // {
    //     //     var vendor = await _vendors.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
    //     //     if (vendor != null)
    //     //     {
    //     //         var newComment = new CommentEntry
    //     //         {
    //     //             Id = ObjectId.GenerateNewId().ToString(),  // Generate a unique ID for this comment
    //     //             UserId = userId,
    //     //             Comment = comment,
    //     //             Rank = rank
    //     //         };
    //     //
    //     //         vendor.Comments.Add(newComment);
    //     //         await _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
    //     //     }
    //     // }
    //     //
    //     // public async Task UpdateCommentAsync(string vendorId, string commentId, string newComment, int newRank, string userId)
    //     // {
    //     //     var vendor = await _vendors.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
    //     //     if (vendor != null)
    //     //     {
    //     //         var comment = vendor.Comments.FirstOrDefault(c => c.Id == commentId && c.UserId == userId);
    //     //         if (comment != null)
    //     //         {
    //     //             comment.Comment = newComment;
    //     //             comment.Rank = newRank;
    //     //             await _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
    //     //         }
    //     //     }
    //     // }
    //     //
    //     // public async Task DeleteCommentAsync(string vendorId, string commentId, string userId)
    //     // {
    //     //     var vendor = await _vendors.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
    //     //     if (vendor != null)
    //     //     {
    //     //         vendor.Comments.RemoveAll(c => c.Id == commentId && c.UserId == userId);
    //     //         await _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
    //     //     }
    //     // }
    //     // Get Vender New
    //     public async Task<Vendor> GetVendorByIdAsync(string vendorId)
    //     {
    //         return await _vendors.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
    //     }
    //
    //
    // }
    
    
    public class VendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly UserService _userService; // Used to sync between Vendor and User collections

    public VendorService(IVendorRepository vendorRepository, UserService userService)
    {
        _vendorRepository = vendorRepository;
        _userService = userService;
    }

    public async Task<Vendor> GetVendorByEmailAsync(string email)
    {
        return await _vendorRepository.GetVendorByEmailAsync(email);
    }

    public async Task CreateVendorAsync(Vendor vendor)
    {
        await _vendorRepository.CreateVendorAsync(vendor);
        // Create an entry in the User collection for login purposes
        var newUser = new User
        {
            Email = vendor.Email,
            Password = vendor.Password,  // Should be encrypted
            Role = "Vendor",
            Status = 1
        };
        await _userService.CreateUserAsync(newUser);
    }

    public async Task UpdateVendorAsync(Vendor vendor)
    {
        await _vendorRepository.UpdateVendorAsync(vendor);
        // Sync the changes to the User collection (for email/password updates)
        var user = await _userService.GetUserByEmailAsync(vendor.Email);
        if (user != null)
        {
            user.Email = vendor.Email;
            user.Password = vendor.Password;
            await _userService.UpdateUserAsync(user);
        }
    }

    public async Task DeleteVendorAsync(string email)
    {
        await _vendorRepository.DeleteVendorAsync(email);
        // Delete from User collection as well
        var user = await _userService.GetUserByEmailAsync(email);
        if (user != null)
        {
            await _userService.DeleteUserAsync(user.Id);
        }
    }

    public async Task<List<Vendor>> GetVendorsAsync()
    {
        return await _vendorRepository.GetVendorsAsync();
    }

    public async Task<Vendor> GetVendorByIdAsync(string vendorId)
    {
        return await _vendorRepository.GetVendorByIdAsync(vendorId);
    }
    

    
    public async Task AddCommentAsync(string vendorId, string comment, int rank, string userId)
    {
        // Validate rank (for example, between 1 and 5)
        if (rank < 1 || rank > 5)
        {
            throw new ArgumentException("Rank must be between 1 and 5.");
        }

        // Create a new comment object
        var newComment = new VendorComment
        {
            Id = ObjectId.GenerateNewId().ToString(), 
            Comment = comment,
            Rank = rank,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };

        // Add the comment using the repository
        await _vendorRepository.AddCommentAsync(vendorId, newComment);
    }
    
    
    public async Task<bool> UpdateCommentAsync(string vendorId, string commentId, string comment, int rank, string userId)
    {
        // Validate rank (for example, between 1 and 5)
        if (rank < 1 || rank > 5)
        {
            throw new ArgumentException("Rank must be between 1 and 5.");
        }

        // Retrieve the vendor from the repository
        var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
        if (vendor == null)
        {
            return false;  // Vendor not found
        }

        // Find the comment to update
        var existingComment = vendor.Comments.FirstOrDefault(c => c.Id == commentId && c.UserId == userId);
        if (existingComment == null)
        {
            return false;  // Comment not found or user not authorized
        }

        // Update the comment
        existingComment.Comment = comment;
        existingComment.Rank = rank;
        existingComment.Timestamp = DateTime.UtcNow;  // Update the timestamp

        // Update the vendor in the repository
        await _vendorRepository.UpdateVendorAsync(vendor);

        return true;
    }
    
    
    public async Task DeleteCommentAsync(string vendorId, string commentId)
    {
        await _vendorRepository.DeleteCommentAsync(vendorId, commentId);
    }


    
}
    
    
}
