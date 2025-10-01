using employee_management_backend.Data;
using employee_management_backend.Dtos;
using employee_management_backend.Entities;
using employee_management_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace employee_management_backend.Repositories;

public class EmployeeRepository : BaseRepository, IEmployeeRepository
{
    public EmployeeRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await ExecuteReaderListAsync(
            @"SELECT e.*, d.DepartmentCode, d.DepartmentName 
              FROM Employee e 
              LEFT JOIN Department d ON e.DepartmentId = d.DepartmentId
              ORDER BY e.employeeId",
            MapEmployeeWithDepartment);
    }

    public async Task<PagedResultDto<Employee>> GetPagedAsync(int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;

        var totalCount = await ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Employee");

        var employees = await ExecuteReaderListAsync(
            @"SELECT e.*, d.DepartmentCode, d.DepartmentName 
          FROM Employee e 
          LEFT JOIN Department d ON e.DepartmentId = d.DepartmentId
          ORDER BY e.LastName, e.FirstName 
          OFFSET @Offset ROWS 
          FETCH NEXT @PageSize ROWS ONLY",
            reader => new Employee
            {
                EmployeeId = reader.GetInt32("EmployeeId"),
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                Email = reader.GetString("Email"),
                DateOfBirth = reader.GetDateTime("DateOfBirth"),
                Age = reader.GetInt32("Age"),
                Salary = reader.GetDecimal("Salary"),
                DepartmentId = reader.GetInt32("DepartmentId"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt"),
                Department = !reader.IsDBNull(reader.GetOrdinal("DepartmentCode")) ? new Department
                {
                    DepartmentId = reader.GetInt32("DepartmentId"),
                    DepartmentCode = reader.GetString("DepartmentCode"),
                    DepartmentName = reader.GetString("DepartmentName")
                } : null
            },
            new SqlParameter("@Offset", offset),
            new SqlParameter("@PageSize", pageSize)
        );

        return new PagedResultDto<Employee>
        {
            Items = employees ?? new List<Employee>(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await ExecuteReaderAsync(
            @"SELECT e.*, d.DepartmentCode, d.DepartmentName 
              FROM Employee e 
              LEFT JOIN Department d ON e.DepartmentId = d.DepartmentId 
              WHERE e.EmployeeId = @Id",
            MapEmployeeWithDepartment,
            new SqlParameter("@Id", id));
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await ExecuteReaderAsync(
            "SELECT * FROM Employee WHERE Email = @Email",
            MapEmployee,
            new SqlParameter("@Email", email.ToLower().Trim()));
    }

    public async Task AddAsync(Employee employee)
    {
        var employeeId = await ExecuteNonQueryWithIdentityAsync(
            @"INSERT INTO Employee (FirstName, LastName, Email, DateOfBirth, Salary, DepartmentId, CreatedAt) 
          VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @Salary, @DepartmentId, GETDATE());
          SELECT CAST(SCOPE_IDENTITY() as int)",
            new SqlParameter("@FirstName", employee.FirstName),
            new SqlParameter("@LastName", employee.LastName),
            new SqlParameter("@Email", employee.Email),
            new SqlParameter("@DateOfBirth", employee.DateOfBirth),
            new SqlParameter("@Salary", employee.Salary),
            new SqlParameter("@DepartmentId", employee.DepartmentId));

        employee.EmployeeId = employeeId;

    }

    public async Task UpdateAsync(Employee employee)
    {
        await ExecuteNonQueryAsync(
            @"UPDATE Employee 
          SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
              DateOfBirth = @DateOfBirth, Salary = @Salary, 
              DepartmentId = @DepartmentId, UpdatedAt = GETDATE() 
          WHERE EmployeeId = @Id",
            new SqlParameter("@Id", employee.EmployeeId),
            new SqlParameter("@FirstName", employee.FirstName),
            new SqlParameter("@LastName", employee.LastName),
            new SqlParameter("@Email", employee.Email),
            new SqlParameter("@DateOfBirth", employee.DateOfBirth),
            new SqlParameter("@Salary", employee.Salary),
            new SqlParameter("@DepartmentId", employee.DepartmentId));
    }

    public async Task DeleteAsync(int id)
    {
        await ExecuteNonQueryAsync(
            "DELETE FROM Employee WHERE EmployeeId = @Id",
            new SqlParameter("@Id", id));
    }

    private Employee MapEmployee(SqlDataReader reader)
    {
        return new Employee
        {
            EmployeeId = reader.GetInt32("EmployeeId"),
            FirstName = reader.GetString("FirstName"),
            LastName = reader.GetString("LastName"),
            Email = reader.GetString("Email"),
            DateOfBirth = reader.GetDateTime("DateOfBirth"),
            Age = reader.GetInt32("Age"),
            Salary = reader.GetDecimal("Salary"),
            DepartmentId = reader.GetInt32("DepartmentId"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt")
        };
    }

    private Employee MapEmployeeWithDepartment(SqlDataReader reader)
    {
        var employee = MapEmployee(reader);

        if (!reader.IsDBNull(reader.GetOrdinal("DepartmentCode")))
        {
            employee.Department = new Department
            {
                DepartmentId = employee.DepartmentId,
                DepartmentCode = reader.GetString("DepartmentCode"),
                DepartmentName = reader.GetString("DepartmentName")
            };
        }

        return employee;
    }

    // Helper method to get identity value after insert
    private async Task<int> ExecuteNonQueryWithIdentityAsync(string sql, params SqlParameter[] parameters)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync() as SqlConnection;
        using var command = new SqlCommand(sql, connection);

        if (parameters?.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return (int)await command.ExecuteScalarAsync();
    }
}