using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IConceptService
    {
        public Task<PaginatedResponse<Concept>> FindAll(ConceptFilter filters);
        public Task<Concept> Save(Concept concept);
    }
}
