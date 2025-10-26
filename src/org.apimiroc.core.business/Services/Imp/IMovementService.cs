using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IMovementService
    {
        public Task<Movement> Save(Movement movement);
        public Task<List<Movement>> FindAll();
    }
}
