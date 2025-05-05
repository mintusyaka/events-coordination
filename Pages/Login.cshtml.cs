using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages
{
    public class LoginModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public LoginModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user == null)
            {
                ErrorMessage = "Невірний email або пароль.";
                return Page();
            }

            string role = null;
            if (_context.Organizers.Any(o => o.UserId == user.UserId))
                role = "Organizer";
            else if (_context.Sponsors.Any(s => s.UserId == user.UserId))
                role = "Sponsor";
            else if (_context.Volunteers.Any(v => v.UserId == user.UserId))
                role = "Volunteer";

            if (role == null)
            {
                ErrorMessage = "Роль не визначена.";
                return Page();
            }

            HttpContext.Session.SetInt32("userId", user.UserId);
            HttpContext.Session.SetString("role", role);

            return RedirectToPage("/Dashboard");
        }
    }
}
