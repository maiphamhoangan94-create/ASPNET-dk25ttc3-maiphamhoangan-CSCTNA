using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.Data.SqlClient;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Services
{
    public class AdoNetReportService
    {
        private readonly string _connectionString;

        public AdoNetReportService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<CourseGoalSummary> GetSummaryAsync()
        {
            var summary = new CourseGoalSummary();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            summary.UserCount = await CountTableRowsAsync(connection, "Users");
            summary.CategoryCount = await CountTableRowsAsync(connection, "Categories");
            summary.PostCount = await CountTableRowsAsync(connection, "Posts");

            return summary;
        }

        private static async Task<int> CountTableRowsAsync(SqlConnection connection, string tableName)
        {
            await using var command = new SqlCommand($"SELECT COUNT(*) FROM [{tableName}]", connection);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
