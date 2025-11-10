# 📚 Library Management System  
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

- **Dependency Injection**  
  Loosely coupled components for better testability and scalability.

- **Configuration-Driven Logic**  
  System behavior controlled via centralized config values for flexibility.

- **Secure Workflows**  
  Password updates require old password validation; sensitive actions require elevated access.

---

## 📦 Methodology

- **Clean Code Principles**  
  Modular, readable, and maintainable codebase following SOLID principles.

- **Trigger-Based Automation**  
  SQL triggers ensure real-time updates and consistency across workflows.

- **Outbox Pattern for Emails**  
  Reliable email dispatch using queued messages processed by background jobs.

- **Server-Side Pagination & Filtering**  
  Efficient data handling for large datasets across all roles.

---

## 🛠 Setup Overview

1. Clone the repository  
2. Configure `appsettings.json` with DB and JWT settings  
3. Run migrations or SQL scripts (In Future will add)
4. Launch the application
5. Access Hangfire dashboard at `/hangfire`

---

## 📄 License

This project is open-source and available under the MIT License.

---

## 🙌 Credits

Crafted to demonstrate real-world software engineering practices in a library domain. Ideal for portfolios, interviews, and educational use.
