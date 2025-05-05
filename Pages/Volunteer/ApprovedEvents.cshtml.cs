// Pages/Volunteer/ApprovedEvents.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Volunteer
{
    public class ApprovedEventsModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public ApprovedEventsModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<Event> ApprovedEvents { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return RedirectToPage("/Login");

            var plannedIds = await _context.PlannedEvents
                .Where(pe => pe.UserId == userId)
                .Select(pe => pe.EventId)
                .ToListAsync();

            ApprovedEvents = await _context.Events
                .Where(e => plannedIds.Contains(e.EventId))
                .Include(e => e.Venue)
                .ToListAsync();

            return Page();
        }
    }
}
