using EventManagementUpdatedProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/Dashboard
    [Authorize]// all action require authentication
    public class DashboardController : Controller
    {
        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            // Access user claims from the authenticated token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Identity's user ID
            var userName = User.FindFirstValue(ClaimTypes.Name); // User's Name from AppUser
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // User's email
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new
            {
                Message = $"Welcome, {userName}! You are logged in.",
                UserId = userId,
                Email = userEmail,
                Role = userRole
            });
        }

        [HttpGet("organizer-dashboard")]
        [Authorize(Roles = nameof(UserRole.Organizer))] // Only Organizer role can access
        public IActionResult GetOrganizerDashboard()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { Message = $"Hello Organizer {userName}! Here is your event dashboard data." });
        }

        [HttpGet("admin-dashboard")]
        [Authorize(Roles = nameof(UserRole.Admin))] // Only Admin role can access
        public IActionResult GetAdminDashboard()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { Message = $"Greetings Admin {userName}! You have full access." });
        }

        [HttpGet("public-data")]
        [AllowAnonymous] // This action does NOT require authentication
        public IActionResult GetPublicData()
        {
            return Ok(new { Message = "This data is accessible by anyone, authenticated or not." });
        }
    }
}
