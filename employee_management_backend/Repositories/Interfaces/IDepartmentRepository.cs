using employee_management_backend.Dtos;
using employee_management_backend.Entities;

namespace employee_management_backend.Repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task<PagedResultDto<Department>> GetPagedAsync(int page, int pageSize);
    Task<Department> GetByIdAsync(int id);
    Task<Department?> GetByCodeAsync(string code);
    Task AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(int id);
}
