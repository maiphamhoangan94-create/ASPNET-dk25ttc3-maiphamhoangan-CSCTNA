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
            var baoCao = new CourseGoalSummary();

            await using var ketNoi = new SqlConnection(_connectionString);
            await ketNoi.OpenAsync();

            baoCao.UserCount = await CountTableRowsAsync(ketNoi, "Users");
            baoCao.CategoryCount = await CountTableRowsAsync(ketNoi, "Categories");
            baoCao.PostCount = await CountTableRowsAsync(ketNoi, "Posts");

            return baoCao;
        }

        private static async Task<int> CountTableRowsAsync(SqlConnection ketNoi, string tenBang)
        {
            await using var cauLenh = new SqlCommand($"SELECT COUNT(*) FROM [{tenBang}]", ketNoi);
            var ketQua = await cauLenh.ExecuteScalarAsync();
            return Convert.ToInt32(ketQua);
        }
    }
}
