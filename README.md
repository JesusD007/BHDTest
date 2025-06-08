# User Management API

This is a **RESTful API** designed with a **layered architecture** and **dependency injection** for secure user creation and retrieval. It follows best practices for clean architecture, modularity, and security.
### [How to Run Step-by-Step](https://github.com/JesusD007/BHDTest/wiki/How-to-Run-Step-by-Step)

---

## Quick Start

### Prerequisites

* [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
* [SQL Server](https://www.microsoft.com/en-us/sql-server/)

### Database Setup

Once the required Entity Framework dependencies are installed, use the following commands in the **NuGet Package Manager Console** to set up the database direcly from code :

```bash
Add-Migration InitDB
Update-Database
```

---

## Database Schema

### Users Table

| Field     | Type               | Description                                           |
| --------- | ------------------ | ----------------------------------------------------- |
| Id        | `UNIQUEIDENTIFIER` | Primary key for the user                              |
| Name      | `VARCHAR(100)`     | Full name of the user (required)                      |
| Email     | `VARCHAR(255)`     | User's email address (required, unique)               |
| Password  | `VARCHAR(255)`     | Hashed user password (required)                       |
| Created   | `DATETIME`         | Timestamp of user creation                            |
| Modified  | `DATETIME`         | Timestamp of last modification                        |
| LastLogin | `DATETIME`         | Timestamp of last login                               |
| IsActive  | `BIT`              | Indicates whether the user is active (default is `1`) |

### Phones Table

| Field       | Type               | Description                         |
| ----------- | ------------------ | ----------------------------------- |
| Id          | `INT`              | Primary key, auto-incremented       |
| Number      | `VARCHAR(20)`      | Phone number                        |
| CityCode    | `VARCHAR(10)`      | City code                           |
| CountryCode | `VARCHAR(10)`      | Country code                        |
| UserId      | `UNIQUEIDENTIFIER` | Foreign key referencing `Users(Id)` |

---

## Configuration and Setup

The `appsettings.json` file contains configuration settings. **Sensitive values** such as connection strings and JWT secrets should be **secured via environment variables** or secret managers in production.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "BHDTestConnection": "Your SQL Server connection string here" example  "Server= (server name); Database= (data base name); Trusted_Connection=True; Trust Server Certificate=True"
  },
  "PasswordRules": {
    "Regex": "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W_]).{8,}$" --> Configurable password rules
  },
  "Jwt": {
    "Key": "Secret-key-here" --> configurable JWT key (should be at least 32 char)
  }
}
```

---

## Endpoints

| Method | Endpoint          | Description                                      |
| ------ | ----------------- | ------------------------------------------------ |
| GET    | `/api/User`       | Retrieves all users                              |
| POST   | `/api/User`       | Creates a new user with associated phone numbers |
| GET    | `/api/User/token` | Retrieves a test JWT token (static mode)         |

---
### Swagger UI

![Swagger](https://github.com/JesusD007/BHDTest/blob/62c5d14d6dc0cde3f085fd145682d9504bca3929/Doc/Swagger.jpg?raw=true)
---
## Post (Create User)
### Request Example

![Request](https://github.com/JesusD007/BHDTest/blob/85bf96f37894d540fdf612d727814eb28ea1aa48/Doc/Request.jpg?raw=true)

---

### Response Example

![Response](https://github.com/JesusD007/BHDTest/blob/85bf96f37894d540fdf612d727814eb28ea1aa48/Doc/Response.jpg?raw=true)
---
## Validation Rules

### Email Regex:

```regex
^[^@\s]+@[^@\s]+\.[^@\s]+$
```

### Password Regex:

```regex
^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$
```

### FluentValidation Example:

```csharp
public UserCreateRequestValidator(IConfiguration configuration) {
    _configuration = configuration;

    RuleFor(x => x.Nombre)
        .NotEmpty().WithMessage("El nombre es obligatorio.");

    RuleFor(x => x.Email)
        .NotEmpty().WithMessage("El email es obligatorio.")
        .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("El formato del email no es válido.");

    RuleFor(x => x.Password)
        .NotEmpty()
        .Matches(_configuration["PasswordRules:Regex"])
        .WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial.");
}
```

---

## Dependencies

| Package                                                 | Purpose                                                  |
| ------------------------------------------------------- | -------------------------------------------------------- |
| `FluentValidation (12.0.0)`                             | Input validation using expressive rules                  |
| `Microsoft.AspNetCore.Authentication.JwtBearer (8.0.3)` | Handles JWT-based authentication                         |
| `Microsoft.EntityFrameworkCore.SqlServer (9.0.5)`       | Entity Framework Core integration with SQL Server        |
| `Microsoft.EntityFrameworkCore.Tools (9.0.5)`           | Provides tooling for EF Core commands (migrations, etc.) |
| `Swashbuckle.AspNetCore (8.1.4)`                        | Swagger/OpenAPI documentation generation                 |

---
