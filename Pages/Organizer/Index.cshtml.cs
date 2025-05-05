using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Organizer
{
    public class IndexModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public IndexModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<Event> Events { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return RedirectToPage("/Login");

            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.UserId == userId);
            if (organizer == null)
                return RedirectToPage("/Login");

            Events = await _context.Events
                .Where(e => e.OrganizerId == organizer.OrganizerId)
                .Include(e => e.Venue)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                // видалити всі зв'язки з PlannedEvents
                var planned = await _context.PlannedEvents
                    .Where(pe => pe.EventId == id)
                    .ToListAsync();

                _context.PlannedEvents.RemoveRange(planned);

                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

    }
}
