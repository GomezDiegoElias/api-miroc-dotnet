using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;

namespace org.apimiroc.core.business.Services
{
    public class MovementService : IMovementService
    {

        private readonly IMovementRepository _repository;

        public MovementService(IMovementRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Movement>> FindAll()
        {
            return await _repository.FindAll();
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
