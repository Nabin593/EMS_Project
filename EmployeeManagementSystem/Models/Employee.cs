namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public DateTime DateOfJoining { get; set; }
        public bool IsDeleted { get; set; }
    }
}
