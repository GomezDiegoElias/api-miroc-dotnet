using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IMovementService
    {
        public Task<Movement> SaveV2(Movement movement);
        public Task<Movement> Save(MovementRequest request);
        public Task<PaginatedResponse<Movement>> FindAllV2(MovementFilter filters);
        public Task<PaginatedResponse<Movement>> FindAll(MovementFilter filters);
        public Task<Movement> FindByCode(int code);
        public Task<Movement> FindById(string id);
        public Task<Movement> Update(Movement movement, int code);
        public Task<Movement> UpdatePartial(Movement movement, int code);
        public Task DeleteById(string id);
        public Task DeleteByCode(int code);
    }
}
