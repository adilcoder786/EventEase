# Final Assignment Submission Checklist (30 marks)

## 1. GitHub repository - 5 marks
- [ ] Repository created and public
- [ ] Code pushed (open the URL in incognito to confirm)
- [ ] README.md displays correctly
- [ ] No `bin/`, `obj/`, `.db` files or secrets committed

## 2. Event Card component with two-way binding - 5 marks
- [ ] `Components/Shared/EventCard.razor` exists and is used on `/` and `/events`
- [ ] Shows title, description, date/time, location, category, available seats
- [ ] *View Details* and *Register Now* buttons work
- [ ] "Save for later" checkbox (`@bind` + `IsSavedChanged`) updates the navbar counter
- [ ] Screenshot: events page with cards

## 3. Routing implemented and debugged - 5 marks
- [ ] `/`, `/events`, `/events/{id}`, `/create-event`, `/register/{eventId}`, `/attendance`, `/dashboard` all load
- [ ] Direct URL + browser refresh (F5) works on each route
- [ ] `/events/9999` shows "Event not found"; `/anything` shows the 404 page
- [ ] Screenshot: 404 page and a refreshed deep link

## 4. Performance, input validation and routing error fixes - 5 marks
- [ ] Async methods, `AsNoTracking`, paging and debounce (see README / Copilot summary)
- [ ] Validation errors show on all forms (event form and registration form)
- [ ] Duplicate registration blocked; full event blocked
- [ ] Screenshot: validation messages

## 5. Registration validation, session state, attendance tracker - 5 marks
- [ ] Registration form uses `EditForm`, `DataAnnotationsValidator`, `ValidationSummary`, `ValidationMessage`
- [ ] Success and error messages appear
- [ ] Session state pre-fills the form on the second registration
- [ ] Attendance page filters by event, marks Present/Absent, shows totals and percentage
- [ ] Screenshots: registration success + attendance page

## 6. Copilot assistance summary - 5 marks
- [ ] `docs/COPILOT_ASSISTANCE_SUMMARY.md` edited so every `[EDIT]` line describes what you **really** did
- [ ] Stages you did not use Copilot for are removed or marked honestly
- [ ] File committed to GitHub (and/or submitted as required)

## Screenshots checklist
- [ ] Home  - [ ] Events list  - [ ] Search/filter result  - [ ] Event details
- [ ] Create event form  - [ ] Registration (errors)  - [ ] Registration (success)
- [ ] My registrations  - [ ] Attendance  - [ ] Dashboard  - [ ] Mobile view  - [ ] 404 page

## Quick test script (10 minutes)
1. `dotnet run` → open `/`
2. Search "blazor" on `/events`; filter by category; toggle "Upcoming only"
3. Open an event → Register → submit empty (see errors) → fill valid data → success
4. Register the same email again → duplicate error
5. Open `/my-registrations` → cancel → seat count increases
6. Open `/attendance` → mark Present/Absent → percentage changes
7. Open `/dashboard` → numbers match
8. Create → Edit → Delete an event
9. Refresh (F5) on every page
