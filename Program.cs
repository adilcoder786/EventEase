using EventEase.Components;
using EventEase.Data;
using EventEase.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Blazor (Interactive Server rendering) ----------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
        options.DetailedErrors = builder.Environment.IsDevelopment());

// ---------- Database (EF Core + SQLite) ----------
// A DbContext *factory* is the recommended pattern for Blazor Server:
// components live for a long time, so each operation creates a short-lived context.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=eventease.db";
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// ---------- Application services (Dependency Injection) ----------
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
// "Scoped" in Blazor Server = one instance per browser tab/circuit,
// which is exactly what we want for per-user session state and toasts.
builder.Services.AddScoped<UserSessionState>();
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

// ---------- Database initialisation (migrate + seed sample data) ----------
await DbInitializer.InitializeAsync(app.Services);

// ---------- HTTP pipeline ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
