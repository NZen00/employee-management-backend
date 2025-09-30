using employee_management_backend.Dtos;
using employee_management_backend.Entities;

namespace employee_management_backend.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee> GetByIdAsync(int id);
    Task AddAsync(EmployeeDto employee);
    Task UpdateAsync(int id, EmployeeDto employee);
    Task DeleteAsync(int id);
}