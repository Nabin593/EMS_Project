using EmployeeManagementSystem.DataAccess;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        #region Repo related things
        private readonly DapperRepository _repository;
        public DepartmentController(DapperRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region Get Departments
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _repository.QueryAsync<Department>("SELECT * FROM Departments order by DepartmentID asc");
            return Json(departments);
        }
        public async Task<IActionResult> Index()
        {
            var departments = await _repository.QueryAsync<Department>("SELECT * FROM Departments order by DepartmentID asc");
            return View(departments);
        }

        #endregion

        #region Add Department
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                var query = "INSERT INTO Departments (DepartmentName) VALUES (@DepartmentName)";
                await _repository.ExecuteAsync(query, department);
                return RedirectToAction("Index");
            }
            return View(department);
        }
        #endregion
    }
}
