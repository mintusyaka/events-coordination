// Pages/Organizer/ManageSponsorRequests.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Organizer
{
    public class ManageSponsorRequestsModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public ManageSponsorRequestsModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<(Event, User, string)> PendingSponsorRequests { get; set; } = new();
        public List<(Event, User, string)> ApprovedSponsors { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.UserId == userId);
            if (organizer == null) return RedirectToPage("/Login");

            var myEvents = await _context.Events
                .Where(e => e.OrganizerId == organizer.OrganizerId)
                .ToListAsync();

            foreach (var ev in myEvents)
            {
                // --- Очікувані спонсори ---
                var pending = await _context.EventSponsors
                    .Where(es => es.EventId == ev.EventId)
                    .Include(es => es.Sponsorship)
                    .Join(_context.Sponsors, es => es.SponsorId, s => s.SponsorId, (es, s) => new { es, s })
                    .Join(_context.Users, x => x.s.UserId, u => u.UserId, (x, u) => new { x.es, u })
                    .ToListAsync();

                foreach (var item in pending)
                {
                    bool isApproved = await _context.PlannedEvents
                        .AnyAsync(p => p.EventId == ev.EventId && p.UserId == item.u.UserId);

                    if (!isApproved)
                    {
                        var sponsorshipType = item.es.Sponsorship?.SponsorshipType1 ?? "Не вказано";
                        PendingSponsorRequests.Add((ev, item.u, sponsorshipType));
                    }
                }

                // --- Підтверджені спонсори ---
                var approvedSponsorUserIds = await _context.PlannedEvents
                    .Where(p => p.EventId == ev.EventId)
                    .Select(p => p.UserId)
                    .ToListAsync();

                foreach (var userIdInEvent in approvedSponsorUserIds)
                {
                    var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.UserId == userIdInEvent);
                    if (sponsor == null) continue; // Пропускаємо не-спонсора

                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userIdInEvent);
                    if (user == null) continue;

                    var es = await _context.EventSponsors
                        .Where(x => x.EventId == ev.EventId && x.SponsorId == sponsor.SponsorId)
                        .Include(x => x.Sponsorship)
                        .FirstOrDefaultAsync();

                    var type = es?.Sponsorship?.SponsorshipType1 ?? "Не вказано";
                    ApprovedSponsors.Add((ev, user, type));
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int eventId, int userId)
        {
            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sponsor == null) return RedirectToPage();

            var entry = await _context.EventSponsors
                .FirstOrDefaultAsync(es => es.EventId == eventId && es.SponsorId == sponsor.SponsorId);

            if (entry != null)
            {
                var alreadyPlanned = await _context.PlannedEvents.AnyAsync(p => p.EventId == eventId && p.UserId == userId);
                if (!alreadyPlanned)
                {
                    _context.PlannedEvents.Add(new PlannedEvent
                    {
                        EventId = eventId,
                        UserId = userId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int eventId, int userId)
        {
            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sponsor == null) return RedirectToPage();

            var entry = await _context.EventSponsors
                .FirstOrDefaultAsync(es => es.EventId == eventId && es.SponsorId == sponsor.SponsorId);

            if (entry != null)
            {
                _context.EventSponsors.Remove(entry);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveApprovedAsync(int eventId, int userId)
        {
            var planned = await _context.PlannedEvents
                .FirstOrDefaultAsync(pe => pe.EventId == eventId && pe.UserId == userId);

            if (planned != null)
            {
                _context.PlannedEvents.Remove(planned);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
