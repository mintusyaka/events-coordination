using Microsoft.EntityFrameworkCore;
using events_coordination_frontend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EventsCoordinationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSession();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
