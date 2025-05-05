// Pages/Organizer/EditEvent.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using events_coordination_frontend.Models;
using events_coordination_frontend.DTOs;

namespace events_coordination_frontend.Pages.Organizer
{
    public class EditEventModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public EditEventModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EventDTO Event { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public List<SelectListItem> Venues { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == Id);
            if (existingEvent == null)
                return NotFound();

            Event = new EventDTO
            {
                Title = existingEvent.Title,
                Description = existingEvent.Description,
                StartDate = existingEvent.StartDate.ToDateTime(TimeOnly.MinValue),
                StartTime = existingEvent.StartTime.ToTimeSpan(),
                EndDate = existingEvent.EndDate.ToDateTime(TimeOnly.MinValue),
                EndTime = existingEvent.EndTime.ToTimeSpan(),
                VenueId = existingEvent.VenueId
            };

            LoadVenues();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == Id);
            if (ev == null)
                return NotFound();

            LoadVenues();
            if (!ModelState.IsValid)
                return Page();

            ev.Title = Event.Title;
            ev.Description = Event.Description;
            ev.StartDate = DateOnly.FromDateTime(Event.StartDate);
            ev.StartTime = TimeOnly.FromTimeSpan(Event.StartTime);
            ev.EndDate = DateOnly.FromDateTime(Event.EndDate);
            ev.EndTime = TimeOnly.FromTimeSpan(Event.EndTime);
            ev.VenueId = Event.VenueId;

            await _context.SaveChangesAsync();
            return RedirectToPage("/Organizer/Index");
        }

        private void LoadVenues()
        {
            Venues = _context.Venues
                .Select(v => new SelectListItem
                {
                    Value = v.VenueId.ToString(),
                    Text = v.Name + " (" + v.City + ")"
                })
                .ToList();
        }
    }
}
