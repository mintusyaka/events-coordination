// Pages/Sponsor/ApprovedEvents.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Sponsor
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
            if (userId == null) return RedirectToPage("/Login");

            ApprovedEvents = await _context.PlannedEvents
                .Where(p => p.UserId == userId)
                .Include(p => p.Event)
                    .ThenInclude(e => e.Venue)
                .Select(p => p.Event)
                .ToListAsync();

            return Page();
        }
    }
}
