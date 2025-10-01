using employee_management_backend.Data;
using employee_management_backend.Dtos;
using employee_management_backend.Entities;
using employee_management_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace employee_management_backend.Repositories;
public class DepartmentRepository : BaseRepository, IDepartmentRepository
{
    public DepartmentRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<List<Department>> GetAllAsync()
    {
        return await ExecuteReaderListAsync(
            "SELECT * FROM Department",
            reader => new Department
            {
                DepartmentId = reader.GetInt32("DepartmentId"),
                DepartmentCode = reader.GetString("DepartmentCode"),
                DepartmentName = reader.GetString("DepartmentName"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt")
            });
    }

    public async Task<Department> GetByIdAsync(int id)
    {
        return await ExecuteReaderAsync(
            "SELECT * FROM Department WHERE DepartmentId = @Id",
            reader => new Department
            {
                DepartmentId = reader.GetInt32("DepartmentId"),
                DepartmentCode = reader.GetString("DepartmentCode"),
                DepartmentName = reader.GetString("DepartmentName"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt")
            },
            new SqlParameter("@Id", id));
    }

    public async Task<Department?> GetByCodeAsync(string code)
    {
        return await ExecuteReaderAsync(
            "SELECT DepartmentId, DepartmentCode, DepartmentName, CreatedAt, UpdatedAt FROM dbo.Department WHERE DepartmentCode = @Code",
            reader => new Department
            {
                DepartmentId = reader.GetInt32("DepartmentId"),
                DepartmentCode = reader.GetString("DepartmentCode"),
                DepartmentName = reader.GetString("DepartmentName"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt")
            },
            new SqlParameter("@Code", code));
    }

    public async Task AddAsync(Department department)
    {
        await ExecuteNonQueryAsync(
            "INSERT INTO Department (DepartmentCode, DepartmentName) VALUES (@Code, @Name)",
            new SqlParameter("@Code", department.DepartmentCode),
            new SqlParameter("@Name", department.DepartmentName));
    }

    public async Task UpdateAsync(Department department)
    {
        await ExecuteNonQueryAsync(
            "UPDATE Department SET DepartmentCode = @Code, DepartmentName = @Name, UpdatedAt = GETDATE() WHERE DepartmentId = @Id",
            new SqlParameter("@Id", department.DepartmentId),
            new SqlParameter("@Code", department.DepartmentCode),
            new SqlParameter("@Name", department.DepartmentName));
    }

    public async Task DeleteAsync(int id)
    {
        await ExecuteNonQueryAsync(
            "DELETE FROM Department WHERE DepartmentId = @Id",
            new SqlParameter("@Id", id));
    }

    public async Task<PagedResultDto<Department>> GetPagedAsync(int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;

        var totalCount = await ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Department");

        // Get paginated data
        var departments = await ExecuteReaderListAsync(
            @"SELECT * FROM Department 
          ORDER BY DepartmentName 
          OFFSET @Offset ROWS 
          FETCH NEXT @PageSize ROWS ONLY",
            reader => new Department
            {
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode")),
                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            },
            new SqlParameter("@Offset", offset),
            new SqlParameter("@PageSize", pageSize)
        );

        return new PagedResultDto<Department>
        {
            Items = departments,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}