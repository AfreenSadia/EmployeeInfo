using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Data;
using EmployeeAPI.Models;

public class EmployeeRep
{
    private readonly EmployeeDbContext _dbContext;

    public EmployeeRep(EmployeeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsEmployeeCodeUniqueExceptCurrentAsync(int employeeId, string employeeCode)
    {
        // Check if any other employee has the same code except the current one
        return !await _dbContext.Employees.AnyAsync(e => e.employeeId != employeeId && e.employeeCode == employeeCode);
    }

    public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
    {
        return await _dbContext.Employees.FindAsync(employeeId);
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        _dbContext.Entry(employee).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

   

}
