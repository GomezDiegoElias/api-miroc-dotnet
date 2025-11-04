using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services
{
    public class ConstructionService : IConstructionService
    {

        private readonly IConstructionRepository _constructionRepository;

        public ConstructionService(IConstructionRepository constructionRepository)
        {
            _constructionRepository = constructionRepository;
        }

        public async Task<Construction> DeleteLogic(string name)
        {
            return await _constructionRepository.DeleteLogic(name);
        }

        public async Task<Construction> DeletePermanent(string name)
        {
            return await _constructionRepository.DeletePermanent(name);
        }

        public async Task<PaginatedResponse<Construction>> FindAll(ConstructionFilter filters)
        {
            return await _constructionRepository.FindAll(filters);
        }

        public async Task<Construction?> FindById(string id)
        {
            return await _constructionRepository.FindById(id)
                ?? throw new ConstructionNotFoundException(id);
        }

        public async Task<Construction?> FindByName(string name)
        {
            return await _constructionRepository.FindByName(name)
                ?? throw new ConstructionNotFoundException(name);
        }

        public async Task<Construction> Save(ConstructionRequest request)
        {
            // Validar unicidad del nombre
            var exists = await _constructionRepository.FindByName(request.Name);
            if (exists != null)
                throw new Exception("El nombre de la construcción ya existe.");

            // Validaciones

            var newConstruction = new Construction
            {
                Id = Construction.GenerateId(),
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Address = request.Address,
                Description = request.Description,
                ClientId = request.ClientId
            };

            var saveConstruction = await _constructionRepository.Save(newConstruction);

            return saveConstruction;

        }

        public async Task<Construction> Update(Construction construction, string name)
        {
            return await _constructionRepository.Update(construction, name);
        }

        public async Task<Construction> UpdatePartial(Construction construction, string name)
        {
            return await _constructionRepository.UpdatePartial(construction, name);
        }
    }
}
