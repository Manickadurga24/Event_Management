using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EventManagementUpdatedProject.Models;
using EventManagementUpdatedProject.DTO;

namespace EventManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // For managing roles
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new { Message = "Password and confirm password do not match." });
            }

            // Check if the email is already registered
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Email address is already registered." });
            }

            // Parse UserRole from string input
            if (!Enum.TryParse(model.Type, true, out UserRole userRole))
            {
                return BadRequest(new { Message = "Invalid user role specified." });
            }

            var user = new AppUser
            {
                UserName = model.Email, // Identity uses UserName for login; often set to Email
                Email = model.Email,
                Name = model.Name,
                ContactNumber = model.ContactNumber,
                Type = userRole,
                EmailConfirmed = true // For development, you might set this to true to skip email confirmation
                                      // In production, implement email confirmation
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                string roleName = userRole.ToString();

                // Now we throw an error if the role doesn't already exist
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    return BadRequest(new { Message = $"Role '{roleName}' does not exist. Please contact an administrator." });
                }

                // Only assign the role if it already exists
                await _userManager.AddToRoleAsync(user, roleName);

                return Ok(new { Message = "Registration successful." });
            }


            //if (result.Succeeded)
            //{
            //    // Ensure the role exists before assigning
            //    string roleName = userRole.ToString();
            //    if (!await _roleManager.RoleExistsAsync(roleName))
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            //    }
            //    await _userManager.AddToRoleAsync(user, roleName);

            //    return Ok(new { Message = "Registration successful." });
            //}

            // If creation failed, return errors
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Message = "User creation failed.", Errors = errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid login attempt. User not found." });
            }

            // Check the password
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Generate JWT Token
                var token = await GenerateJwtToken(user); // Make GenerateJwtToken async
                return Ok(new { Token = token });
            }

            // If login failed, provide a generic message to prevent enumeration attacks
            return Unauthorized(new { Message = "Invalid login attempt." });
        }

        // New Helper method to generate JWT token
        private async Task<string> GenerateJwtToken(AppUser user) // Made async to get roles
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""), // Sub is subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Jti is unique token ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User's unique ID
                new Claim(ClaimTypes.Email, user.Email ?? ""), // User's email
                new Claim(ClaimTypes.Name, user.Name ?? "") // User's display name
            };

            // Add user roles to claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // Add each role as a claim
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"] ?? "7")); // Token expiry

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured."),
                audience: _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured."),
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
