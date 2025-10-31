using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IConceptService
    {
        public Task<PaginatedResponse<Concept>> FindAll(ConceptFilter filters);
        public Task<Concept> Save(Concept concept);
        public Task<Concept> FindById(int id);
        public Task<Concept> FindByName(string name);
        public Task<Concept> FindByType(string name);
        public Task<Concept> Update(int id, ConceptRequest request);
        //public Task<Concept> UpdatePartial(Concept concept);
        public Task<Concept> Delete(int id);
        //public Task<Concept> DeletePermanent(int id);
    }
}
