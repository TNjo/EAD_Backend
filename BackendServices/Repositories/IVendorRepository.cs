using BackendServices.Models;

namespace BackendServices;

public interface IVendorRepository
{
    Task<Vendor> GetVendorByEmailAsync(string email);
    Task CreateVendorAsync(Vendor vendor);
    Task UpdateVendorAsync(Vendor vendor);
    Task DeleteVendorAsync(string email);
    Task<List<Vendor>> GetVendorsAsync();
    Task<Vendor> GetVendorByIdAsync(string vendorId);
    Task AddCommentAsync(string vendorId, VendorComment comment);
    
    Task DeleteCommentAsync(string vendorId, string commentId);
    
}