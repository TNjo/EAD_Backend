using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendServices.DTOs;
using BackendServices.Helpers;
using BackendServices.Models;
using BackendServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;


namespace BackendServices.Controllers;

// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

// [ApiController]
    // [Route("api/[controller]")]
    // public class AuthController : ControllerBase
    // {
    //     private readonly IMongoCollection<User> _users;
    //     private readonly IConfiguration _configuration;
    //
    //     public AuthController(IMongoDatabase database, IConfiguration configuration)
    //     {
    //         _users = database.GetCollection<User>("Users");
    //         _configuration = configuration;
    //     }
    //
    //     [HttpPost("register")]
    //     public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
    //     {
    //         var userExists = await _users.Find(u => u.Username == registerDto.Username).FirstOrDefaultAsync();
    //         if (userExists != null)
    //         {
    //             return BadRequest("User already exists.");
    //         }
    //
    //         /*var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password); */
    //         var hashedPassword = BCrypt.HashPassword(registerDto.Password);
    //
    //
    //         var user = new User
    //         {
    //             Username = registerDto.Username,
    //             PasswordHash = hashedPassword,
    //             Role = "User" // Default role for regular users
    //         };
    //
    //         await _users.InsertOneAsync(user);
    //
    //         return Ok("User registered successfully.");
    //     }
    //     
    //     
    //     // [HttpPost("login")]
    //     // public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    //     // {
    //     //     var user = await _users.Find(u => u.Username == loginDto.Username).FirstOrDefaultAsync();
    //     //     if (user == null || !BCrypt.Verify(loginDto.Password, user.PasswordHash))
    //     //     {
    //     //         return Unauthorized("Invalid credentials.");
    //     //     }
    //     //
    //     //     var tokenHandler = new JwtSecurityTokenHandler();
    //     //     var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Use the key from appsettings.json
    //     //     var tokenDescriptor = new SecurityTokenDescriptor
    //     //     {
    //     //         Subject = new ClaimsIdentity(new[]
    //     //         {
    //     //             new Claim(ClaimTypes.Name, user.Username),
    //     //             new Claim(ClaimTypes.Role, user.Role)
    //     //         }),
    //     //         Expires = DateTime.UtcNow.AddHours(1),
    //     //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //     //     };
    //     //     var token = tokenHandler.CreateToken(tokenDescriptor);
    //     //     var tokenString = tokenHandler.WriteToken(token);
    //     //
    //     //     return Ok(new { Token = tokenString });
    //     // }
    //     
    //     [HttpPost("login")]
    //     public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    //     {
    //         var user = await _users.Find(u => u.Username == loginDto.Username).FirstOrDefaultAsync();
    //         if (user == null || !BCrypt.Verify(loginDto.Password, user.PasswordHash))
    //         {
    //             return Unauthorized("Invalid credentials.");
    //         }
    //
    //         var tokenHandler = new JwtSecurityTokenHandler();
    //         var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
    //         var tokenDescriptor = new SecurityTokenDescriptor
    //         {
    //             Subject = new ClaimsIdentity(new[]
    //             {
    //                 new Claim(ClaimTypes.Name, user.Username), // Use Username for authentication
    //                 new Claim("VendorId", user.Id), // Store ObjectId as VendorId
    //                 new Claim(ClaimTypes.Role, user.Role) // Store Role
    //             }),
    //             Expires = DateTime.UtcNow.AddHours(1),
    //             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //         };
    //         var token = tokenHandler.CreateToken(tokenDescriptor);
    //         var tokenString = tokenHandler.WriteToken(token);
    //
    //         return Ok(new { Token = tokenString });
    //     }
    //     
    // }



    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtHelper _jwtHelper;

        public AuthController(UserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!await _userService.IsEmailUniqueAsync(model.Email))
            {
                return BadRequest("Email already exists.");
            }

            var newUser = new User
            {
                Email = model.Email,
                Password = UserService.EncryptPassword(model.Password),
                Username = model.Username,
                Role = "User",  // Default role is User
                Status = 1  // Active by default
            };

            await _userService.CreateUserAsync(newUser);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null || !UserService.VerifyPassword(model.Password, user.Password) || user.Status != 1)
            {
                return Unauthorized("Invalid credentials or inactive account.");
            }

            var token = _jwtHelper.GenerateJwtToken(user);
            return Ok(new { Token = token, Role = user.Role });
        }

        [Authorize]
        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateAccount()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Status = 0;
            await _userService.UpdateUserAsync(user);
            return Ok("Account deactivated.");
        }

        [Authorize(Roles = "Customer Service Representative")]
        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAccount([FromBody] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Status = 1;
            await _userService.UpdateUserAsync(user);
            return Ok("Account activated.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null || !UserService.VerifyPassword(model.OldPassword, user.Password))
            {
                return BadRequest("Invalid credentials.");
            }

            user.Password = UserService.EncryptPassword(model.NewPassword);
            await _userService.UpdateUserAsync(user);
            return Ok("Password updated.");
        }

        // Implement the forget password with email verification and password reset logic similarly.


        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Vendors cannot delete their accounts
            if (user.Role == "Vendor")
            {
                return Forbid("Vendors cannot delete their own accounts.");
            }

            // Admin or CSR can delete any account
            if (role == "Admin" || role == "Customer Service Representative")
            {
                await _userService.DeleteUserAsync(userId);
                return Ok("User account deleted by Admin/CSR.");
            }

            // Normal users can delete their own accounts
            if (userId == user.Id)
            {
                await _userService.DeleteUserAsync(userId);
                return Ok("Your account has been deleted.");
            }

            return Forbid("Unauthorized to delete this account.");
        }

        [Authorize(Roles = "Admin, Customer Service Representative, Vendor")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Get all users from the UserService
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin, Customer Service Representative, Vendor")]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            // Get a single user by their Id
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }


    }

    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

