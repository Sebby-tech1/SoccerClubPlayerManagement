using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoccerClubPlayerManagement.Data;
using SoccerClubPlayerManagement.Models;
using SoccerClubPlayerManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services (controllers + Razor views)
builder.Services.AddControllersWithViews();

// Register the EF Core DbContext, using the connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Azure Blob Storage for player photos (falls back to Azurite locally if no connection string set)
builder.Services.AddSingleton<BlobStorageService>();

// ASP.NET Core Identity with roles (Coach / Member), backed by the same DbContext
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Relaxed for a student/demo project — tighten these for production use.
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // required for wwwroot/uploads/players photos to be served

app.UseRouting();

app.UseAuthentication(); // must come before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed the Coach/Member roles on startup so the Register page's dropdown always works,
// even against a freshly migrated database.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var roleName in new[] { "Coach", "Member" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

app.Run();
