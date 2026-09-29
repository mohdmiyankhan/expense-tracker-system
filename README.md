# 💰 Expense Tracker System

A modern **Expense Tracker System** designed to help users manage their personal income and expenses efficiently. The application provides a structured way to record transactions, categorize expenses, monitor financial activity, and understand spending patterns.

---

## 📌 Project Overview

The **Expense Tracker System** is a web-based application developed to simplify personal expense management.

The system is designed with a scalable architecture so that additional financial management features can be added in the future.

### 🎯 Main Objectives

* Track daily expenses and income
* Categorize financial transactions
* Monitor spending patterns
* Maintain transaction history
* Provide a clean and user-friendly interface
* Build a scalable foundation for future financial features

---

## 🚀 Features

### 👤 User Management

* User registration
* User login
* Authentication and authorization
* Secure user-specific data access

### 💸 Expense Management

* Add expenses
* Update expenses
* Delete expenses
* View expense history
* Categorize expenses
* Filter and search transactions

### 💰 Income Management

* Add income
* Update income
* Delete income
* View income history

### 📊 Dashboard

* Total income
* Total expenses
* Current balance
* Expense summary
* Category-wise expense information

### 🔐 Security

* Authentication and authorization
* JWT-based authentication
* Protected APIs
* User-specific data access
* Configuration-based secret management

---

## 🏗️ Technology Stack

### Backend

* **C#**
* **ASP.NET Core**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **LINQ**
* **JWT Authentication**

### Frontend

* **Angular**
* **TypeScript**
* **HTML5**
* **CSS3**

### Database

* **SQL Server**

### Development Tools

* **Visual Studio**
* **Visual Studio Code**
* **Git**
* **GitHub**

---

## 🏛️ High-Level Architecture

```text
┌──────────────────────────┐
│        Angular UI        │
│      Web Application     │
└────────────┬─────────────┘
             │
             │ HTTP / REST API
             ▼
┌──────────────────────────┐
│     ASP.NET Core API     │
│                          │
│ Controllers              │
│ Services                 │
│ Business Logic           │
│ Authentication           │
└────────────┬─────────────┘
             │
             │ Entity Framework Core
             ▼
┌──────────────────────────┐
│        SQL Server        │
│                          │
│ Users                    │
│ Income                   │
│ Expenses                 │
│ Categories               │
└──────────────────────────┘
```

---

## 📁 Project Structure

The project follows a separation between frontend and backend components.

```text
ExpenseTrackerSystem/
│
├── Backend/
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   ├── DTOs/
│   ├── Data/
│   └── ...
│
├── Frontend/
│   ├── src/
│   ├── components/
│   ├── services/
│   └── ...
│
├── .gitignore
└── README.md
```

> The exact folder structure may evolve as the project grows.

---

## ⚙️ Prerequisites

Before running the project, make sure the following are installed:

* .NET SDK
* Node.js
* Angular CLI
* SQL Server
* Git
* Visual Studio or Visual Studio Code

---

## 🔧 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/mohdmiyankhan/expense-tracker-system.git
```

Navigate to the project:

```bash
cd expense-tracker-system
```

---

## 🔹 Backend Setup

Navigate to the backend project:

```bash
cd Backend
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the API:

```bash
dotnet run
```

The API URL will be displayed in the terminal when the application starts.

---

## 🔹 Database Setup

Configure your SQL Server connection string using your local configuration or environment variables.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  }
}
```

> Do not commit real database passwords, API keys, JWT secrets, or other sensitive credentials to GitHub.

If Entity Framework migrations are configured, run:

```bash
dotnet ef database update
```

---

## 🔹 Frontend Setup

Navigate to the frontend project:

```bash
cd Frontend
```

Install dependencies:

```bash
npm install
```

Start the Angular application:

```bash
ng serve
```

The application will normally be available at:

```text
http://localhost:4200
```

---

## 🔐 Configuration & Secrets

Sensitive configuration should **never** be committed to the repository.

Examples of sensitive information:

```text
Database passwords
JWT secrets
API keys
AWS credentials
SMTP credentials
Production connection strings
```

Use environment variables, local configuration files, GitHub Secrets, or an appropriate secret-management solution instead.

---

## 🌿 Git Branching Strategy

This project follows a controlled branch workflow.

```text
feature/login
      │
      ▼
development
      │
      ▼
main
```

### Developer Workflow

Developers should create a feature branch from `development`:

```bash
git checkout development
git pull origin development

git checkout -b feature/login
```

After completing the feature:

```bash
git add .
git commit -m "Implement login feature"

git push -u origin feature/login
```

Then create a Pull Request:

```text
feature/login → development
```

After review and approval, the feature is merged into `development`.

---

## 🔒 Main Branch Protection

The `main` branch is treated as the protected/release branch.

### Main branch rules

* Direct developer pushes are not allowed
* Pull Request is required
* Review approval is required
* Stale approvals are dismissed when new commits are pushed
* Latest reviewable push requires approval
* Conversations must be resolved before merging
* Force pushes are blocked
* Branch deletion is restricted
* Only authorized administrators can bypass the protection rules

### Release Flow

```text
Developer
    │
    ▼
feature/*
    │
    │ Pull Request
    ▼
development
    │
    │ Admin Review
    │
    ▼
development
    │
    │ Pull Request
    ▼
main
    │
    │ Admin Approval
    ▼
main
```

This workflow helps keep the `main` branch stable and controlled.

---

## 🧪 Testing

Before creating a Pull Request, developers should verify:

```bash
dotnet build
```

and run available tests:

```bash
dotnet test
```

For Angular:

```bash
npm install
ng build
```

Additional automated CI/CD checks can be added as the project evolves.

---

## 📊 Planned Features

The following features can be added in future versions:

* [ ] Advanced dashboard
* [ ] Monthly expense reports
* [ ] Category-wise charts
* [ ] Budget management
* [ ] Recurring expenses
* [ ] Export transactions to Excel/PDF
* [ ] Email notifications
* [ ] Multi-currency support
* [ ] Advanced search and filtering
* [ ] Financial analytics
* [ ] Mobile-friendly improvements
* [ ] Automated CI/CD pipeline
* [ ] Automated unit and integration testing

---

## 🤝 Contributing

Contributions are welcome.

### Contribution Process

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run the required tests/build
5. Push your branch
6. Create a Pull Request
7. Wait for review

Example:

```bash
git checkout development

git checkout -b feature/your-feature

git add .

git commit -m "Add your feature"

git push -u origin feature/your-feature
```

---

## 🐛 Bug Reports

If you find a bug, please create an issue with:

* Clear description of the problem
* Steps to reproduce
* Expected behavior
* Actual behavior
* Relevant screenshots/logs
* Environment details

---

## 📄 License

License information will be added as the project is prepared for public/open-source distribution.

---

## 👨‍💻 Author

**Mohd Miyan Khan**

GitHub:

https://github.com/mohdmiyankhan

---

## ⭐ Support

If you find this project useful, consider giving the repository a ⭐ on GitHub.

---

## 📌 Project Status

**Status:** 🚧 Active Development

The project is currently under development, and features may change as new functionality is added.
