// Pages/Sponsor/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace events_coordination_frontend.Pages.Sponsor
{
    public class IndexModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public IndexModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public List<Event> Events { get; set; } = new();
        public List<SelectListItem> SponsorshipTypes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? City { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty]
        public int SelectedSponsorshipId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            SponsorshipTypes = await _context.SponsorshipTypes
                .Select(st => new SelectListItem { Value = st.SponsorshipId.ToString(), Text = st.SponsorshipType1 })
                .ToListAsync();

            var query = _context.Events
                .Include(e => e.Venue)
                .Where(e => e.Status == "planned")
                .AsQueryable();

            if (!string.IsNullOrEmpty(City))
                query = query.Where(e => e.Venue.City.Contains(City));

            if (FromDate.HasValue)
                query = query.Where(e => e.StartDate >= DateOnly.FromDateTime(FromDate.Value));

            if (ToDate.HasValue)
                query = query.Where(e => e.StartDate <= DateOnly.FromDateTime(ToDate.Value));

            Events = await query.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRequestAsync(int eventId, int selectedSponsorshipId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sponsor == null) return RedirectToPage("/Login");

            bool alreadyRequested = await _context.EventSponsors.AnyAsync(es => es.EventId == eventId && es.SponsorId == sponsor.SponsorId);
            if (!alreadyRequested)
            {
                _context.EventSponsors.Add(new EventSponsor
                {
                    EventId = eventId,
                    SponsorId = sponsor.SponsorId,
                    SponsorshipId = selectedSponsorshipId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
