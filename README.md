# Payroll System API

A RESTful API for managing employee payroll built with ASP.NET Core Web API (.NET 10) and MSSQL Server.

## Features

- CRUD operations for employees
- Automatic employee number generation
- Take-home pay computation based on working days and birthday bonuses
- Swagger UI for API documentation

## Tech Stack

- **Framework**: ASP.NET Core Web API (.NET 10)
- **Database**: Microsoft SQL Server
- **Documentation**: Swagger/OpenAPI

## Business Rules

### Employee Number Generation
Format: `[PREFIX]-[5-DIGIT]-[DOB]`
- **PREFIX**: First 3 letters of last name (padded with `*` if less than 3 characters)
- **5-DIGIT**: Random number padded with zeros
- **DOB**: Date of birth in `ddMMMyyyy` format

Example: `DEL-12340-17MAY1994` for DELA CRUZ born on May 17, 1994

### Working Days
- **MWF**: Monday, Wednesday, Friday
- **TTHS**: Tuesday, Thursday, Saturday

### Pay Calculation
- Employees receive **2x daily rate** for each scheduled working day
- Employees receive **100% daily rate** on their birthday (even if not a working day)

## Setup

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or VS Code

### Database Setup

1. Open SQL Server Management Studio (SSMS)
2. Execute the script located at `Database/DatabaseSetup.sql`

```sql
-- This will create:
-- - PayrollDB database
-- - Employees table
-- - All required stored procedures
```

### Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PayrollDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Running the Application

```bash
dotnet restore
dotnet run
```

Navigate to `https://localhost:{port}/swagger` to access Swagger UI.

## API Endpoints

### Get All Employees
```http
GET /api/employees
```

**Response:**
```json
[
  {
    "employeeNumber": "DEL-12340-17MAY1994",
    "lastName": "DELA CRUZ",
    "firstName": "JUAN",
    "middleName": "",
    "birthDate": "1994-05-17",
    "dailyRate": 2000.00,
    "workingDays": "MWF"
  }
]
```

### Get Employee by Number
```http
GET /api/employees/{employeeNumber}
```

### Create Employee
```http
POST /api/employees
Content-Type: application/json

{
  "lastName": "DELA CRUZ",
  "firstName": "JUAN",
  "middleName": "",
  "birthDate": "1994-05-17",
  "dailyRate": 2000.00,
  "workingDays": "MWF"
}
```

**Response:**
```json
{
  "employeeNumber": "DEL-12340-17MAY1994",
  "lastName": "DELA CRUZ",
  "firstName": "JUAN",
  "middleName": "",
  "birthDate": "1994-05-17",
  "dailyRate": 2000.00,
  "workingDays": "MWF"
}
```

### Update Employee
```http
PUT /api/employees/{employeeNumber}
Content-Type: application/json

{
  "lastName": "DELA CRUZ",
  "firstName": "JUAN",
  "middleName": "SANTOS",
  "birthDate": "1994-05-17",
  "dailyRate": 2500.00,
  "workingDays": "TTHS"
}
```

### Delete Employee
```http
DELETE /api/employees/{employeeNumber}
```

### Compute Take-Home Pay
```http
POST /api/employees/compute-pay
Content-Type: application/json

{
  "employeeNumber": "DEL-12340-17MAY1994",
  "startDate": "2011-05-16",
  "endDate": "2011-05-20"
}
```

**Response:**
```json
{
  "employeeNumber": "DEL-12340-17MAY1994",
  "employeeName": "DELA CRUZ, JUAN",
  "startDate": "2011-05-16",
  "endDate": "2011-05-20",
  "takeHomePay": 14000.00,
  "formattedPay": "Php 14,000.00"
}
```

## Sample Test Data

### Sample 1: DELA CRUZ, JUAN
| Field | Value |
|-------|-------|
| Last Name | DELA CRUZ |
| First Name | JUAN |
| Birth Date | May 17, 1994 |
| Daily Rate | 2,000.00 |
| Working Days | MWF |

**Pay Computation:**
- Period: May 16-20, 2011
- Working Days: Mon(16), Wed(18), Fri(20) = 3 days × 2,000 × 2 = 12,000
- Birthday: May 17 (Tue) = 2,000
- **Total: Php 14,000.00**

### Sample 2: SY, ANNIE
| Field | Value |
|-------|-------|
| Last Name | SY |
| First Name | ANNIE |
| Birth Date | September 1, 1994 |
| Daily Rate | 1,500.00 |
| Working Days | TTHS |

**Pay Computation:**
- Period: September 1-9, 2011
- Working Days: Thu(1), Sat(3), Tue(6), Thu(8) = 4 days × 1,500 × 2 = 12,000
- Birthday: Sep 1 is already a working day (counted in above)
- **Total: Php 13,500.00**

## Project Structure

```
PayrollSystemAPI/
    Controllers/
        EmployeesController.cs
    Models/
        Employee.cs
        DTOs/
            CreateEmployeeRequest.cs
            UpdateEmployeeRequest.cs
            ComputePayrollRequest.cs
            PayrollResponse.cs
    Repositories/
        EmployeeRepository.cs
    Services/
        EmployeeIDGenerator.cs
        PayrollService.cs
    Database/
        DatabaseSetup.sql
    Program.cs
    appsettings.json
    README.md
```

## License

This project is for technical examination purposes.