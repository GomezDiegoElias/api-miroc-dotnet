using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IConstructionRepository
    {
        public Task<PaginatedResponse<Construction>> FindAllV2(ConstructionFilter filters);
        public Task<PaginatedResponse<Construction>> FindAll(ConstructionFilter filters);
        public Task<Construction?> FindByName(string name);
        public Task<Construction?> FindById(string id);
        public Task<Construction> Save(Construction construction);
        public Task<Construction> Update(Construction construction, string nameOld);
        public Task<Construction> DeletePermanent(string name);
        public Task<Construction> DeleteLogic(string name);
        public Task<Construction> UpdatePartial(Construction construction, string nameOld);
    }
}
