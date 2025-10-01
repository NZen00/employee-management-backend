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

    public async Task<PagedResultDto<Department>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max limit

        return await _repository.GetPagedAsync(page, pageSize);
    }

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
        department.DepartmentId = domain.DepartmentId;
    }

    public async Task UpdateAsync(int id, DepartmentDto department) 
    {
        var existingId = await _repository.GetByIdAsync(id);
        if (existingId == null)
            throw new KeyNotFoundException($"Department with ID {id} not found.");

        var existingCode = await _repository.GetByCodeAsync(department.DepartmentCode);
        if (existingCode != null && existingCode.DepartmentId != id)
            throw new ArgumentException("Department code already exists.");

        var domain = new Department
        {
            DepartmentId = id,
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
