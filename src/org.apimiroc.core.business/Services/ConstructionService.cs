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
        private readonly IClientRepository _clientRepository;

        public ConstructionService(IConstructionRepository constructionRepository, IClientRepository clientRepository)
        {
            _constructionRepository = constructionRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Construction> DeleteLogic(string name)
        {
            return await _constructionRepository.DeleteLogic(name);
        }

        public async Task<Construction> DeletePermanent(string name)
        {
            return await _constructionRepository.DeletePermanent(name);
        }

        public async Task<PaginatedResponse<Construction>> FindAllV2(ConstructionFilter filters)
        {
            return await _constructionRepository.FindAllV2(filters);
        }

        public async Task<PaginatedResponse<Construction>> FindAll(ConstructionFilter filters)
        {
            return await _constructionRepository.FindAll(filters);
        }

        public async Task<Construction?> FindById(string id)
        {
            return await _constructionRepository.FindById(id)
                ?? throw new ConstructionNotFoundException($"Obra con ID {id} no existe");
        }

        public async Task<Construction?> FindByName(string name)
        {
            return await _constructionRepository.FindByName(name)
                ?? throw new ConstructionNotFoundException($"Obra con nombre {name} no existe");
        }

        public async Task<Construction> Save(ConstructionRequest request)
        {

            // Validar unicidad del nombre
            var exists = await _constructionRepository.FindByName(request.Name);
            if (exists != null)
                throw new ConstructionNotFoundException($"El nombre {request.Name} ya existe.");

            var client = await _clientRepository.FindByDni(request.ClientDni)
                ?? throw new ClientNotFoundException(request.ClientDni);

            var construction = new Construction
            {
                Id = Construction.GenerateId(),
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Address = request.Address,
                Description = request.Description,
                ClientId = client.Id
            };

            return await _constructionRepository.Save(construction);

        }

        public async Task<Construction> SaveV2(ConstructionRequestV2 request)
        {
            // Validar unicidad del nombre
            var exists = await _constructionRepository.FindByName(request.Name);
            if (exists != null)
                throw new ConstructionNotFoundException($"El nombre {request.Name} ya existe.");

            var client = await _clientRepository.FindById(request.ClientId)
                ?? throw new ClientNotFoundException($"Cliente con ID {request.ClientId} no existe");

            // Validaciones

            var newConstruction = new Construction
            {
                Id = Construction.GenerateId(),
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Address = request.Address,
                Description = request.Description,
                ClientId = client.Id
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
