using BackendServices.Configurations;
using BackendServices.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BackendServices;

public class VendorRepository : IVendorRepository
{
    private readonly IMongoCollection<Vendor> _vendors;

    public VendorRepository(IOptions<MongoDBSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _vendors = database.GetCollection<Vendor>("Vendors");
    }

    public async Task<Vendor> GetVendorByEmailAsync(string email)
    {
        return await _vendors.Find(v => v.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateVendorAsync(Vendor vendor)
    {
        await _vendors.InsertOneAsync(vendor);
    }

    public async Task UpdateVendorAsync(Vendor vendor)
    {
        await _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
    }

    public async Task DeleteVendorAsync(string email)
    {
        await _vendors.DeleteOneAsync(v => v.Email == email);
    }

    public async Task<List<Vendor>> GetVendorsAsync()
    {
        return await _vendors.Find(_ => true).ToListAsync();
    }

    public async Task<Vendor> GetVendorByIdAsync(string vendorId)
    {
        return await _vendors.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
    }
    
    public async Task AddCommentAsync(string vendorId, VendorComment comment)
    {
        // Find the vendor and add the comment to its comment list
        var filter = Builders<Vendor>.Filter.Eq(v => v.Id, vendorId);
        var update = Builders<Vendor>.Update.Push(v => v.Comments, comment);  // Add the comment to the list
        await _vendors.UpdateOneAsync(filter, update);
    }
    
    
    public async Task DeleteCommentAsync(string vendorId, string commentId)
    {
        var vendor = await _vendors.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
        if (vendor == null)
        {
            throw new ArgumentException("Vendor not found.");
        }

        var comment = vendor.Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
        {
            throw new ArgumentException("Comment not found.");
        }

        vendor.Comments.Remove(comment);
        await _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
    }
    
    


}