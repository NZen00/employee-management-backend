using employee_management_backend.Data;
using employee_management_backend.Repositories;
using employee_management_backend.Repositories.Interfaces;
using employee_management_backend.Services;
using employee_management_backend.Services.Interfaces;

namespace financial_accounting_backend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Application Services
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }
}