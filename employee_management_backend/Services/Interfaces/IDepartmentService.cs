using employee_management_backend.Dtos;
using employee_management_backend.Entities;

namespace employee_management_backend.Services.Interfaces;

public interface IDepartmentService
{
    Task<List<Department>> GetAllAsync();
    Task<PagedResultDto<Department>> GetPagedAsync(int page, int pageSize);
    Task<Department> GetByIdAsync(int id);
    Task AddAsync(DepartmentDto department);
    Task UpdateAsync(int id, DepartmentDto department);
    Task DeleteAsync(int id);
}
