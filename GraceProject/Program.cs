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

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<GraceDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<StudentService>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await CreateRolesAndAdmin(services);
//}

//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    string[] roleNames = { "Admin", "Student", "Educator" };

//    foreach (var roleName in roleNames)
//    {
//        if (!await roleManager.RoleExistsAsync(roleName))
//        {
//            await roleManager.CreateAsync(new IdentityRole(roleName));
//        }
//    }
//}

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


//async Task CreateRolesAndAdmin(IServiceProvider serviceProvider)
//{
//    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//    string[] roleNames = { "Admin", "Educator", "Student", "Guest" };

//    foreach (var roleName in roleNames)
//    {
//        if (!await roleManager.RoleExistsAsync(roleName))
//        {
//            await roleManager.CreateAsync(new IdentityRole(roleName));
//        }
//    }

//    // Create default admin user (Modify Email/Password as needed)
//    string adminEmail = "admin@example.com";
//    string adminPassword = "Admin@123";

//    var adminUser = await userManager.FindByEmailAsync(adminEmail);
//    if (adminUser == null)
//    {
//        var newAdmin = new ApplicationUser
//        {
//            UserName = adminEmail,
//            Email = adminEmail,
//            EmailConfirmed = true,
//            FirstName = "Admin",  
//            LastName = "User"    
//        };

//        var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);

//        if (createAdmin.Succeeded)
//        {
//            await userManager.AddToRoleAsync(newAdmin, "Admin");
//        }
//    }
//}

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
