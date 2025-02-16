using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GraceProject.Data;
using GraceProject.Models;
using GraceProject;
using Serilog;  // Add this for Serilog
using Microsoft.AspNetCore.Authentication;
using GraceProject.Controllers.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Initialize Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("AppLogs/myapplog-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();  // Use Serilog for logging

var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

builder.Services.AddDbContext<GraceDbContext>(options => options.UseSqlServer(connectionString));

var environmentName = builder.Configuration["AppEnvironment"];

bool requireConfirmedAccount = environmentName.Equals("Production", StringComparison.OrdinalIgnoreCase);

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = requireConfirmedAccount)
    .AddEntityFrameworkStores<GraceDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Custom authentication middleware, always check if the user is not login , redirect to the login page
app.UseMiddleware<Authentication>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
