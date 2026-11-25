# Step-by-step guide (beginner friendly)

| Step | Files | What it does |
|---|---|---|
| 1 Setup | `EventEase.csproj`, `Program.cs`, `appsettings.json` | Defines packages, registers services and starts the web app |
| 2 Models | `Models/Event.cs`, `Registration.cs`, `AttendanceStatus.cs` | C# classes that become database tables; Data Annotations add validation rules |
| 3 Database | `Data/ApplicationDbContext.cs`, `DbInitializer.cs` | Connects EF Core to SQLite, defines the relationship and unique index, adds sample data |
| 4 Services | `Services/*.cs` | All database logic lives here (not in pages) so pages stay small and reusable |
| 5 Layout | `Components/Layout/*`, `wwwroot/css/app.css` | Navbar, footer, session banner and the design system |
| 6 Shared components | `Components/Shared/*` | Reusable UI parts: EventCard, EventEditor, Pagination, toasts, spinner |
| 7 Pages | `Components/Pages/*` | One file per route (URL) |

## How a page works (Events page example)
1. `@page "/events"` makes the file respond to the URL `/events`.
2. `@inject IEventService EventService` asks Dependency Injection for the service.
3. `OnInitializedAsync` loads data when the page opens.
4. `@bind` connects form inputs to C# fields; the UI re-renders when they change.
5. `<EventCard EventItem="ev" />` uses the reusable component for every event.

## How registration protects your data
`RegistrationService.RegisterAsync` runs inside a database transaction and checks, in order: event exists → event not over → email not already registered → seat available. Only then does it save the registration and reduce `AvailableSeats`. A unique database index is a second safety net.
