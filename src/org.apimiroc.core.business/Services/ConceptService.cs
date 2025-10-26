using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.business.Services
{
    public class ConceptService : IConceptService
    {

        private readonly IConceptRepository _repository;

        public ConceptService(IConceptRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<Concept>> FindAll(ConceptFilter filters)
        {
            return await _repository.FindAll(filters);
        }

        public async Task<Concept> Save(Concept concept)
        {
            return await _repository.Save(concept);
        }
    }
}
