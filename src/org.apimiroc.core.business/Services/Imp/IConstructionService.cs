using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IConstructionService
    {
        public Task<PaginatedResponse<Construction>> FindAll(ConstructionFilter filters);
        public Task<Construction?> FindByName(string name);
        public Task<Construction> Save(ConstructionRequest request);
        public Task<Construction> Update(Construction construction, string name);
        public Task<Construction> DeletePermanent(string name);
        public Task<Construction> DeleteLogic(string name);
        public Task<Construction> UpdatePartial(Construction construction, string name);
    }
}
