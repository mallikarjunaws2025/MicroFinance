# MicroFinance Management System

A comprehensive ASP.NET MVC 4 web application for managing microfinance operations including member management, loan disbursement, collections, and reporting.

## Features

- **Member Management**: Register and manage customer information
- **Loan Management**: Loan disbursement, EMI calculation, and tracking
- **Collection Management**: Track payments and outstanding amounts
- **Staff Management**: User authentication and role-based access
- **Reporting**: Daily reports, loan reports, and financial summaries
- **Group Management**: Organize members into financial groups
- **Dashboard**: Visual charts and analytics

## Technology Stack

- **Framework**: ASP.NET MVC 4 (.NET Framework 4.5)
- **Database**: SQL Server with Entity Framework 5.0
- **Authentication**: Custom session-based authentication with password hashing
- **Logging**: NLog framework
- **UI**: Bootstrap responsive design

## Prerequisites

- Visual Studio 2019/2022 or Build Tools
- SQL Server LocalDB (comes with Visual Studio)
- IIS Express
- .NET Framework 4.5 or higher

## Quick Start

### Option 1: Using Startup Script
1. Navigate to the project directory
2. Run `StartApp.bat`
3. The application will build and start automatically

### Option 2: Manual Setup
1. **Database Setup**:
   ```cmd
   sqllocaldb start MSSQLLocalDB
   ```

2. **Build the Project**:
   ```cmd
   msbuild ManageFinancery.csproj /p:Configuration=Debug
   ```

3. **Run the Application**:
   ```cmd
   iisexpress /path:"%cd%" /port:8080
   ```

4. **Access the Application**:
   - URL: http://localhost:8080
   - Default Login: `admin` / `admin123`

## Default Admin Account

When the application starts for the first time, it automatically creates a default administrator account:
- **Username**: admin
- **Password**: admin123
- **Role**: Admin

**Important**: Change the default password immediately after first login.

## Security Features

- Password hashing using SHA-256 with salt
- SQL injection protection
- Session-based authentication
- Role-based access control (Admin/User)
- User activity logging
- Input validation and sanitization

## Database Schema

Main entities:
- **Staff**: Employee management
- **StaffLogin**: Authentication credentials
- **Member**: Customer information
- **Loan**: Loan details and disbursement
- **Loan_Cols**: Loan collections/payments
- **FinGroup**: Financial groups
- **Branch**: Branch information
- **UserLog**: User activity tracking

## Configuration

### Database Connection
The application uses SQL Server LocalDB by default. To change the connection:

1. Edit `Web.config`
2. Update the `MicroFinanceEntities` connection string
3. Ensure the target database has the required schema

### Application Settings
Key settings in `Web.config`:
- `IntrRate`: Default interest rate
- `AdvancedEMI`: EMI calculation mode
- Logging configuration in `NLog.config`

## Project Structure

```
├── Controllers/         # MVC Controllers
├── Views/              # Razor Views
├── VModels/            # View Models
├── DB/                 # Entity Framework Models
├── Comman/             # Helper classes
├── Filters/            # Custom authorization filters
├── Validation/         # Custom validation attributes
├── App_Data/           # Database and scripts
├── Scripts/            # JavaScript files
├── Content/            # CSS and styling
└── Resource/           # Images and resources
```

## API Endpoints

The application follows standard MVC routing:
- `/Staff/StaffLogin` - Login page
- `/Charts/Index` - Dashboard (post-login)
- `/Member/CreateMember` - Member registration
- `/Loans/LoansDisbus` - Loan disbursement
- `/Report/*` - Various reports

## Troubleshooting

### Common Issues

1. **Database Connection Errors**:
   - Ensure SQL Server LocalDB is running
   - Check connection string in Web.config
   - Verify database permissions

2. **Build Errors**:
   - Restore NuGet packages
   - Check .NET Framework version
   - Verify Entity Framework references

3. **Authentication Issues**:
   - Clear browser cookies/session
   - Check if default admin was created
   - Verify session configuration

### Logs
- Application logs are stored using NLog
- Check the configured log path for error details
- Enable debug mode for detailed error information

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This is a proprietary microfinance management system. All rights reserved.

## Support

For support and maintenance:
- Check the application logs for errors
- Verify database connectivity
- Ensure all prerequisites are installed
- Contact the development team for assistance

---

**Note**: This application handles sensitive financial data. Ensure proper security measures are in place before deploying to production.