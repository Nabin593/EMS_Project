using EmployeeManagementSystem.DataAccess;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        #region Repo Related things
        private readonly DapperRepository _repository;
        private const int PageSize = 10;
        public EmployeeController(DapperRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region Search Filter and pagination
        //public async Task<IActionResult> Index(string search = "", string department = "", int page = 1)
        //{
        //    page = page < 1 ? 1 : page;
        //    var parameters = new
        //    {
        //        Search = string.IsNullOrEmpty(search) ? null : search,
        //        Department = string.IsNullOrEmpty(department) ? null : department,
        //        PageIndex = page,
        //        PageSize
        //    };

        //    using (var connection = _repository.Connection)
        //    {
        //        var multi = await connection.QueryMultipleAsync("EXEC sp_GetEmployeesPaginated @Search, @Department, @PageIndex, @PageSize", parameters);

        //        var employees = await multi.ReadAsync<Employee>();
        //        var paginationData = await multi.ReadFirstAsync<(int TotalCount, int TotalPages)>();

        //        ViewBag.TotalPages = paginationData.TotalPages;
        //        ViewBag.CurrentPage = page;
        //        ViewBag.Search = search;
        //        ViewBag.Department = department;

        //        return View(employees);
        //    }
        //}
        public async Task<IActionResult> Index(string search = "", string department = "", int page = 1, string sortingOrder = "")
        {
            page = page < 1 ? 1 : page;

            var parameters = new
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Department = string.IsNullOrEmpty(department) ? null : department,
                PageIndex = page,
                PageSize = 10  // Set your PageSize value here
            };

            using (var connection = _repository.Connection)
            {
                var multi = await connection.QueryMultipleAsync("EXEC sp_GetEmployeesPaginated @Search, @Department, @PageIndex, @PageSize", parameters);

                var employees = await multi.ReadAsync<Employee>();
                var paginationData = await multi.ReadFirstAsync<(int TotalCount, int TotalPages)>();

                // Sorting Logic
                switch (sortingOrder)
                {
                    case "Name":
                        employees = employees.OrderBy(e => e.Name);
                        break;
                    case "Department":
                        employees = employees.OrderBy(e => e.Department);  
                        break;
                    case "Salary":
                        employees = employees.OrderBy(e => e.Salary);  
                        break;
                    default:
                        break;
                }
                ViewBag.TotalPages = paginationData.TotalPages;
                ViewBag.CurrentPage = page;
                ViewBag.Search = search;
                ViewBag.Department = department;
                ViewBag.SortingOrder = sortingOrder;

                return View(employees);
            }
        }

        #endregion

        #region GetBy ID
        public async Task<IActionResult> EmpDetails(int id)
        {
            var employee = (await _repository.QueryAsync<Employee>(
                "select  b.DepartmentName as Department,* from Employees a join Departments b on a.DepartmentID = b.DepartmentID WHERE EmployeeID = @EmployeeID AND IsDeleted = 0",
                new { EmployeeID = id })).FirstOrDefault();

            if (employee == null) return NotFound();
            return View(employee);
        }
        #endregion

        #region Add New Employee
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                var query = "EXEC sp_AddEmployee @Name, @Department, @Salary, @DateOfJoining";
                await _repository.ExecuteAsync(query, employee);
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        #endregion

        #region Get employee and edit employee
        public async Task<IActionResult> Edit(int id)
        {
            var employee = (await _repository.QueryAsync<Employee>(
                "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID AND IsDeleted = 0",
                new { EmployeeID = id })).FirstOrDefault();

            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                var query = "EXEC sp_UpdateEmployee @EmployeeID, @Name, @Department, @Salary, @DateOfJoining";
                await _repository.ExecuteAsync(query, employee);
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        #endregion

        #region Delete the employee
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.ExecuteAsync("EXEC sp_SoftDeleteEmployee @EmployeeID", new { EmployeeID = id });
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = (await _repository.QueryAsync<Employee>(
                "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID AND IsDeleted = 0",
                new { EmployeeID = id })).FirstOrDefault();

            if (employee == null) return NotFound();
            return View(employee);
        }
        #endregion
    }
}
