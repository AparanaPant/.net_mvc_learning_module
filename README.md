# GRACE – Learning Modules & Quiz Platform (ASP.NET Core)

An ASP.NET Core MVC application that manages **learning modules, slides, quizzes, students, and reports** on top of **ASP.NET Identity** and **Entity Framework Core**. It includes admin pages to curate modules/slides, a student portal to consume content and take quizzes, and reporting for outcomes.

---

## Key Features

- **Authentication & Authorization**  
  Built with **ASP.NET Core Identity** (`ApplicationUser`, Identity tables) with environment‑based sign‑in confirmation logic.

- **Learning Content Management**  
  Admin controllers and views for **Modules** and **Slides** (including `SlideSection`), plus asset handling within `wwwroot`.

- **Quiz Engine**  
  Entities for `Quiz`, `Question`, `Option`, and `UserQuiz` with student‑facing flows (`Controllers/Student`, `Controllers/QuizController`).

- **Student & Educator Models**  
  Domain entities for `Student`, `Educator`, `Course`, `Enrollment`, `School`, `SchoolInfo` to support an LMS‑style workflow.

- **Reporting**  
  `ReportController` for outcome and activity reporting.

- **Logging**  
  **Serilog** configured for console + rolling file logs at `AppLogs/myapplog-*.txt`.

- **UI**  
  Razor Views with **Bootstrap 5** styling, **jQuery 3.7**, and **DataTables 2.x**. Static assets live under `wwwroot/` (no Node build pipeline required).

---

## Tech Stack (as implemented)

- **Runtime/Framework:** .NET **7.0** (target framework: `net7.0`)
- **Web:** ASP.NET Core MVC, Razor Pages (Identity area)
- **Auth:** ASP.NET Core **Identity**
- **ORM:** **Entity Framework Core** (packages for **SQL Server** and **SQLite** are referenced)
- **Database:** SQL Server by default (`UseSqlServer(...)`); SQLite package is present but not wired by default
- **Logging:** **Serilog** (`Serilog.AspNetCore`, `Serilog.Sinks.File`)
- **Frontend:** **Bootstrap 5.1**, **jQuery 3.7**, **DataTables 2.1**
- **Tooling:** EF Core Tools, dotnet CLI



## Getting Started

### Prerequisites

- **.NET SDK 7.0** (works as‑is)  
- **SQL Server** (Developer/LocalDB/Express) for default configuration  

### 1) Clone 

### 2) Configure Connection String

Open **`appsettings.json`** and set **`ConnectionStrings:AuthDbContextConnection`** to your SQL Server instance, for example:

```jsonc
{
  "ConnectionStrings": {
    "AuthDbContextConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GraceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  },
  "AppEnvironment": "Development"
}
```

> The app reads `AppEnvironment` to decide whether to require confirmed accounts at sign‑in (production requires confirmation).

### 3) Apply Migrations

Install EF tools if needed and update the database:

```bash
dotnet tool install --global dotnet-ef   # once
dotnet ef database update --project GraceProject
```

### 4) Run the App

```bash
dotnet run --project GraceProject
```

By default, the development profile serves at **http://localhost:5206** (see `Properties/launchSettings.json`).
