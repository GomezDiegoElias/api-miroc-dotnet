using Microsoft.Extensions.Logging;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services
{
    public class MovementService : IMovementService
    {

        private readonly ILogger<MovementService> _logger;

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
            IConstructionRepository constructionRepository,
            ILogger<MovementService> logger
        )
        {
            _repository = repository;
            _conceptRepository = conceptRepository;
            _clientRepository = clientRepository;
            _providerRepository = providerRepository;
            _employeeRepository = employeeRepository;
            _constructionRepository = constructionRepository;
            _logger = logger;
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

        public async Task<PaginatedResponse<Movement>> FindAllV2(MovementFilter filters)
        {
            return await _repository.FindAllV2(filters);
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

        public async Task<Movement> Save(MovementRequest request)
        {

            _logger.LogInformation("Intentando guardar nuevo movimiento");

            _logger.LogInformation("Validando entidades relacionadas para movimiento...");

            Client? client = null;
            Provider? provider = null;
            Employee? employee = null;
            Construction? construction = null;

            if (request.ClientDni != null)
            {
                _logger.LogInformation("Buscando cliente con DNI {ClientDni}", request.ClientDni);
                client = await _clientRepository.FindByDni(request.ClientDni.Value);
                if (client == null)
                {
                    _logger.LogWarning("Cliente con DNI {ClientDni} no encontrado", request.ClientDni);
                    throw new ClientNotFoundException($"Cliente con DNI {request.ClientDni} no existe");
                }
            }

            if (request.ProviderCuit != null)
            {
                _logger.LogInformation("Buscando proveedor con CUIT {ProviderCuit}", request.ProviderCuit);
                provider = await _providerRepository.FindByCuit(request.ProviderCuit.Value);
                if (provider == null)
                {
                    _logger.LogWarning("Proveedor con CUIT {ProviderCuit} no encontrado", request.ProviderCuit);
                    throw new ProviderNotFoundException($"Proveedor con CUIT {request.ProviderCuit} no existe");
                }
            }

            if (request.EmployeeDni != null)
            {
                _logger.LogInformation("Buscando empleado con DNI {EmployeeDni}", request.EmployeeDni);
                employee =  await _employeeRepository.FindByDni(request.EmployeeDni.Value);
                if (employee == null)
                {
                    _logger.LogWarning("Empleado con DNI {EmployeeDni} no encontrado", request.EmployeeDni);
                    throw new EmployeeNotFoundException($"Empleado con DNI {request.EmployeeDni} no existe");
                }
            }

             if (!string.IsNullOrEmpty(request.ConstructionName)) 
             {
                construction = await _constructionRepository.FindByName(request.ConstructionName);
                if (construction == null)
                {
                    throw new ConstructionNotFoundException($"Obra con nombre {request.ConstructionName} no existe");
                }
             }
             
            var movement = new Movement
            { 
                Id = Movement.GenerateId(),
                Amount = request.Amount,
                Date = DateTime.Now,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod),
                ConceptId = request.ConceptId,
                ClientId = client?.Id,
                ProviderId = provider?.Id,
                EmployeeId = employee?.Id,
                ConstructionId = construction?.Id,
            };

            return await _repository.Save(movement);

        }

        // ░░░░░░░░░░░░░░░░░░░░░░░░░░ Version 2 - relaciones con id ░░░░░░░░░░░░░░░░░░░░░░░░░░
        public async Task<Movement> SaveV2(Movement movement)
        {

            // Uso de logs
            _logger.LogInformation("Intentando guardar nuevo movimiento");

            _logger.LogInformation("Validando entidades relacionadas para movimiento...");

            // Validaciones
            // validacion de concept id si ejecuta el await por que no permite nulo para la creacion del movimiento
            if (await _conceptRepository.FindById(movement.ConceptId) == null)
            {
                _logger.LogWarning("Concepto con ID {ConceptId} no encontrado", movement.ConceptId);
                throw new ConceptNotFoundException($"Concepto con ID {movement.ConceptId} no existe");
            }

            // validacion de relaciones que permiten nulos
            await ValidateMovement(movement);

            var result = await _repository.Save(movement);

            _logger.LogInformation("Movimiento guardado exitosamente con ID {Id}", result.Id);

            return result;
        
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


        // ░░░░░░░░░░░░░░░░░░░░░░░░░░ Validacion usada solo en la version 2 - relacion con id ░░░░░░░░░░░░░░░░░░░░░░░░░░
        // validacion de relaciones que permiten nulos
        // solo ejecuta el await si el id no es nulo, para evitar consultas innecesarias a la base de datos
        private async Task ValidateMovement(Movement movement)
        {
            // Client
            if (movement.ClientId != null)
            {
                _logger.LogInformation("Validando existencia del cliente con ID {ClientId}", movement.ClientId);
                if (await _clientRepository.FindById(movement.ClientId) == null)
                {
                    _logger.LogWarning("Cliente con ID {ClientId} no encontrado", movement.ClientId);
                    throw new ClientNotFoundException($"Cliente con ID {movement.ClientId} no existe");
                }
            }

            // Employee
            if (movement.EmployeeId != null)
            {
                _logger.LogInformation("Validando existencia del empleado con ID {EmployeeId}", movement.EmployeeId);
                if (await _employeeRepository.FindById(movement.EmployeeId) == null)
                {
                    _logger.LogWarning("Empleado con ID {EmployeeId} no encontrado", movement.EmployeeId);
                    throw new EmployeeNotFoundException($"Empleado con ID {movement.EmployeeId} no existe");
                }
            }

            // Provider
            if (movement.ProviderId != null)
            {
                _logger.LogInformation("Validando existencia del proveedor con ID {ProviderId}", movement.ProviderId);
                if (await _providerRepository.FindById(movement.ProviderId) == null)
                {
                    _logger.LogWarning("Proveedor con ID {ProviderId} no encontrado", movement.ProviderId);
                    throw new ProviderNotFoundException($"Proveedor con ID {movement.ProviderId} no existe");
                }
            }
            
            // Construction
            if (movement.ConstructionId != null)
            {
                _logger.LogInformation("Validando existencia de la obra con ID {ConstructionId}", movement.ConstructionId);
                if (await _constructionRepository.FindById(movement.ConstructionId) == null)
                {
                    _logger.LogWarning("Construcción con ID {ConstructionId} no encontrado", movement.ConstructionId);
                    throw new ConstructionNotFoundException($"Obra con ID {movement.ConstructionId} no existe");
                }
            }
        }

    }
}
