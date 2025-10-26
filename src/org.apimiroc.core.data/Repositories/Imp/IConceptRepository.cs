using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IConceptRepository
    {
        public Task<PaginatedResponse<Concept>> FindAll(ConceptFilter filters);
        public Task<Concept> Save(Concept concept);
    }
}
