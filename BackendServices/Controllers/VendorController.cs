using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BackendServices.DTOs;
using BackendServices.Helpers;
using BackendServices.Models;
using BackendServices.Services;


namespace BackendServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly VendorService _vendorService;
        private readonly UserService _userService;
        private readonly JwtHelper _jwtHelper;

        public VendorController(VendorService vendorService, JwtHelper jwtHelper, UserService userService)
        {
            _vendorService = vendorService;
            _jwtHelper = jwtHelper;
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]  // Only Admins can create vendors
        [HttpPost("create")]
        public async Task<IActionResult> CreateVendor([FromBody] VendorDTO vendorModel)
        {
            // Check if the email exists in both Vendor and User collections
            if (await _vendorService.GetVendorByEmailAsync(vendorModel.Email) != null || await _userService.GetUserByEmailAsync(vendorModel.Email) != null)
            {
                return BadRequest("Vendor with this email already exists.");
            }

            var newVendor = new Vendor
            {
                VendorName = vendorModel.VendorName,
                Email = vendorModel.Email,
                Password = UserService.EncryptPassword(vendorModel.Password),  // Encrypt password
                Category = vendorModel.Category
            };

            await _vendorService.CreateVendorAsync(newVendor);
            return Ok("Vendor created successfully.");
        }





        [Authorize(Roles = "Admin")]
        [HttpPut("update/{vendorId}")]
        public async Task<IActionResult> UpdateVendor(string vendorId, [FromBody] VendorDTO updatedVendor)
        {
            var existingVendor = await _vendorService.GetVendorByEmailAsync(updatedVendor.Email);
            if (existingVendor == null)
            {
                return NotFound("Vendor not found.");
            }

            // Update fields
            existingVendor.VendorName = updatedVendor.VendorName;
            existingVendor.Email = updatedVendor.Email;
            existingVendor.Password = UserService.EncryptPassword(updatedVendor.Password);
            existingVendor.Category = updatedVendor.Category;

            // Sync the changes to the User collection
            await _vendorService.UpdateVendorAsync(existingVendor);
            return Ok("Vendor updated successfully.");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{email}")]
        public async Task<IActionResult> DeleteVendor(string email)
        {
            await _vendorService.DeleteVendorAsync(email);
            return Ok("Vendor deleted successfully.");
        }



        [Authorize(Roles = "Admin, Vendor")]
        [HttpPost("deactivate/{email}")]
        public async Task<IActionResult> DeactivateVendor(string email)
        {
            var vendor = await _vendorService.GetVendorByEmailAsync(email);
            if (vendor == null)
            {
                return NotFound("Vendor not found.");
            }

            vendor.Status = 0;  // Deactivate
            await _vendorService.UpdateVendorAsync(vendor);
            return Ok("Vendor deactivated.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("activate/{email}")]
        public async Task<IActionResult> ActivateVendor(string email)
        {
            var vendor = await _vendorService.GetVendorByEmailAsync(email);
            if (vendor == null)
            {
                return NotFound("Vendor not found.");
            }

            vendor.Status = 1;  // Activate
            await _vendorService.UpdateVendorAsync(vendor);
            return Ok("Vendor activated.");
        }

       
        // List all vendors with comments and ranks
        [HttpGet("list")]
        public async Task<IActionResult> ListVendors()
        {
            var vendors = await _vendorService.GetVendorsAsync();
            return Ok(vendors);
        }
        //Get a Single Vendor
       
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetVendorById(string vendorId)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
            if (vendor == null)
            {
                return NotFound("Vendor not found.");
            }

            return Ok(vendor);
        }
        
        
        [Authorize]
        [HttpPost("comment/{vendorId}")]
        public async Task<IActionResult> AddComment(string vendorId, [FromBody] CommentDTO model)
        {
            var userId = User.FindFirst("UserId")?.Value;  // Get User ID from the JWT token
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Call the VendorService to add the comment
            await _vendorService.AddCommentAsync(vendorId, model.Comment, model.Rank, userId);
            return Ok("Comment added successfully.");
        }
        
        
        [Authorize]
        [HttpPut("comment/{vendorId}/{commentId}")]
        public async Task<IActionResult> UpdateComment(string vendorId, string commentId, [FromBody] CommentDTO model)
        {
            var userId = User.FindFirst("UserId")?.Value;  // Get User ID from the JWT token
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Call the VendorService to update the comment
            var result = await _vendorService.UpdateCommentAsync(vendorId, commentId, model.Comment, model.Rank, userId);
            if (!result)
            {
                return NotFound("Comment or Vendor not found, or unauthorized to update this comment.");
            }

            return Ok("Comment updated successfully.");
        }
        
        
        
        [Authorize]
        [HttpDelete("{vendorId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(string vendorId, string commentId)
        {
            try
            {
                await _vendorService.DeleteCommentAsync(vendorId, commentId);
                return NoContent();  // Returns HTTP 204
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);  // Returns HTTP 404 if vendor or comment not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);  // Returns HTTP 500 for any other error
            }
        }




        // [Authorize]
        // [HttpPost("comment/{vendorId}")]
        // public async Task<IActionResult> AddComment(string vendorId, [FromBody] CommentModel model)
        // {
        //     var userId = User.FindFirst("UserId")?.Value;  // Get User ID from token
        //     await _vendorService.AddCommentAsync(vendorId, model.Comment, model.Rank, userId);
        //     return Ok("Comment added successfully.");
        // }
        //
        // [Authorize]
        // [HttpPut("comment/{vendorId}/{commentId}")]
        // public async Task<IActionResult> UpdateComment(string vendorId, string commentId, [FromBody] CommentModel model)
        // {
        //     var userId = User.FindFirst("UserId")?.Value;  // Get User ID from token
        //     await _vendorService.UpdateCommentAsync(vendorId, commentId, model.Comment, model.Rank, userId);
        //     return Ok("Comment updated successfully.");
        // }
        //
        // [Authorize]
        // [HttpDelete("comment/{vendorId}/{commentId}")]
        // public async Task<IActionResult> DeleteComment(string vendorId, string commentId)
        // {
        //     var userId = User.FindFirst("UserId")?.Value;  // Get User ID from token
        //     await _vendorService.DeleteCommentAsync(vendorId, commentId, userId);
        //     return Ok("Comment deleted successfully.");
        // }





    }

 

    
}
