using employee_management_backend.Dtos;
using employee_management_backend.Entities;
using employee_management_backend.Repositories.Interfaces;
using employee_management_backend.Services.Interfaces;

namespace employee_management_backend.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeService(IEmployeeRepository repository, IDepartmentRepository departmentRepository)
    {
        _repository = repository;
        _departmentRepository = departmentRepository;
    }

    public async Task<List<Employee>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<PagedResultDto<Employee>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max limit

        return await _repository.GetPagedAsync(page, pageSize);
    }

    public async Task<Employee> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        return employee ?? throw new KeyNotFoundException($"Employee with ID {id} not found.");
    }

    public async Task AddAsync(EmployeeDto employee)
    {
        await ValidateEmployeeAsync(employee);

        // Calculate age for validation (but don't send to database)
        var age = CalculateAge(employee.DateOfBirth);
        if (age < 18)
            throw new ArgumentException("Employee must be at least 18 years old.");

        // Check if email already exists
        if (await _repository.GetByEmailAsync(employee.Email) != null)
            throw new ArgumentException("Email address already exists.");

        var domain = new Employee
        {
            FirstName = employee.FirstName.Trim(),
            LastName = employee.LastName.Trim(),
            Email = employee.Email.ToLower().Trim(),
            DateOfBirth = employee.DateOfBirth,
            Salary = employee.Salary,
            DepartmentId = employee.DepartmentId
        };

        await _repository.AddAsync(domain);
        employee.EmployeeId = domain.EmployeeId;
    }

    public async Task UpdateAsync(int id, EmployeeDto employee)
    {
        if (id != employee.EmployeeId)
            throw new ArgumentException("ID mismatch between route and request body.");

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found.");

        await ValidateEmployeeAsync(employee);

        // Calculate age for validation
        var age = CalculateAge(employee.DateOfBirth);
        if (age < 18)
            throw new ArgumentException("Employee must be at least 18 years old.");

        // Check if email exists for other employees
        var existingByEmail = await _repository.GetByEmailAsync(employee.Email);
        if (existingByEmail != null && existingByEmail.EmployeeId != id)
            throw new ArgumentException("Email address already exists for another employee.");

        var domain = new Employee
        {
            EmployeeId = id,
            FirstName = employee.FirstName.Trim(),
            LastName = employee.LastName.Trim(),
            Email = employee.Email.ToLower().Trim(),
            DateOfBirth = employee.DateOfBirth,
            Salary = employee.Salary,
            DepartmentId = employee.DepartmentId
        };

        await _repository.UpdateAsync(domain);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found.");

        await _repository.DeleteAsync(id);
    }

    private async Task ValidateEmployeeAsync(EmployeeDto employee)
    {
        // Validate department exists
        var department = await _departmentRepository.GetByIdAsync(employee.DepartmentId);
        if (department == null)
            throw new ArgumentException("Selected department does not exist.");
    }

    // Keep this helper method for validation
    private int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}