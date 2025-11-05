using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.business.Services
{
    public class MovementService : IMovementService
    {

        private readonly IMovementRepository _repository;
        private readonly IConceptRepository _conceptRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConstructionRepository _constructionRepository;

        public MovementService(
            IMovementRepository repository,
            IConceptRepository conceptRepository,
            IClientRepository clientRepository,
            IProviderRepository providerRepository,
            IEmployeeRepository employeeRepository,
            IConstructionRepository constructionRepository)
        {
            _repository = repository;
            _conceptRepository = conceptRepository;
            _clientRepository = clientRepository;
            _providerRepository = providerRepository;
            _employeeRepository = employeeRepository;
            _constructionRepository = constructionRepository;
        }

        public async Task DeleteByCode(int code)
        {
            var movement = await FindByCode(code);
            await _repository.DeleteLogic(movement);
        }

        public async Task DeleteById(string id)
        {
            var movement = await FindById(id);
            await _repository.DeleteLogic(movement);
        }

        public async Task<PaginatedResponse<Movement>> FindAll(MovementFilter filters)
        {
            return await _repository.FindAll(filters);
        }

        public async Task<Movement> FindByCode(int code)
        {
            return await _repository.FindByCode(code)
                ?? throw new MovementNotFoundException(code);
        }

        public async Task<Movement> FindById(string id)
        {
            return await _repository.FindById(id)
                ?? throw new MovementNotFoundException($"Movimiento con ID {id} no existe");
        }

        public async Task<Movement> Save(Movement movement)
        {

            // Validaciones
            var concept = await _conceptRepository.FindById(movement.ConceptId)
                ?? throw new ConceptNotFoundException(movement.ConceptId);

            if (movement.ClientId != null && await _clientRepository.FindById(movement.ClientId) == null)
                throw new ClientNotFoundException($"Cliente con ID {movement.ClientId} no existe");

            if (movement.EmployeeId != null && await _employeeRepository.FindById(movement.EmployeeId) == null)
                throw new EmployeeNotFoundException($"Empleado con ID {movement.EmployeeId} no existe");

            if (movement.ProviderId != null && await _providerRepository.FindById(movement.ProviderId) == null)
                throw new ProviderNotFoundException($"Proveedor con ID {movement.ProviderId} no existe");

            if (movement.ConstructionId != null && await _constructionRepository.FindById(movement.ConstructionId) == null)
                throw new ConstructionNotFoundException($"Obra con ID {movement.ConstructionId} no existe");

            return await _repository.Save(movement);
        
        }

        public async Task<Movement> Update(Movement movement, int code)
        {
            var existingMovement = await _repository.FindByCode(code)
                ?? throw new MovementNotFoundException(code);

            existingMovement.Amount = movement.Amount;
            existingMovement.PaymentMethod = movement.PaymentMethod;
            existingMovement.ConceptId = movement.ConceptId;
            existingMovement.ClientId = movement.ClientId;
            existingMovement.ProviderId = movement.ProviderId;
            existingMovement.EmployeeId = movement.EmployeeId;
            existingMovement.ConstructionId = movement.ConstructionId;
            existingMovement.Date = DateTime.Now;

            return await _repository.Update(existingMovement);
        }


        public Task<Movement> UpdatePartial(Movement movement, int code)
        {
            throw new NotImplementedException();
        }
    }
}
