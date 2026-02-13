# 📚 Library Management System

![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blue)
![Tests](https://img.shields.io/badge/tests-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-green)

A modern, full-stack demo application that replicates real-world library operations using enterprise-grade architecture and tools. Built with scalability, maintainability, and automation in mind, this system showcases best practices in software engineering—from clean code and layered architecture to background processing and secure role-based access.

---

## 🧰 Technologies Used

| Category        | Tools & Frameworks                         |
|-----------------|--------------------------------------------|
| Backend         | .NET Core MVC                              |
| Frontend        | jQuery, HTML/CSS, Bootstrap                |
| Database        | SQL Server                                 |
| Background Jobs | Hangfire                                   |
| Logging         | Serilog                                    |
| Architecture    | Onion Architecture                         |
| Authentication  | JWT Token-based Authentication             |
| Testing         | xUnit, FluentAssertions, Moq               |

---

## 🚀 Key Features

- **Role-Based Access Control**  
  Secure login system with distinct dashboards for Admin, Librarian, and User roles.

- **Membership & Book Management**  
  Dynamic handling of book inventory, genres, and membership plans with configurable limits.

- **Reservation & Borrowing Workflow**  
  Priority-based reservation system, automated allocation, and renewal logic.

- **Penalty Calculation**  
  Smart logic for overdue, lost books, and expired memberships with configurable rules.

- **Audit Logging**  
  Entity-level logging and system-wide exception tracking using Serilog.

- **Background Processing**  
  Automated jobs for reminders, cleanups, and email dispatch using Hangfire.

- **Responsive UI**  
  Role-specific interfaces optimized for usability and clarity.
  
- **Comprehensive Testing**  
  Unit and integration tests demonstrating TDD practices and quality assurance.

---

## 👥 Role Capabilities

| Role          | Capabilities                                                                   |
|---------------|--------------------------------------------------------------------------------|
| **Admin**     | Full access, view logs/history, manage jobs, override passwords                |
| **Librarian** | Manage books, reservations, penalties; limited access to logs                  |
| **User**      | View personal data, reserve/borrow books, pay penalties, download library card |

---

## 🧱 Architectural Highlights

- **Onion Architecture**  
  Separation of concerns across layers:
  - **Core:** Domain entities and interfaces.
  - **Infrastructure:** Database access, external services.
  - **Application.Contracts:** DTOs and service interfaces.
  - **Application.Service:** Business logic implementations.
  - **Common:** Shared utilities and helpers.
  - **Bootstrapper:** Dependency injection and app configuration.
  - **Presentation:** ASP.NET Core MVC UI layer.
  - **Test:** Unit and integration tests to verify the behavior and interaction of all layers.

- **Dependency Injection**  
  Loosely coupled components for better testability and scalability.

- **Configuration-Driven Logic**  
  System behavior controlled via centralized config values for flexibility.

- **Secure Workflows**  
  Password updates require old password validation; sensitive actions require elevated access.

---

## 🧪 Testing Strategy

This project demonstrates professional testing practices with both **unit tests** and **integration tests**, following the testing pyramid principle.

### **Test Structure**
```
LMS.Tests/
├── Unit/
│   ├── Services/          # Business logic unit tests with mocked dependencies
│   ├── Validators/        # Input validation tests
├── Integration/
│   ├── Base/
│   │   └── IntegrationTestBase.cs    # Base class for test setup/teardown
│   └── Services/
│       └── PenaltyCalculationIntegrationTests.cs  # End-to-end service tests
└── Helpers/
    ├── TestDataBuilder.cs        # Test data creation patterns
    ├── MockHelper.cs             # Mock object factory
    └── TestLibraryManagementSysContext.cs  # Test-specific DbContext
```

### **Testing Highlights**

- ✅ **Unit Tests:** Fast, isolated tests for business logic with mocked dependencies
- ✅ **Integration Tests:** Real database tests using SQL Server LocalDB
- ✅ **Test Isolation:** Each integration test runs against a unique database instance
- ✅ **Test Data Builders:** Readable, maintainable test data creation
- ✅ **Custom Test Context:** Conditional identity handling for reference vs transactional data
- ✅ **Proper Mocking:** Strategic use of mocks vs real implementations

### **Running Tests**
```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test --filter "FullyQualifiedName~Unit"

# Run only integration tests
dotnet test --filter "FullyQualifiedName~Integration"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### **Integration Test Setup**

Integration tests use **SQL Server LocalDB** (included with Visual Studio) to provide a realistic testing environment:

1. **Automatic Database Creation:** Each test creates a unique database
2. **Reference Data Seeding:** Statuses, roles, and other lookup data seeded automatically
3. **Test Isolation:** Database is deleted after each test
4. **No Manual Setup Required:** LocalDB handles everything automatically

**Requirements:**
- SQL Server LocalDB (included with Visual Studio)
- Or SQL Server Express with LocalDB feature enabled

**Verify LocalDB Installation:**

```bash
sqllocaldb info
```

## 📦 Methodology

- **Clean Code Principles**  
  Modular, readable, and maintainable codebase following SOLID principles.

- **Test-Driven Approach**  
  Critical business logic validated through comprehensive unit and integration tests.

- **Trigger-Based Automation**  
  SQL triggers ensure real-time updates and consistency across workflows.

- **Outbox Pattern for Emails**  
  Reliable email dispatch using queued messages processed by background jobs.

- **Server-Side Pagination & Filtering**  
  Efficient data handling for large datasets across all roles.

---

## 🛠 Setup Overview

### **Prerequisites**
- .NET 8.0 SDK or later
- SQL Server (or SQL Server LocalDB for testing)
- Visual Studio 2022 or VS Code with C# extension

### **Installation Steps**

1. **Clone the repository**
```bash
   git clone https://github.com/DhruviKhanpara/LMS.git
   cd LMS
```

2. **Configure database connection**
   
   Update `appsettings.json` with your SQL Server connection string and JWT secret key:
```json
   {
     "ConnectionStrings": {
       "LibraryManagementSysConnection": "Server=.;Database=LibraryDB;Trusted_Connection=true;"
     },
     "AppSettings": {
        "SecretKey": "askhYU/.sg/sdmghskdfnjh/z.d,/s';d,.lf'ka'"
     }
   }
```

   Update `appsettings.Development.json` with your email adderess if the 'SendToActualRecipients' is false else make it true
```json
   {
     "SmtpConfigurations": {
       "SendToActualRecipients": false,
       "OverrideRecipients": "xyz@test.com"
     },
   }
```

3. **Run database migrations**
```bash
   dotnet ef database update
```

# This will create tables, constraints, and indexes only.
# Triggers, functions, and seed data are NOT created by EF.

```bash
    # Or run the provided SQL scripts in /Database folder
        
    # Step 1: Initial setup (DBA only)
    1. Run `00_create_database_and_users.sql`
    
    # Step 2: Schema creation (Application DB user)
    2. Connect using application DB user
    3. Run scripts in order:
       - 01_schema.sql
```

```bash
    # Step 3: Database logic and configuration (Application DB user)

    1. Connect using application DB user
    2. Run scripts in order:
       - 02_functions.sql
       - 03_triggers.sql
       - 04_seed_config_data.sql
```

4. **Build the solution**
```bash
   dotnet build
```

5. **Run the application**
```bash
   dotnet run --project LMS.Presentation
```

6. **Access the application**
   - Main app: `https://localhost:5001`
   - Hangfire dashboard: `https://localhost:5001/hangfire`

### **Running Tests**

```bash
# Restore packages
dotnet restore

# Run all tests
dotnet test

# Run tests with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```


### **Default Credentials**

| Role      | Username | Password          |
|-----------|----------|-------------------|
| Admin     | admin    | Admin@123         |
| Librarian | librarian| Librarian@123     |
| User      | user     | User@123          |

---

## 🎯 Learning Outcomes

This project demonstrates:
- ✅ Enterprise-level architecture and design patterns
- ✅ Test-driven development with comprehensive test suite
- ✅ Background job processing and scheduling
- ✅ Complex business logic implementation (penalty calculations, reservations)
- ✅ Secure authentication and authorization
- ✅ Clean code principles and SOLID adherence
- ✅ Database design with normalization and triggers
- ✅ Logging and audit trail implementation

---

## 📄 License

This project is open-source and available under the MIT License.

---

## 🙌 Credits

Crafted to demonstrate real-world software engineering practices in a library domain. Ideal for portfolios, interviews, and educational use.

---

## 📧 Contact

For questions or feedback about this project, feel free to reach out or open an issue.

---

**⭐ If you find this project helpful, please consider giving it a star!**