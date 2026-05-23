# Smart Photo Journal

A personal journal web application built with ASP.NET Core MVC (.NET 8) that lets users write daily entries, attach multiple photos, and organize everything into folders — all behind a secure, session-based login system.

---

## Features

- **Daily Journal Entries** — Create, view, edit, and delete journal entries tied to a specific date. Future-dated entries are blocked.
- **Multiple Photo Attachments** — Each entry requires at least one photo. Images are stored in the database as binary data (max 5 MB per image).
- **Calendar View** — Navigate entries by year, month, and day through an interactive calendar interface.
- **Folder Organization** — Create custom folders (with emoji labels) and assign entries to one or more folders.
- **Full-Text Search** — Search across all journal entries by keyword.
- **User Authentication** — Cookie-based authentication with a 7-day "remember me" option and a 30-minute session timeout.
- **XSS Protection** — Entry content is sanitized with `HtmlSanitizer` before being saved.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Database | SQL Server (via Entity Framework Core 8) |
| ORM | Entity Framework Core with Migrations |
| Authentication | Cookie Authentication (`CookieAuth`) |
| Frontend | Razor Views, Bootstrap 5, jQuery |
| Security | HtmlSanitizer (XSS protection) |

---

## Project Structure

```
deneme2.0/
├── Controllers/
│   ├── AccountController.cs    # Login, Register, Logout
│   ├── JournalController.cs    # CRUD for journal entries, search, calendar
│   ├── FolderController.cs     # Folder management
│   └── HomeController.cs       # Landing page
├── Models/
│   ├── JournalEntry.cs         # Core journal entry model
│   ├── JournalImage.cs         # Binary image storage
│   ├── Folder.cs               # Folder model
│   ├── JournalEntryFolder.cs   # Many-to-many join table
│   ├── User.cs                 # Application user
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── CalendarViewModel.cs
├── Services/
│   ├── IJournalService.cs / JournalService.cs
│   ├── IFolderService.cs / FolderService.cs
│   └── IAuthService.cs / AuthService.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Views/
│   ├── Journal/                # Index, Create, Edit, Details, Search, Calendar...
│   ├── Folder/                 # Index, Details, Entries
│   └── Account/                # Login, Register
├── Migrations/
├── wwwroot/                    # Bootstrap, jQuery, site CSS/JS
├── Program.cs
└── appsettings.json
```

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server LocalDB

---

## Getting Started

**1. Clone the repository**

```bash
git clone <your-repo-url>
cd deneme2.0
```

**2. Configure the database connection**

Open `appsettings.json` and update the connection string if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartPhotoJournalDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

**3. Apply database migrations**

```bash
dotnet ef database update
```

**4. Run the application**

```bash
dotnet run --project deneme2.0
```

The app will be available at `https://localhost:7xxx` (check the console output for the exact port).

---

## Authentication

- Users register and log in with a username and password.
- Sessions are cookie-based and last 30 minutes of inactivity, with an optional 7-day persistent cookie.
- All journal and folder routes require authentication; unauthenticated requests are redirected to `/Account/Login`.

---

## Key Constraints

- **Photos are required** — Every journal entry must include at least one image.
- **5 MB image limit** — Each uploaded image must be under 5 MB.
- **No future entries** — The creation date cannot be set to a future date.
- **User isolation** — Each user can only access their own entries and folders.

---

## NuGet Dependencies

| Package | Version |
|---|---|
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.8 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.8 |
| Microsoft.EntityFrameworkCore.Design | 8.0.8 |
| HtmlSanitizer | 9.0.892 |
