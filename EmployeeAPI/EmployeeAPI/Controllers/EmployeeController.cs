using DocumentFormat.OpenXml.InkML;
using EmployeeAPI.Data;
using EmployeeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext dbContext;
        private readonly EmployeeRep employeeRep;
        public EmployeeController(EmployeeDbContext dbContext, EmployeeRep employeeRep)
        {
            this.dbContext = dbContext;
            this.employeeRep = employeeRep;
        }
        [HttpGet]
        public IActionResult GetEmployee()
        {
            return Ok(dbContext.Employees.ToList());
        }
        [HttpGet("monthly-attendance-report")]
        public IActionResult GetMonthlyAttendanceReport()
        {
            var monthlyAttendanceReport = dbContext.EmpAttendances
                .Include(ea => ea.Employee) // Include the Employee entity
                .GroupBy(ea => new { ea.employeeId, ea.attendanceDate.Month })
                .Select(g => new
                {
                    EmployeeId = g.Key.employeeId,
                    Month = g.Key.Month,
                    EmployeeName = g.First().Employee.employeeName, // Assuming you have a navigation property from EmployeeAttendance to Employee
                    PayableSalary = g.First().Employee.employeeSalary, // Assuming EmployeeSalary is in the Employee model
                    TotalPresent = g.Count(ea => ea.isPresent),
                    TotalAbsent = g.Count(ea => ea.isAbsent),
                    TotalOffday = g.Count(ea => ea.isOffday)
                })
                .ToList();

            return Ok(monthlyAttendanceReport);
        }


        [HttpGet("get-all")]
        public IActionResult GetAllEmployee()
        {
            var allemployee = new List<Employee>
               {
                    new Employee
                    {
                        employeeId = 502030,
                        employeeName = "Mehedi Hasan",
                        employeeCode = "EMP320",
                        employeeSalary = 50000,
                        supervisorId = 502036
                    },
                    new Employee
                    {
                        employeeId = 502031,
                        employeeName = "Ashikur Rahman",
                        employeeCode = "EMP321",
                        employeeSalary = 45000,
                        supervisorId = 502036
                    },
                    new Employee
                    {
                        employeeId = 502032,
                        employeeName = "Rakibul Islam",
                        employeeCode = "EMP322",
                        employeeSalary = 52000,
                        supervisorId = 502030
                    },
                    new Employee
                    {
                        employeeId = 502033,
                        employeeName = "Hasan Abdullah",
                        employeeCode = "EMP323",
                        employeeSalary = 46000,
                        supervisorId = 502031
                    },
                    new Employee
                    {
                        employeeId = 502034,
                        employeeName = "Akib Khan",
                        employeeCode = "EMP324",
                        employeeSalary = 66000,
                        supervisorId = 502032
                    },
                    new Employee
                    {
                        employeeId = 502035,
                        employeeName = "Rasel Shikder",
                        employeeCode = "EMP325",
                        employeeSalary = 53500,
                        supervisorId = 502033
                    },
                    new Employee
                    {
                        employeeId = 502036,
                        employeeName = "Selim Reja",
                        employeeCode = "EMP326",
                        employeeSalary = 59000,
                        supervisorId = 502035
                    }
            };

            return Ok(allemployee);
        }
        [HttpGet("get-allAttendance")]
        public IActionResult GetAttendance()
        {
            return Ok(dbContext.EmpAttendances.ToList());
        }

        [HttpGet("get-employeeAttendance")]
        public IActionResult GetAllEmployeeAttendance()
        {
            var employeeAttendance = new List<EmployeeAttendance>
            {
                new EmployeeAttendance
                {
                    attendanceId = 1,
                    employeeId = 502030,
                    attendanceDate = new DateTime(2023, 6, 24),
                    isPresent = true,
                    isAbsent = false,
                    isOffday = false
                },
                new EmployeeAttendance
                {
                    attendanceId = 2,
                    employeeId = 502030,
                    attendanceDate = new DateTime(2023, 6, 25),
                    isPresent = false,
                    isAbsent = true,
                    isOffday = false
                },
                new EmployeeAttendance
                {
                    attendanceId = 3,
                    employeeId = 502031,
                    attendanceDate = new DateTime(2023, 6, 25),
                    isPresent = true,
                    isAbsent = false,
                    isOffday = false
                }
            };

            return Ok(employeeAttendance);
        }

    [HttpGet("third-highest-salary")]
        public async Task<IActionResult> GetEmployeeWithSalary()
        {
            var thirdHighestEmployee = await dbContext.Employees
                .OrderByDescending(e => e.employeeSalary)
                .Skip(2) 
                .FirstOrDefaultAsync();

            if (thirdHighestEmployee != null)
            {
                return Ok(thirdHighestEmployee);
            }
            else
            {
                return NotFound("No employee found with the third highest salary.");
            }
        }

        [HttpGet("hierarchy/{employeeId}")]
        public async Task<IActionResult> GetEmployeeHierarchy(int employeeId)
        {
            var hierarchy = new List<string>();

            var employee = await dbContext.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            while (employee != null)
            {
                hierarchy.Add(employee.employeeName);
                employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.employeeId == employee.supervisorId);
            }

            
            hierarchy.Reverse();

            return Ok(string.Join(" -> ", hierarchy));
        }

        [HttpGet("salary-without-absent")]
        public async Task<IActionResult> GetEmployeesWithSalaryWithoutAbsent()
        {
            
            var employeesWithoutAbsent = await dbContext.Employees
                .Where(e => !dbContext.EmpAttendances.Any(a => a.employeeId == e.employeeId && a.isAbsent))
                .OrderByDescending(e => e.employeeSalary)
                .ToListAsync();

            if (employeesWithoutAbsent != null && employeesWithoutAbsent.Any())
            {
                return Ok(employeesWithoutAbsent);
            }
            else
            {
                return NotFound("No employees found with no absent records.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeReq addEmployeeReq)
        {
            var employee = new Employee
            {
                employeeId = addEmployeeReq.employeeId,
                employeeName = addEmployeeReq.employeeName,
                employeeCode = addEmployeeReq.employeeCode,
                employeeSalary = addEmployeeReq.employeeSalary,
                supervisorId = addEmployeeReq.supervisorId
            };

            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync(); 

            return Ok(employee);
        }
        [HttpPost("add-attendance")]
        public async Task<IActionResult> AddAttendance(AddEmpAttendance addEmpAttendance)
        {
            var employeeAttendance = new EmployeeAttendance
            {
                employeeId = addEmpAttendance.employeeId,
                attendanceDate = addEmpAttendance.attendanceDate,
                isAbsent = addEmpAttendance.isAbsent,
                isOffday = addEmpAttendance.isOffday,
                isPresent = addEmpAttendance.isPresent
            };
            await dbContext.EmpAttendances.AddAsync(employeeAttendance);
            await dbContext.SaveChangesAsync();

            return Ok(employeeAttendance);
        }

        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int employeeId, [FromBody] UpdateEmployeeReq updateEmployeeReq)
        {
            var employee = await dbContext.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            if (await employeeRep.IsEmployeeCodeUniqueExceptCurrentAsync(employeeId, updateEmployeeReq.employeeCode))
            {
                employee.employeeName = updateEmployeeReq.employeeName;
                employee.employeeCode = updateEmployeeReq.employeeCode;

                await dbContext.SaveChangesAsync();
                return Ok(employee);
            }
            else
            {
                return BadRequest("Employee code is already in use");
            }
        }


    }

    }


