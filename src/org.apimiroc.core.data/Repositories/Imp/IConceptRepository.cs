using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IConceptRepository
    {
        public Task<PaginatedResponse<Concept>> FindAll(ConceptFilter filters);
        public Task<Concept> Save(Concept concept);
        public Task<Concept?> FindById(int id);
        public Task<Concept?> FindByName(string name);
        public Task<Concept?> FindByType(string name);
        public Task<Concept> Update(Concept concept);
        //public Task<Concept> UpdatePartial(Concept concept);
        public Task Delete(Concept concept);
        //public Task DeletePermanent(Concept concept);
    }
}
