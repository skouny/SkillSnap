# SkillSnap

A modern portfolio management application built with .NET 9, featuring a Blazor WebAssembly frontend and ASP.NET Core Web API backend.

## 📋 Overview

SkillSnap is a full-stack application that allows users to create and manage their professional portfolios. Users can showcase their projects, highlight their skills, and maintain a personalized profile with authentication and authorization.

## 🏗️ Architecture

This solution consists of two main projects:

- **SkillSnap.Api** - ASP.NET Core Web API backend with Entity Framework Core and SQLite
- **SkillSnap.Client** - Blazor WebAssembly frontend application

## ✨ Features

- 🔐 **User Authentication & Authorization** - JWT-based authentication with ASP.NET Identity
- 👤 **Portfolio Management** - Create and manage user profiles with bio and profile images
- 📁 **Project Showcase** - Add, edit, and display projects with descriptions and images
- 🎯 **Skills Management** - Track and display skills with proficiency levels
- 💾 **Data Persistence** - SQLite database with Entity Framework Core
- 🎨 **Modern UI** - Responsive Blazor components with Bootstrap styling
- 🔄 **RESTful API** - Well-structured API endpoints for all operations

## 🛠️ Technology Stack

### Backend (SkillSnap.Api)

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9.0
- SQLite Database
- ASP.NET Core Identity
- JWT Bearer Authentication
- OpenAPI/Swagger

### Frontend (SkillSnap.Client)

- Blazor WebAssembly
- .NET 9.0
- Bootstrap 5
- HTTP Client for API communication

## 📦 Project Structure

```txt
SkillSnap/
├── SkillSnap.Api/              # Backend API
│   ├── Controllers/            # API Controllers
│   │   ├── AuthController.cs
│   │   ├── PortfolioUsersController.cs
│   │   ├── ProjectsController.cs
│   │   ├── SkillsController.cs
│   │   └── SeedController.cs
│   ├── Models/                 # Data Models
│   │   ├── ApplicationUser.cs
│   │   ├── PortfolioUser.cs
│   │   ├── Project.cs
│   │   └── Skill.cs
│   ├── Migrations/             # EF Core Migrations
│   └── SkillSnapContext.cs     # Database Context
│
└── SkillSnap.Client/           # Frontend Client
    ├── Components/             # Reusable Components
    │   ├── ProfileCard.razor
    │   ├── ProjectList.razor
    │   └── SkillTags.razor
    ├── Pages/                  # Page Components
    │   ├── Home.razor
    │   ├── Login.razor
    │   ├── Register.razor
    │   ├── Demo.razor
    │   └── Test.razor
    ├── Services/               # API Services
    │   ├── AuthService.cs
    │   ├── PortfolioUserService.cs
    │   ├── ProjectService.cs
    │   ├── SkillService.cs
    │   └── UserSessionService.cs
    └── Layout/                 # Layout Components
        ├── MainLayout.razor
        └── NavMenu.razor
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A code editor (Visual Studio, VS Code, or Rider)

### Installation

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd SkillSnap
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Update the database** (from the SkillSnap.Api directory)

   ```bash
   cd SkillSnap.Api
   dotnet ef database update
   ```

### Running the Application

You need to run both the API and the Client simultaneously:

#### Option 1: Using Visual Studio

1. Set multiple startup projects
2. Right-click the solution → Properties → Multiple Startup Projects
3. Set both SkillSnap.Api and SkillSnap.Client to "Start"
4. Press F5

#### Option 2: Using Command Line

**Terminal 1 - Start the API:**

```bash
cd SkillSnap.Api
dotnet run
```

The API will start at `http://localhost:5064`

**Terminal 2 - Start the Client:**

```bash
cd SkillSnap.Client
dotnet run
```

The client will start at `http://localhost:5249`

## 🔧 Configuration

### API Configuration (appsettings.json)

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=skillsnap.db"
  }
}
```

### Client Configuration

The client is configured to connect to the API at `http://localhost:5064`. Update this in `SkillSnap.Client/Program.cs` if needed:

```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5064") });
```

## 📚 API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

### Portfolio Users

- `GET /api/portfoliousers` - Get all portfolio users
- `GET /api/portfoliousers/{id}` - Get specific portfolio user
- `POST /api/portfoliousers` - Create portfolio user
- `PUT /api/portfoliousers/{id}` - Update portfolio user
- `DELETE /api/portfoliousers/{id}` - Delete portfolio user

### Projects

- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get specific project
- `POST /api/projects` - Create project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

### Skills

- `GET /api/skills` - Get all skills
- `GET /api/skills/{id}` - Get specific skill
- `POST /api/skills` - Create skill
- `PUT /api/skills/{id}` - Update skill
- `DELETE /api/skills/{id}` - Delete skill

### Seed Data

- `POST /api/seed` - Seed the database with sample data

## 🗄️ Database

The application uses SQLite for data storage with the following entities:

- **ApplicationUser** - Identity user for authentication
- **PortfolioUser** - User portfolio information
- **Project** - User projects with descriptions and images
- **Skill** - User skills with proficiency levels

### Running Migrations

To create a new migration:

```bash
cd SkillSnap.Api
dotnet ef migrations add MigrationName
```

To update the database:

```bash
dotnet ef database update
```

## 🔐 Authentication

The application uses JWT (JSON Web Tokens) for authentication:

1. Users register/login through the Auth endpoints
2. Server returns a JWT token upon successful authentication
3. Client stores the token and includes it in subsequent API requests
4. Token is validated on protected endpoints

## 🎨 Components

### Reusable Components

- **ProfileCard** - Displays user profile information
- **ProjectList** - Renders a list of projects
- **SkillTags** - Shows skill tags with levels

### Services

- **AuthService** - Handles authentication operations
- **UserSessionService** - Manages user session state
- **PortfolioUserService** - CRUD operations for portfolio users
- **ProjectService** - CRUD operations for projects
- **SkillService** - CRUD operations for skills

## 📝 Development Notes

- The API uses CORS policy named "AllowClient" configured for the Blazor client
- JWT tokens use symmetric security keys for signing
- Entity Framework Core uses lazy loading proxies for navigation properties
- JSON serialization is configured to ignore circular references

## 🤝 Contributing

This is a course project. Feel free to fork and experiment with the code!

## 📄 License

This project is for educational purposes.

## 🎓 Learning Activities

Check the `Activities/` folder for guided learning exercises related to this project.

---

Built with ❤️ using .NET 9
