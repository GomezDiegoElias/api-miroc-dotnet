using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services
{
    public class ConceptService : IConceptService
    {

        private readonly IConceptRepository _repository;

        public ConceptService(IConceptRepository repository)
        {
            _repository = repository;
        }

        public async Task<Concept> Delete(int id)
        {
            var entity = await FindById(id) ?? throw new ConceptNotFoundException(id);
            await _repository.Delete(entity);
            return entity;
            
        }
        //public async Task<Concept> DeletePermanent(int id)
        //{
        //    var entity = await FindById(id) ?? throw new ConceptNotFoundException(id);
        //    await _repository.DeletePermanent(entity);
        //    return entity;
        //}

        public async Task<PaginatedResponse<Concept>> FindAll(ConceptFilter filters)
        {
            return await _repository.FindAll(filters);
        }

        public async Task<Concept> FindById(int id)
        {
            if (id <= 0) throw new ConceptNotFoundException($"El ID {id} ingresado no esta permitido");
            return await _repository.FindById(id) ?? throw new ConceptNotFoundException(id);
        }

        public async Task<Concept> FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ConceptNotFoundException($"El nombre {name} debe estar vacio ni con espacios vacios");
            return await _repository.FindByName(name) ?? throw new ConceptNotFoundException($"Concepto con nombre {name} no existe");
        }

        public async Task<Concept> FindByType(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ConceptNotFoundException($"El tipo {name} debe estar vacio ni con espacios vacios");
            return await _repository.FindByType(name) ?? throw new ConceptNotFoundException($"Concepto con tipo {name} no existe");
        }

        public async Task<Concept> Save(Concept concept)
        {
            return await _repository.Save(concept);
        }

        public async Task<Concept> Update(int id, ConceptRequest request)
        {
            var existingConcept = await FindById(id) ?? throw new ConceptNotFoundException(id);
            existingConcept.Name = request.Name;
            existingConcept.type = request.Type;
            existingConcept.Description = request.Description!;
            return await _repository.Update(existingConcept);
        }

        //public Task<Concept> UpdatePartial(Concept concept)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
