using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Sponsor
{
    public class AppliedEventsModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public AppliedEventsModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<(Event Event, string SponsorshipType)> RequestedEvents { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sponsor == null) return RedirectToPage("/Login");

            // Отримуємо ідентифікатори подій, які вже підтверджено
            var plannedEventIds = await _context.PlannedEvents
                .Where(pe => pe.UserId == userId)
                .Select(pe => pe.EventId)
                .ToListAsync();

            // Отримуємо заявки, які ще не підтверджені
            RequestedEvents = await _context.EventSponsors
                .Where(es => es.SponsorId == sponsor.SponsorId && !plannedEventIds.Contains(es.EventId))
                .Include(es => es.Event).ThenInclude(e => e.Venue)
                .Include(es => es.Sponsorship)
                .Select(es => new ValueTuple<Event, string>(es.Event, es.Sponsorship.SponsorshipType1))
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostWithdrawAsync(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sponsor == null) return RedirectToPage("/Login");

            var request = await _context.EventSponsors
                .FirstOrDefaultAsync(es => es.EventId == eventId && es.SponsorId == sponsor.SponsorId);

            if (request != null)
            {
                _context.EventSponsors.Remove(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
