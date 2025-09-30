using employee_management_backend.Dtos;
using employee_management_backend.Entities;
using employee_management_backend.Repositories.Interfaces;
using employee_management_backend.Services.Interfaces;

namespace employee_management_backend.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Department>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<Department> GetByIdAsync(int id)
    {
        var department = await _repository.GetByIdAsync(id);
        return department ?? throw new KeyNotFoundException($"Department with ID {id} not found.");
    }

    public async Task AddAsync(DepartmentDto department)
    {
        if (await _repository.GetByCodeAsync(department.DepartmentCode) != null)
            throw new ArgumentException("Department code already exists.");

        var domain = new Department
        {
            DepartmentCode = department.DepartmentCode,
            DepartmentName = department.DepartmentName
        };
        await _repository.AddAsync(domain);
        department.DepartmentId = domain.DepartmentId; // Update DTO with ID
    }

    public async Task UpdateAsync(int id, DepartmentDto department) // Changed to accept id
    {
        var existingId = await _repository.GetByIdAsync(id);
        if (existingId == null)
            throw new KeyNotFoundException($"Department with ID {id} not found.");

        var existingCode = await _repository.GetByCodeAsync(department.DepartmentCode);
        if (existingCode != null && existingCode.DepartmentId != id)
            throw new ArgumentException("Department code already exists.");

        var domain = new Department
        {
            DepartmentId = id, // Use URL id
            DepartmentCode = department.DepartmentCode,
            DepartmentName = department.DepartmentName
        };
        await _repository.UpdateAsync(domain);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Department with ID {id} not found.");

        await _repository.DeleteAsync(id);
    }
}
