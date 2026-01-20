-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PayrollDB')
BEGIN
    CREATE DATABASE PayrollDB;
END
GO

USE PayrollDB;
GO

-- Create Employees Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE Employees (
        EmployeeNumber NVARCHAR(50) PRIMARY KEY,
        LastName NVARCHAR(100) NOT NULL,
        FirstName NVARCHAR(100) NOT NULL,
        MiddleName NVARCHAR(100) NULL,
        BirthDate DATE NOT NULL,
        DailyRate DECIMAL(18,2) NOT NULL,
        WorkingDays NVARCHAR(10) NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- Get All Employees
CREATE OR ALTER PROCEDURE sp_GetAllEmployees
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmployeeNumber, LastName, FirstName, MiddleName, BirthDate, DailyRate, WorkingDays
    FROM Employees
    ORDER BY LastName, FirstName;
END
GO

-- Get Employee by Number
CREATE OR ALTER PROCEDURE sp_GetEmployeeByNumber
    @EmployeeNumber NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmployeeNumber, LastName, FirstName, MiddleName, BirthDate, DailyRate, WorkingDays
    FROM Employees
    WHERE EmployeeNumber = @EmployeeNumber;
END
GO

-- Upsert Employee (Insert or Update)
CREATE OR ALTER PROCEDURE sp_UpsertEmployee
    @EmployeeNumber NVARCHAR(50),
    @LastName NVARCHAR(100),
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100),
    @BirthDate DATE,
    @DailyRate DECIMAL(18,2),
    @WorkingDays NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM Employees WHERE EmployeeNumber = @EmployeeNumber)
    BEGIN
        UPDATE Employees
        SET LastName = @LastName,
            FirstName = @FirstName,
            MiddleName = @MiddleName,
            BirthDate = @BirthDate,
            DailyRate = @DailyRate,
            WorkingDays = @WorkingDays,
            UpdatedAt = GETDATE()
        WHERE EmployeeNumber = @EmployeeNumber;
    END
    ELSE
    BEGIN
        INSERT INTO Employees (EmployeeNumber, LastName, FirstName, MiddleName, BirthDate, DailyRate, WorkingDays)
        VALUES (@EmployeeNumber, @LastName, @FirstName, @MiddleName, @BirthDate, @DailyRate, @WorkingDays);
    END
END
GO

-- Delete Employee
CREATE OR ALTER PROCEDURE sp_DeleteEmployee
    @EmployeeNumber NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Employees WHERE EmployeeNumber = @EmployeeNumber;
END
GO
