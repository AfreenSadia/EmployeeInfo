namespace EmployeeAPI.Models
{
    public class AddEmpAttendance
    {
        public int employeeId { get; set; }
        public DateTime attendanceDate { get; set; }
        public bool isPresent { get; set; }
        public bool isAbsent { get; set; }
        public bool isOffday { get; set; }
    }
}
