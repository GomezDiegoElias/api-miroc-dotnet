using Microsoft.Data.SqlClient;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IPaginationRepository
    {
        public Task<PaginatedResponse<T>> ExecutePaginationAsync<T>(
            string storedProcedure,
            Func<SqlDataReader, T> map,
            int pageIndex,
            int pageSize,
            Dictionary<string, object>? extraParams = null
        );
    }
}
