using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeAPI.Models
{
    public class EmployeeAttendance
    {
        [Key]
        public int attendanceId { get; set; }
       
        public int employeeId { get; set; }
        [ForeignKey("employeeId")]
        public Employee Employee { get; set; }
        public DateTime attendanceDate { get; set; }
        public bool isPresent { get; set; }
        public bool isAbsent { get; set; }
        public bool isOffday { get; set; }
    }
}
