using Microsoft.Data.SqlClient;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IPaginationRepository
    {
        public Task<PaginatedResponse<T>> ExecutePaginationAsync<T>(
            string storedProcedure,
            Func<SqlDataReader, T> map,
            PaginationFilter filter,
            Dictionary<string, object>? extraParams = null
        );
    }
}
