# Budget Planner Application

## Project Overview
The Budget Planner is a comprehensive financial management application designed to help users track their income, expenses, and manage their budget effectively. The application is built using a modern web architecture with separate frontend and backend components.

## Table of Contents
1. [Application Architecture](#application-architecture)
2. [Application Description](#application-description)
3. [User Interface](#user-interface)
4. [Technical Stack](#technical-stack)
5. [Setup and Installation](#setup-and-installation)
6. [Summary](#summary)

## Application Architecture

### Backend (BudgetPlanner.API)
The backend is built using ASP.NET Core Web API and follows a clean architecture pattern:
- **Controllers**: Handle HTTP requests and responses
- **Services**: Implement business logic
- **Models**: Define data structures
- **Data**: Contains database context and configurations
- **Authentication**: JWT-based authentication system

### Frontend (BudgetPlanner.Web)
The frontend is implemented using ASP.NET Core MVC:
- **Views**: Razor views for the user interface
- **Controllers**: Handle user interactions
- **Services**: Manage API communication
- **Models**: Define view models and data structures

### Database
- SQLite database for data persistence
- Entity Framework Core for ORM
- Secure password hashing using BCrypt

## Application Description

The Budget Planner application provides the following key features:

1. **User Management**
   - Secure user registration and authentication
   - JWT-based session management
   - User profile management

2. **Budget Management**
   - Income tracking
   - Expense categorization
   - Budget planning and monitoring
   - Financial reports and analytics

3. **Security Features**
   - Secure password hashing
   - JWT token-based authentication
   - HTTPS enforcement
   - CORS policy implementation

## User Interface

The application features a modern, responsive user interface with:

1. **Dashboard**
   - Overview of financial status
   - Quick access to key features
   - Visual representation of budget data

2. **Navigation**
   - Intuitive menu structure
   - Easy access to all features
   - Responsive design for all devices

3. **Data Visualization**
   - Charts and graphs for financial data
   - Interactive reports
   - Clear data presentation

## Technical Stack

### Backend Technologies
- ASP.NET Core 9.0
- Entity Framework Core
- SQLite Database
- JWT Authentication
- Swagger/OpenAPI

### Frontend Technologies
- ASP.NET Core MVC
- HTML5/CSS3
- JavaScript
- Bootstrap for responsive design

### Development Tools
- Visual Studio 2022
- Git for version control
- Swagger for API documentation

## Setup and Installation

1. **Prerequisites**
   - .NET 9.0 SDK
   - Visual Studio 2022 or later
   - Git

2. **Backend Setup**
   ```bash
   cd BudgetPlanner.API
   dotnet restore
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   cd BudgetPlanner.Web
   dotnet restore
   dotnet run
   ```

4. **Database**
   - The application will automatically create and initialize the database on first run
   - SQLite database file is created in the API project directory

## Summary

The Budget Planner application is a modern, secure, and user-friendly financial management solution. It provides comprehensive tools for budget tracking, expense management, and financial planning. The application's architecture ensures scalability, maintainability, and security, while the user interface offers an intuitive experience for managing personal finances.

The project demonstrates the implementation of modern web development practices, including:
- Clean architecture principles
- Secure authentication and authorization
- Responsive design
- API-first development approach
- Database management and optimization

This application serves as a practical solution for personal finance management while showcasing professional software development practices and modern web technologies.
