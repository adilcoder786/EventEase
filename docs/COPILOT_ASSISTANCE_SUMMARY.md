# Microsoft Copilot Assistance Summary (5 marks)

> ## ⚠️ Read this first - keep it honest
> This document is a **template with draft wording**. The project code in this repository was generated and
> structured with an AI assistant (Claude), **not** necessarily with Microsoft Copilot.
> Graders expect an honest description of how *you* used Copilot. So:
> 1. Open the project in **Visual Studio / VS Code with GitHub Copilot or Microsoft Copilot**.
> 2. For each stage below, actually run the suggested prompt, read the answer, and compare it with the code.
> 3. Then edit the **"What I did with Copilot"** lines so they describe what really happened
>    (what you asked, what it suggested, what you accepted, changed or rejected).
> 4. Delete any stage where you did not use Copilot rather than claiming that you did.

Each stage lists: **what was built**, a **prompt you can try**, and **what to check**.

---

## 1. Creating the foundational Event Card component
- **What was built:** `Components/Shared/EventCard.razor` shows title, description, date and time, location, category, available seats, *View Details* and *Register Now* buttons, a seats progress bar, and a "Save for later" checkbox.
- **Prompt to try:** *"Create a reusable Blazor component EventCard that takes an Event parameter and displays name, description, date, time, location, category and available seats, with View Details and Register Now buttons."*
- **What to check:** `[Parameter, EditorRequired]` on the event, null safety, disabled "Sold Out"/"Event Ended" state.
- **What I did with Copilot:** `[EDIT: describe your real usage]`

## 2. Implementing two-way data binding
- **What was built:** The card's checkbox uses `@bind="_saved"` with `@bind:after`; the `IsSaved` + `IsSavedChanged` parameters give parent↔child two-way binding. The Events page uses `@bind` for the search box, category select and "Upcoming only" checkbox.
- **Prompt to try:** *"Explain how to make a Blazor child component support two-way binding using a Value parameter and ValueChanged EventCallback, and show @bind:after."*
- **What to check:** Toggling the checkbox updates the "♥ saved" counter in the navbar.
- **What I did with Copilot:** `[EDIT]`

## 3. Creating and debugging routing
- **What was built:** Routes `/`, `/events`, `/events/{id:int}`, `/create-event`, `/register/{eventId:int?}`, `/attendance`, `/dashboard` (+ `/edit-event/{id}`, `/my-registrations`), a custom `NotFound` page in `Routes.razor`, and a not-found state when an event id does not exist.
- **Prompt to try:** *"My Blazor route /events/{id} shows a blank page when I refresh. What are common causes (route constraints, base href, prerendering)?"*
- **What to check:** Type each URL directly in the browser and press F5; try `/events/9999` and `/nonsense`.
- **What I did with Copilot:** `[EDIT]`

## 4. Optimizing application performance
- **What was built:** `IDbContextFactory` with short-lived contexts, `AsNoTracking()` for read queries, server-side paging, search debounce (300 ms), unique index and indexes on `EventDate`/`Category`, prerender disabled to avoid duplicate data loading, async methods everywhere.
- **Prompt to try:** *"Review EventService.GetEventsAsync for performance problems and suggest improvements for a Blazor Server app using EF Core and SQLite."*
- **What to check:** Run with `Microsoft.EntityFrameworkCore.Database.Command` logging at Information to count queries per page.
- **What I did with Copilot:** `[EDIT]`

## 5. Implementing registration form validation
- **What was built:** `Register.razor` with `EditForm`, `DataAnnotationsValidator`, `ValidationSummary`, `ValidationMessage`; required, email and phone-regex rules in `Registration.cs`; duplicate and capacity checks inside a transaction in `RegistrationService`; success and error messages.
- **Prompt to try:** *"Add validation to this Registration model for required name, valid email and a phone number, and show how to display errors in a Blazor EditForm."*
- **What to check:** Submit empty form, invalid email, invalid phone, same email twice, and a full event.
- **What I did with Copilot:** `[EDIT]`

## 6. Implementing state management
- **What was built:** Scoped `UserSessionState` (profile, saved events, registered events, inactivity timeout, `OnChange` event) and `NotificationService`; `MainLayout` runs a timer that expires the session and shows a banner.
- **Prompt to try:** *"How should I share state between Blazor Server components using a scoped service and an OnChange event? How do I avoid memory leaks?"*
- **What to check:** Components unsubscribe in `Dispose()`; set `Session:TimeoutMinutes` to `1` in `appsettings.json` to test expiry.
- **What I did with Copilot:** `[EDIT]`

## 7. Creating the attendance tracker
- **What was built:** `Attendance.razor` with event filter, Present/Absent/Reset buttons, totals, attendance percentage and paging; `RegistrationService.SetAttendanceAsync` and `GetAttendanceSummaryAsync`.
- **Prompt to try:** *"Create a Blazor page that lists registrations, lets me mark each Present or Absent, and calculates the attendance percentage."*
- **What to check:** Percentage = Present ÷ all registered participants; numbers update after each click.
- **What I did with Copilot:** `[EDIT]`

## 8. Debugging errors and improving code quality
- **What was built:** try/catch with logging in services, user-friendly error messages, null handling, `ServiceResult` pattern, separate `Services`/`Data`/`Models` folders, comments for beginners.
- **Prompt to try:** *"Here is my compiler/runtime error: [paste]. Explain the cause in simple words and show the fix."*
- **What I did with Copilot:** `[EDIT: list real errors you fixed, e.g. package version mismatch, wrong route constraint, etc.]`
