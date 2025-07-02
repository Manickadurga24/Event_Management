using EventManagementUpdatedProject.Models;
using EventManagementUpdatedProject.DTO;
using EventManagementUpdatedProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _context.Events.ToListAsync();
            var result = events.Select(ev => new EventResponseDTO
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                StartDate = ev.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = ev.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Location = ev.Location,
                OrganizerName = ev.OrganizerName
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            var dto = new EventResponseDTO
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                StartDate = ev.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = ev.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Location = ev.Location,
                OrganizerName = ev.OrganizerName
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreateDTO dto)
        {
            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Location = dto.Location,
                OrganizerName = dto.OrganizerName
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEventById), new { id = ev.Id }, ev);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventUpdateDTO dto)
        {
            if (id != dto.Id) return BadRequest("Event ID mismatch.");

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.StartDate = dto.StartDate;
            ev.EndDate = dto.EndDate;
            ev.Location = dto.Location;
            ev.OrganizerName = dto.OrganizerName;

            await _context.SaveChangesAsync();
            return Ok(ev);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
