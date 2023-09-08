using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    public class AddEmployeeReq
    {
        public int employeeId { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }
        public int employeeSalary { get; set; }
        public int supervisorId { get; set; }
    }
}
