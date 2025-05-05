using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Volunteer
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

            var events = await _context.PlannedEvents
                .Where(pe => pe.UserId == userId)
                .Include(pe => pe.Event)
                .ThenInclude(e => e.Venue)
                .Select(pe => pe.Event)
                .ToListAsync();

            CalendarEvents = events.Select(e => new
            {
                title = e.Title,
                start = e.StartDate.ToString("yyyy-MM-dd") + "T" + e.StartTime.ToString("hh\\:mm"),
                end = e.EndDate.ToString("yyyy-MM-dd") + "T" + e.EndTime.ToString("hh\\:mm"),
                description = e.Description ?? "",
                eventId = e.EventId  // <== обовʼязково!
            }).ToList<object>();




            return Page();
        }

        public async Task<IActionResult> OnPostWithdrawAsync(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var planned = await _context.PlannedEvents
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

            if (planned != null)
            {
                _context.PlannedEvents.Remove(planned);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

    }
}
