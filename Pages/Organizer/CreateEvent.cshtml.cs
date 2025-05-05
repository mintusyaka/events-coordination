using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;
using events_coordination_frontend.DTOs;

namespace events_coordination_frontend.Pages.Organizer
{
    public class CreateEventModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public CreateEventModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EventDTO Event { get; set; }

        public List<SelectListItem> Venues { get; set; } = new();

        public IActionResult OnGet()
        {
            LoadVenues();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToPage("/Login");

            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.UserId == userId);
            if (organizer == null) return RedirectToPage("/Login");

            LoadVenues(); // заповнюємо навіть якщо форма невалідна

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newEvent = new Event
            {
                Title = Event.Title,
                Description = Event.Description,
                StartDate = DateOnly.FromDateTime(Event.StartDate),
                StartTime = TimeOnly.FromTimeSpan(Event.StartTime),
                EndDate = DateOnly.FromDateTime(Event.EndDate),
                EndTime = TimeOnly.FromTimeSpan(Event.EndTime),
                VenueId = Event.VenueId,
                OrganizerId = organizer.OrganizerId,
                Status = "planned"
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            // додаємо запис до PlannedEvents
            _context.PlannedEvents.Add(new PlannedEvent
            {
                EventId = newEvent.EventId,
                UserId = organizer.UserId
            });
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
                }).ToList();
        }
    }
}
