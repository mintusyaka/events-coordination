using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace events_coordination_frontend.Pages
{
    public class DashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("role");

            return role switch
            {
                "Organizer" => RedirectToPage("/Organizer/Index"),
                "Sponsor" => RedirectToPage("/Sponsor/Index"),
                "Volunteer" => RedirectToPage("/Volunteer/Index"),
                _ => RedirectToPage("/Login") // нрср аскю онлхкйю
            };
        }
    }
}
