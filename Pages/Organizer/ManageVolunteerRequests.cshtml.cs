// Pages/Organizer/ManageVolunteerRequests.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages.Organizer
{
    public class ManageVolunteerRequestsModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public ManageVolunteerRequestsModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        public Dictionary<Event, List<User>> VolunteersByEvent { get; set; } = new();
        public Dictionary<Event, List<User>> ApprovedVolunteersByEvent { get; set; } = new();

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
                // Pending volunteers
                var pendingVolunteers = await _context.EventVolunteers
                    .Where(evv => evv.EventId == ev.EventId)
                    .Join(_context.Volunteers,
                        evv => evv.VolunteerId,
                        v => v.VolunteerId,
                        (evv, v) => v.UserId)
                    .Join(_context.Users,
                        uid => uid,
                        user => user.UserId,
                        (uid, user) => user)
                    .ToListAsync();

                if (pendingVolunteers.Any())
                {
                    VolunteersByEvent[ev] = pendingVolunteers;
                }

                // Approved volunteers
                var approvedVolunteers = await _context.PlannedEvents
                    .Where(pe => pe.EventId == ev.EventId)
                    .Join(_context.Volunteers,
                        pe => pe.UserId,
                        v => v.UserId,
                        (pe, v) => v.UserId)
                    .Join(_context.Users,
                        uid => uid,
                        u => u.UserId,
                        (uid, user) => user)
                    .ToListAsync();


                if (approvedVolunteers.Any())
                {
                    ApprovedVolunteersByEvent[ev] = approvedVolunteers;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int eventId, int userId)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);
            if (volunteer == null) return RedirectToPage();

            bool alreadyPlanned = await _context.PlannedEvents.AnyAsync(p => p.EventId == eventId && p.UserId == userId);
            if (!alreadyPlanned)
            {
                _context.PlannedEvents.Add(new PlannedEvent
                {
                    EventId = eventId,
                    UserId = userId
                });
            }

            var ev = await _context.EventVolunteers
                .FirstOrDefaultAsync(e => e.EventId == eventId && e.VolunteerId == volunteer.VolunteerId);

            if (ev != null)
            {
                _context.EventVolunteers.Remove(ev);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int eventId, int userId)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);
            if (volunteer == null) return RedirectToPage();

            var ev = await _context.EventVolunteers
                .FirstOrDefaultAsync(e => e.EventId == eventId && e.VolunteerId == volunteer.VolunteerId);

            if (ev != null)
            {
                _context.EventVolunteers.Remove(ev);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveApprovedAsync(int eventId, int userId)
        {
            var planned = await _context.PlannedEvents
                .FirstOrDefaultAsync(pe => pe.EventId == eventId && pe.UserId == userId);

            if (planned != null)
            {
                _context.PlannedEvents.Remove(planned);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
