using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Response.Movements;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IMovementRepository
    {
        public Task<Movement> Save(Movement movement);
        public Task<PaginatedResponse<Movement>> FindAll(MovementFilter filters);
        public Task<PaginatedResponse<Movement>> FindAllV2(MovementFilter filters);
        public Task<Movement> FindByCode(int code);
        public Task<Movement> FindById(string id);
        public Task<Movement> Update(Movement movement);
        public Task<Movement> UpdatePartial(Movement movement);
        public Task DeleteLogic(Movement movement);
        public Task<TotalSummaryOfMovements> getTotalSumarry();
    }
}
