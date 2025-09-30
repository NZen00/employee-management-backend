using System.Data;
using System.Threading.Tasks;

namespace employee_management_backend.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}