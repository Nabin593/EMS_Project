Database Query:-
Create Database EmployeeManagement;
Create Table Query :-
------create table Department--------------------------------
CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE
);
------insert query for Department table-----------------------------
INSERT INTO Departments (DepartmentName) VALUES
('Account'),
('Developer'),
('HR'),
('IT'),
('QA');
------create table Employee--------------------------------
CREATE TABLE Employees ( 
EmployeeID INT IDENTITY(1,1) 
PRIMARY KEY, 
Name NVARCHAR(100) NOT NULL, 
Salary DECIMAL(18,2) NOT NULL, 
DateOfJoining DATETIME NOT NULL, 
IsDeleted BIT DEFAULT 0, 
DepartmentID INT NULL, 
CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID) 
);

------insert query for Employees table------------------------------
INSERT INTO Employees (Name, Salary, DateOfJoining, DepartmentID) VALUES
('Rajan Dulal', 60000.00, '2025-01-01', 1),
('Nabin Ghimire', 500000.00, '2025-01-29', 3),
('Ramesh Karki', 5600.00, '2025-01-16', 3),
('Sabin Khadka', 45000.00, '2025-01-08', 1),
('Ramesh Bhusal', 45660.00, '2025-01-16', 2),
('Kamal Khatri', 5000.00, '2025-01-29 19:20:30.027', 1),
('Kamala Malakar', 20000.00, '2025-01-29', 2),
('Kritika mali', 15000.00, '2025-01-29 19:20:57.330', 2),
('Kritika mali', 15000.00, '2025-01-29 19:20:57.850', 2),
('Kritika mali', 15000.00, '2025-01-29 19:20:58.333', 2),
('Ram', 50000.00, '2025-01-29 21:31:43.463', 3);


Stored Procedure :-
------To Add employee---------------------------------
Create PROCEDURE [dbo].[sp_AddEmployee]
    @Name NVARCHAR(100),
    @DepartmentID NVARCHAR(50),
    @Salary DECIMAL(18,2),
    @DateOfJoining DATETIME
AS
BEGIN
    INSERT INTO Employees (Name, DepartmentID, Salary, DateOfJoining, IsDeleted)
    VALUES (@Name, @DepartmentID, @Salary, @DateOfJoining, 0);
END;

-------------------------- Delete employee  --------------------------------------------

Create PROCEDURE [dbo].[sp_DeleteEmployee]
    @EmployeeID INT
AS
BEGIN
    DELETE FROM Employees
    WHERE EmployeeID = @EmployeeID;
END;
-------------------------- Soft Delete --------------------------------------------

Create PROCEDURE [dbo].[sp_SoftDeleteEmployee]
    @EmployeeID INT
AS
BEGIN
    UPDATE Employees
    SET IsDeleted = 1
    WHERE EmployeeID = @EmployeeID;
END;

-------------------------- Update employee  --------------------------------------------
Create PROCEDURE [dbo].[sp_UpdateEmployee]
    @EmployeeID INT,
    @Name NVARCHAR(100),
    @Department NVARCHAR(50),
    @Salary DECIMAL(18,2),
    @DateOfJoining DATETIME
AS
BEGIN
    UPDATE Employees
    SET Name = @Name, 
        DepartmentID = @Department, 
        Salary = @Salary, 
        DateOfJoining = @DateOfJoining
    WHERE EmployeeID = @EmployeeID AND IsDeleted = 0;
END;

-------------------------- GetAll employee  --------------------------------------------
Create PROCEDURE [dbo].[sp_GetEmployees]
AS
BEGIN
    SELECT * FROM [Employees] WHERE IsDeleted = 0 order by 1 desc;
END;

-------------------------- Total employee with pagination  ------------------------------
ALTER PROCEDURE [dbo].[sp_GetEmployeesPaginated]
    @Search NVARCHAR(100) = NULL,
    @Department NVARCHAR(50) = NULL,
    @PageIndex INT = 1,
    @PageSize INT = 10
AS
BEGIN
    IF @Search IS NULL OR LTRIM(RTRIM(@Search)) = ''
        SET @Search = '%'
    ELSE
        SET @Search = '%' + @Search + '%';
    DECLARE @Offset INT = CASE WHEN @PageIndex > 1 THEN (@PageIndex - 1) * @PageSize ELSE 0 END;
    SELECT e.EmployeeID, e.Name, e.Salary, e.DateOfJoining, d.DepartmentName as Department
    FROM Employees e
    JOIN Departments d ON e.DepartmentID = d.DepartmentID
    WHERE e.IsDeleted = 0
    AND (e.Name LIKE @Search)
    AND (@Department IS NULL OR @Department = '' OR d.DepartmentName = @Department)
    ORDER BY e.EmployeeID
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) 
    FROM Employees e
    JOIN Departments d ON e.DepartmentID = d.DepartmentID
    WHERE e.IsDeleted = 0
    AND (e.Name LIKE @Search)
    AND (@Department IS NULL OR @Department = '' OR d.DepartmentName = @Department);
    DECLARE @TotalPages INT;
    SET @TotalPages = CEILING(CAST(@TotalCount AS DECIMAL) / @PageSize);
    SELECT @TotalCount AS TotalCount, @TotalPages AS TotalPages;
END;

