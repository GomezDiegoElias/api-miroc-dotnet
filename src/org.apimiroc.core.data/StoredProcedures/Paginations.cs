using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.apimiroc.core.data.StoredProcedures
{
    public static class Paginations
    {

        // Metodo para la paginacion de usuarios
        //public static async Task<PaginatedResponse<User>> getUserPagination(int pageIndex, int pageSize)
        //{
        //    var users = new List<User>();
        //    var totalItems = 0;

        //    using var connection = new SqlConnection(Database.GetConnectionString());
        //    await connection.OpenAsync();

        //    using var command = new SqlCommand("getUserPagination", connection);
        //    command.CommandType = CommandType.StoredProcedure;
        //    command.Parameters.AddWithValue("@PageIndex", pageIndex);
        //    command.Parameters.AddWithValue("@PageSize", pageSize);

        //    using var reader = await command.ExecuteReaderAsync();

        //    while (await reader.ReadAsync())
        //    {
        //        users.Add(User.Builder()
        //            .Id(reader["id"].ToString() ?? string.Empty)
        //            .Dni(Convert.ToInt64(reader["dni"]))
        //            .Email(reader["email"].ToString() ?? string.Empty)
        //            .FirstName(reader["first_name"].ToString() ?? string.Empty)
        //            .LastName(reader["last_name"].ToString() ?? string.Empty)
        //            .Role(new Role(reader["role"].ToString() ?? string.Empty, Enumerable.Empty<string>()))
        //            .Status(Enum.Parse<Status>(reader["status"].ToString() ?? string.Empty))
        //            .Build()
        //        );

        //        if (totalItems == 0)
        //        {
        //            totalItems = Convert.ToInt32(reader["TotalFilas"]);
        //        }
        //    }

        //    await connection.CloseAsync();

        //    var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    return new PaginatedResponse<User>
        //    {
        //        Items = users,
        //        PageIndex = pageIndex,
        //        PageSize = pageSize,
        //        TotalItems = totalItems,
        //        TotalPages = totalPages
        //    };
        //}

    }
}
