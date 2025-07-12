# Bookstore Web Application

A modern web application for managing a bookstore built with ASP.NET Core MVC, featuring a robust authentication system, shopping cart functionality, and comprehensive product management.

## Features

### 🛍️ Customer Features
- User registration and authentication
- Shopping cart functionality 
- Multiple price tiers based on quantity (1-50, 50+, 100+)
- Secure checkout process
- Order history and tracking
- External authentication providers support

### 👨‍💼 Admin Features
- Complete product management (CRUD operations)
- Category management
- Image upload and management
- User role management (Admin, Customer, Company, Employee)
- Company account management
- Order processing and status updates

### 🔒 Security
- ASP.NET Core Identity integration
- Role-based authorization
- Secure password policies
- External authentication providers

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Architecture**: N-Tier Architecture
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: 
  - Bootstrap 5
  - jQuery
  - jQuery Validation
- **File Storage**: Local file system for product images

## Project Structure
Bookstore/
├── Bookstore.Business/      # Business logic layer
├── Bookstore.Common/        # Shared components (e.g., utilities, constants)
├── Bookstore.DataAccess/    # Data access layer (Entity Framework, repositories)
└── BulkyWeb/                # Presentation layer (MVC UI)


## Dependencies

- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.Extensions.Configuration
- jQuery Validation
- Bootstrap

## Getting Started

1. **Prerequisites**
   - .NET 9.0 SDK
   - SQL Server
   - Visual Studio 2022 or later

2. **Database Setup**
 Update-Database


3. **Running the Application**
- Open the solution in Visual Studio
- Restore NuGet packages
- Build and run the application


## Features in Detail

### Product Management
- Product creation with image upload
- Price tier management
- Category association
- Stock management

### Order Processing
- Shopping cart management
- Order status tracking
- Payment integration
- Shipping details

### User Management
- Role-based access control
- Company account management
- User profile management
- Address management

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

