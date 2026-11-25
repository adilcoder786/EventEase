# 🎟️ EventEase - Event Management System

EventEase is a modern full-stack event management web application built with **ASP.NET Core** and **Blazor (Interactive Server)**. Visitors can discover events, search and filter them, register with validated forms, manage their registrations, and organisers can track attendance and view live statistics.

> Built as the final project of the Coursera **Microsoft Full Stack Development** course.

## ✨ Features

- **Dashboard** - total events, total registrations, upcoming events, attendance statistics and recent registrations
- **Event management (full CRUD)** - create, view, edit and delete events (name, description, date, start time, location, category, capacity, available seats, image)
- **Reusable `EventCard` component** - title, description, date/time, location, category, seats left, *View Details* and *Register Now* buttons, plus a "Save for later" checkbox using two-way `@bind`
- **Search, filter & pagination** - debounced search, category filter, upcoming-only filter, paged results
- **Registration form** - `EditForm` + `DataAnnotationsValidator`, required/email/phone validation, duplicate prevention, capacity validation, success & error messages
- **My Registrations** - look up and cancel your registrations (seats are released automatically)
- **Attendance tracker** - filter by event, mark Present / Absent / Reset, totals and attendance percentage
- **State management** - scoped `UserSessionState` service (pre-filled forms, saved events, registered badges, inactivity timeout) and `NotificationService` (toasts)
- **Modern responsive UI** - custom design system, mobile navigation, hover effects, loading indicators, empty states, toast notifications, custom 404 page

## 🧰 Technology stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core 8, Blazor Web App (Interactive Server rendering) |
| Language | C# 12 |
| Database | SQLite via Entity Framework Core 8 |
| Validation | Data Annotations |
| Front end | HTML5, CSS3 (no external UI framework) |
| Patterns | Dependency Injection, service layer, `IDbContextFactory`, async/await |
| Version control | Git & GitHub |

## ✅ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (check with `dotnet --version`)
- Git
- Visual Studio 2022 or VS Code (with the C# Dev Kit)

## 🚀 Installation & running

```bash
# 1. Clone your repository
git clone https://github.com/<your-username>/EventEase.git
cd EventEase

# 2. Restore packages
dotnet restore

# 3. (Recommended) install the EF Core tool and create the migration
dotnet tool install --global dotnet-ef --version 8.0.*
dotnet ef migrations add InitialCreate --output-dir Data/Migrations

# 4. Run the app (the database is created and sample data inserted automatically)
dotnet run
```

Open the URL printed in the terminal (for example `http://localhost:5080`).

**About migrations:** `Program.cs` calls `DbInitializer`, which applies migrations if they exist (`Database.Migrate()`), otherwise it creates the schema with `EnsureCreated()`, so the project runs either way. Generate and commit the migration (step 3) to demonstrate database migrations in your assignment. If you ran the app *before* creating the migration, delete `eventease.db` first, then create the migration and run again.

## 🧭 Routes

| Route | Page |
|---|---|
| `/` | Landing page |
| `/events` | Browse, search & filter events |
| `/events/{id}` | Event details (with edit & delete) |
| `/create-event` | Create event |
| `/edit-event/{id}` | Edit event |
| `/register/{eventId}` (or `/register`) | Registration form |
| `/my-registrations` | Look up / cancel registrations |
| `/attendance` | Attendance tracker |
| `/dashboard` | Statistics dashboard |
| anything else | Custom 404 page |

## 📁 Project structure

```
EventEase/
├── Program.cs                     # Startup, DI registration, pipeline
├── appsettings.json               # Connection string + session timeout (no secrets)
├── Components/
│   ├── App.razor, Routes.razor    # Root component and router (+ 404 handling)
│   ├── Layout/                    # MainLayout, NavMenu
│   ├── Pages/                     # Home, Events, EventDetails, CreateEvent, EditEvent,
│   │                              # Register, MyRegistrations, Attendance, Dashboard, Error
│   └── Shared/                    # EventCard, EventEditor, Pagination, ToastContainer,
│                                  # LoadingSpinner, EmptyState, NotFoundContent
├── Models/                        # Event, Registration, enums, result/paging types
├── Data/                          # ApplicationDbContext, DbInitializer (+ Migrations/)
├── Services/                      # Event, Registration, Dashboard services,
│                                  # UserSessionState, NotificationService
├── wwwroot/css/app.css            # Design system
├── wwwroot/images/                # Event illustrations
└── docs/                          # Copilot summary, GitHub guide, checklists
```

## 🔒 Security notes

- The SQLite connection string contains **no password**; there are no secrets in this repository.
- `.gitignore` excludes database files, build output, and secret/config overrides.
- There is **no fake login**. Session state only stores convenience data (name/email for pre-filling forms) and expires after inactivity. If you add login later, use ASP.NET Core Identity.
- "My Registrations" is a simple email lookup for this learning project and is not a substitute for authentication.

## 📸 Screenshots

Add your screenshots to a `docs/screenshots/` folder and update the links below.

| Page | Screenshot |
|---|---|
| Home / landing page | `docs/screenshots/home.png` |
| Events list with search | `docs/screenshots/events.png` |
| Event details | `docs/screenshots/details.png` |
| Registration form with validation errors | `docs/screenshots/register-validation.png` |
| Registration success | `docs/screenshots/register-success.png` |
| Attendance tracker | `docs/screenshots/attendance.png` |
| Dashboard | `docs/screenshots/dashboard.png` |
| Mobile layout | `docs/screenshots/mobile.png` |

Example: `![Home](docs/screenshots/home.png)`

## 📝 License

Educational project. Free to use for learning.
