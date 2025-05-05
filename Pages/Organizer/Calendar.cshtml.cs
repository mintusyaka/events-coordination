using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Organizer
{
    public class CalendarModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public CalendarModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<object> CalendarEvents { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.UserId == userId);
            if (organizer == null) return RedirectToPage("/Login");

            var events = await _context.Events
                .Where(e => e.OrganizerId == organizer.OrganizerId)
                .Include(e => e.Venue)
                .ToListAsync();

            CalendarEvents = new List<object>();

            foreach (var e in events)
            {
                var sponsorCount = await _context.PlannedEvents
                    .CountAsync(p => p.EventId == e.EventId && _context.Sponsors.Any(s => s.UserId == p.UserId));

                var volunteerCount = await _context.PlannedEvents
                    .CountAsync(p => p.EventId == e.EventId && _context.Volunteers.Any(v => v.UserId == p.UserId));

                CalendarEvents.Add(new
                {
                    title = e.Title,
                    start = e.StartDate.ToString("yyyy-MM-dd") + "T" + e.StartTime.ToString("hh\\:mm"),
                    end = e.EndDate.ToString("yyyy-MM-dd") + "T" + e.EndTime.ToString("hh\\:mm"),
                    description = e.Description ?? "",
                    eventId = e.EventId,
                    sponsorCount,
                    volunteerCount
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int eventId)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (@event == null) return NotFound();

            // Видаляємо пов’язані записи
            var sponsors = _context.EventSponsors.Where(x => x.EventId == eventId);
            var volunteers = _context.EventVolunteers.Where(x => x.EventId == eventId);
            var plans = _context.PlannedEvents.Where(x => x.EventId == eventId);

            _context.EventSponsors.RemoveRange(sponsors);
            _context.EventVolunteers.RemoveRange(volunteers);
            _context.PlannedEvents.RemoveRange(plans);

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
