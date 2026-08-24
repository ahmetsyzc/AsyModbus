# AsyModbus

AsyModbus is a web-based application developed with C#, ASP.NET Web Forms, ADO.NET, and Microsoft SQL Server.

The project is currently under development. Its main purpose is to improve my practical experience in web development, database operations, reusable UI components, and structured application design.

## 🚀 Current Features

- User management
- User login and authentication
- Role-based user structure
- Session management
- Password reset and password management
- Profile image management
- User input validation
- SQL Server database integration
- Stored procedure-based database operations
- Reusable ASP.NET UserControls
- Reusable user listing grid
- Client-side search and pagination
- Configurable record count
- Responsive Bootstrap-based interface
- Centralized application messages
- Toastr notification system

## 🛠️ Technologies

- C#
- .NET
- ASP.NET Web Forms
- ADO.NET
- Microsoft SQL Server
- HTML
- CSS
- Bootstrap
- JavaScript
- jQuery
- Toastr
- Git & GitHub
- Visual Studio

## 🏗️ Project Structure

The project separates database operations, business logic, reusable components, and web application pages to create a more organized and maintainable structure.

Main components include:

- **AsyModbus** — Web application and user interface
- **BusinessLayer** — Business logic, entities, and database operations
- **UserControls** — Reusable interface components
- **SQL Server** — Database and stored procedures
- **Scripts** — Client-side JavaScript operations
- **Styles** — Application-specific CSS styles

## 🧩 Reusable Components

The project uses ASP.NET UserControls to reduce repeated UI code and create reusable components.

Current reusable components include:

- Phone number input control
- Reusable data grid (`ucMyGrid`)
- Search functionality
- Record count selection
- Client-side pagination
- Reusable action buttons

This structure allows common components to be reused across different listing and management pages.

## 🗄️ Database

The application uses Microsoft SQL Server.

Database operations are mainly handled with stored procedures for operations such as:

- Adding users
- Updating users
- Deleting users
- Retrieving users
- Listing users
- User authentication
- Password operations
- Role management
- Duplicate record checks

ADO.NET is used for communication between the application and SQL Server.

Database operations are centralized to reduce repeated connection, command, parameter, and transaction code.

## 🔐 User Management

The project currently includes user management features such as:

- User registration
- User editing
- User deletion
- User listing
- Login
- User roles
- Active/inactive user status
- Session management
- Password reset
- Profile image upload and management
- User data validation

## 🎨 User Interface

The application interface has been updated using Bootstrap to provide a cleaner and more responsive structure.

Current UI features include:

- Responsive page layouts
- Bootstrap cards and forms
- Responsive login page
- Responsive password reset page
- Reusable grid design
- Search and pagination controls
- Sticky table headers
- Bootstrap buttons and form controls

Custom CSS is still used where project-specific styling is required.

## 🔔 Message & Notification System

Application messages are managed through a centralized message structure.

The system separates:

- Message content
- Message type
- Message presentation

Supported notification types include:

- Success
- Warning
- Error
- Information

Toastr is used to display notifications to the user.

This structure prevents repeated message strings across pages and provides consistent notification handling throughout the application.

## 📌 Project Status

🚧 **Currently under development**

The project is continuously updated as I learn and implement new concepts.

Current development focuses on improving:

- Reusable components
- Code organization
- UI consistency
- Database architecture
- Maintainability
- Application-wide standards

## 📄 Documentation

Detailed project requirements and analysis are available in Turkish.

- [Proje Analizi (Türkçe)](docs/Proje-Analizi.md)

## 🎯 Purpose

This project is being developed as a practical learning project to improve my skills in:

- Backend development
- Frontend development
- Database design
- ASP.NET development
- ADO.NET
- Object-Oriented Programming
- Reusable component development
- Software architecture
- Clean and maintainable code
