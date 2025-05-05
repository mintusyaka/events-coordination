// Pages/Volunteer/AppliedEvents.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Volunteer
{
    public class AppliedEventsModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public AppliedEventsModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<Event> AppliedEvents { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return RedirectToPage("/Login");

            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);
            if (volunteer == null)
                return RedirectToPage("/Login");

            var appliedIds = await _context.EventVolunteers
                .Where(ev => ev.VolunteerId == volunteer.VolunteerId)
                .Select(ev => ev.EventId)
                .ToListAsync();

            AppliedEvents = await _context.Events
                .Where(e => appliedIds.Contains(e.EventId))
                .Include(e => e.Venue)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);
            if (volunteer == null) return RedirectToPage();

            var record = await _context.EventVolunteers
                .FirstOrDefaultAsync(ev => ev.EventId == eventId && ev.VolunteerId == volunteer.VolunteerId);

            if (record != null)
            {
                _context.EventVolunteers.Remove(record);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

    }
}
