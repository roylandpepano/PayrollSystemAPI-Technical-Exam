using Microsoft.Data.SqlClient;
using PayrollSystemAPI___Technical_Exam.Models;
using System.Data;

namespace PayrollSystemAPI___Technical_Exam.Repositories
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string not configured");
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            var employees = new List<Employee>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllEmployees", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                employees.Add(MapEmployee(reader));
            }

            return employees;
        }

        public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetEmployeeByNumber", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapEmployee(reader);
            }

            return null;
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UpsertEmployee", connection);
            command.CommandType = CommandType.StoredProcedure;

            AddEmployeeParameters(command, employee);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return employee;
        }

        public async Task<Employee?> UpdateAsync(string employeeNumber, Employee employee)
        {
            employee.EmployeeNumber = employeeNumber;

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UpsertEmployee", connection);
            command.CommandType = CommandType.StoredProcedure;

            AddEmployeeParameters(command, employee);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0 ? employee : null;
        }

        public async Task<bool> DeleteAsync(string employeeNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteEmployee", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        private static void AddEmployeeParameters(SqlCommand command, Employee employee)
        {
            command.Parameters.AddWithValue("@EmployeeNumber", employee.EmployeeNumber);
            command.Parameters.AddWithValue("@LastName", employee.LastName);
            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
            command.Parameters.AddWithValue("@MiddleName", employee.MiddleName ?? string.Empty);
            command.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
            command.Parameters.AddWithValue("@DailyRate", employee.DailyRate);
            command.Parameters.AddWithValue("@WorkingDays", employee.WorkingDays);
        }

        private static Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeNumber = reader["EmployeeNumber"].ToString() ?? string.Empty,
                LastName = reader["LastName"].ToString() ?? string.Empty,
                FirstName = reader["FirstName"].ToString() ?? string.Empty,
                MiddleName = reader["MiddleName"].ToString() ?? string.Empty,
                BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                DailyRate = Convert.ToDecimal(reader["DailyRate"]),
                WorkingDays = reader["WorkingDays"].ToString() ?? "MWF"
            };
        }
    }
}
