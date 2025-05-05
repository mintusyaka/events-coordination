using events_coordination_frontend.Models;

namespace events_coordination_frontend.Services
{
    public class RoleService
    {
        private readonly EventsCoordinationContext _context;

        public RoleService(EventsCoordinationContext context)
        {
            _context = context;
        }

        // Перевіряємо, чи є організатором
        public bool IsOrganizer(string userEmail)
        {
            var user = _context.Users.FirstOrDefault(o => o.Email == userEmail);
            if (user != null)
                return _context.Organizers.Any(o => o.UserId == user.UserId);
            else return false;
        }

        // Перевіряємо, чи є волонтером
        public bool IsVolunteer(string userEmail)
        {
            var user = _context.Users.FirstOrDefault(o => o.Email == userEmail);
            if (user != null)
                return _context.Volunteers.Any(o => o.UserId == user.UserId);
            else return false;
        }

        // Перевіряємо, чи є спонсором
        public bool IsSponsor(string userEmail)
        {
            var user = _context.Users.FirstOrDefault(o => o.Email == userEmail);
            if (user != null)
                return _context.Sponsors.Any(o => o.UserId == user.UserId);
            else return false;
        }
    }

}
