# ResumeForge - Online Resume Builder (ASP.NET Core MVC)

**Stack:** ASP.NET Core MVC (.NET 8), C#, Entity Framework Core, SQL Server (LocalDB), Bootstrap 5

## How to run
1. Install the **.NET 8 SDK** and SQL Server LocalDB (it comes with Visual Studio; or install SQL Server Express).
2. Open this folder in Visual Studio (open `ResumeForge.csproj`) or in a terminal.
3. Run: `dotnet run` (or press F5 in Visual Studio).
4. Open the address shown in the terminal, for example https://localhost:5001.

The database `ResumeForgeDb` and its tables are created automatically on first run (`EnsureCreated`).
If you use SQL Server Express, change the connection string in `appsettings.json` to `Server=.\\SQLEXPRESS;Database=ResumeForgeDb;Trusted_Connection=True;TrustServerCertificate=True`.

## Roles
- **The first person to register becomes the Admin.** Everyone after that is a User.
- User: create, edit, preview, delete resumes; choose a template; download as PDF
- Admin: sees total users, total resumes, template usage, and can delete users

## Download as PDF
The preview page has a print stylesheet. Click **Download as PDF** and choose "Save as PDF" in the print window. No extra library is needed.

## Database tables
| Table | Columns |
|---|---|
| Users | Id, Name, Email (unique), PasswordHash, Role |
| Resumes | Id, UserId (FK to Users, cascade delete), Title, FullName, Email, Phone, Address, Summary, Skills, Education, Experience, Projects, Template, CreatedAt |

## Folder structure
```
Controllers/   Account, Home, Resume, Admin
Models/        Models.cs (entities, view models, AppDb context)
Views/         Razor pages for each controller + Shared/_Layout
wwwroot/css/   site.css (theme + resume templates + print styles)
Program.cs     app setup, database, login cookie
```

## Viva points
- MVC pattern: Model (data), View (Razor page), Controller (logic).
- Entity Framework Core is the ORM: C# classes become tables, LINQ becomes SQL.
- Passwords are stored as hashes using ASP.NET's `PasswordHasher`, never as plain text.
- Cookie authentication with roles; `[Authorize]` and `[Authorize(Roles = "Admin")]` protect pages.
- `[ValidateAntiForgeryToken]` blocks CSRF; data annotations validate input; `[Bind]` stops over-posting.
- Every resume query includes `UserId`, so a user can never open someone else's resume.

## Future scope
More templates, profile photo, share link, cover letter builder, import from LinkedIn.
