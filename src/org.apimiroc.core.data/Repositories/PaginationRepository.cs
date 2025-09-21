using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.shared.Dto.General;
using System.Data;

namespace org.apimiroc.core.data.Repositories
{
    public class PaginationRepository : IPaginationRepository
    {

        private readonly string _connectionString;

        public PaginationRepository(DbContextOptions<AppDbContext> dbContextOptions)
        {
            var builder = new AppDbContext(dbContextOptions);
            _connectionString = builder.Database.GetDbConnection().ConnectionString;
        }

        public async Task<PaginatedResponse<T>> ExecutePaginationAsync<T>(
            string storedProcedure, 
            Func<SqlDataReader, T> map, 
            int pageIndex, 
            int pageSize, 
            Dictionary<string, object>? extraParams = null
        )
        {

            var items = new List<T>();
            var totalItems = 0;

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parámetros comunes de paginación
            command.Parameters.AddWithValue("@PageIndex", pageIndex);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            // Parámetros adicionales si aplica
            if (extraParams != null)
            {
                foreach (var param in extraParams)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(map(reader));

                if (totalItems == 0 && reader["TotalFilas"] != DBNull.Value)
                {
                    totalItems = Convert.ToInt32(reader["TotalFilas"]);
                }
            }

            await connection.CloseAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new PaginatedResponse<T>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

        }

    }
}
