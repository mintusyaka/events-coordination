using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using events_coordination_frontend.Models;

namespace events_coordination_frontend.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly EventsCoordinationContext _context;

        public RegisterModel(EventsCoordinationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInput Input { get; set; }

        public string? ErrorMessage { get; set; }

        public class RegisterInput
        {
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Email { get; set; } = "";
            public string Phone { get; set; } = "";
            public string Password { get; set; } = "";
            public string Role { get; set; } = "";
            public string? Organization { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // 1. Додаємо в Users
            var user = new User
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Phone = Input.Phone,
                Password = Input.Password,
                Balance = 0
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 2. Додаємо в рольову таблицю
            switch (Input.Role)
            {
                case "Volunteer":
                    _context.Volunteers.Add(new Models.Volunteer
                    {
                        UserId = user.UserId
                    });
                    break;

                case "Sponsor":
                    if (string.IsNullOrWhiteSpace(Input.Organization))
                    {
                        ErrorMessage = "Поле 'Організація' є обов'язковим для ролі Спонсор.";
                        return Page();
                    }

                    _context.Sponsors.Add(new Models.Sponsor
                    {
                        UserId = user.UserId,
                        Organization = Input.Organization
                    });
                    break;

                case "Organizer":
                    if (string.IsNullOrWhiteSpace(Input.Organization))
                    {
                        ErrorMessage = "Поле 'Організація' є обов'язковим для ролі Організатор.";
                        return Page();
                    }

                    _context.Organizers.Add(new Models.Organizer
                    {
                        UserId = user.UserId,
                        Organization = Input.Organization
                    });
                    break;

                default:
                    ErrorMessage = "Оберіть правильну роль.";
                    return Page();
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Login");
        }
    }
}
