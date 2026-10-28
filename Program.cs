using ErJobPortal.Repositories;
using ErJobPortal.Data;
using ErJobPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// MVC
builder.Services.AddControllersWithViews();

// Services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<VisitorTrackingService>();

// Database
builder.Services.AddScoped<DbConnection>();

// Repository
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<CandidateProfileRepository>();

// Register HttpClientFactory
builder.Services.AddHttpClient();

// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Exception handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();