using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;

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
        public async Task<Movement> Save(Movement movement)
        {
            return await _repository.Save(movement);
        }
    }
}
