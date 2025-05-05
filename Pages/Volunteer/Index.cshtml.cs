using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Volunteer
{
    public class IndexModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public IndexModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<Event> AvailableEvents { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? City { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            // Отримуємо ID подій, у яких користувач або вже бере участь, або подав заявку
            var appliedEventIds = await _context.EventVolunteers
                .Where(ev => ev.Volunteer.UserId == userId)
                .Select(ev => ev.EventId)
                .ToListAsync();

            var approvedEventIds = await _context.PlannedEvents
                .Where(pe => pe.UserId == userId)
                .Select(pe => pe.EventId)
                .ToListAsync();

            var excludedIds = appliedEventIds.Union(approvedEventIds).ToHashSet();

            var query = _context.Events
                .Include(e => e.Venue)
                .Where(e => !excludedIds.Contains(e.EventId)); // Фільтруємо

            if (!string.IsNullOrWhiteSpace(City))
            {
                query = query.Where(e => e.Venue.City.Contains(City));
            }

            if (FromDate != null)
            {
                query = query.Where(e => e.StartDate >= DateOnly.FromDateTime(FromDate.Value));
            }

            if (ToDate != null)
            {
                query = query.Where(e => e.StartDate <= DateOnly.FromDateTime(ToDate.Value));
            }

            AvailableEvents = await query.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostApplyAsync(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);
            if (volunteer == null) return RedirectToPage("/Login");

            // Перевіряємо, чи вже є така заявка
            bool alreadyApplied = await _context.EventVolunteers
                .AnyAsync(ev => ev.EventId == eventId && ev.VolunteerId == volunteer.VolunteerId);

            if (!alreadyApplied)
            {
                _context.EventVolunteers.Add(new EventVolunteer
                {
                    EventId = eventId,
                    VolunteerId = volunteer.VolunteerId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

    }
}
