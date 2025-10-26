using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IMovementRepository
    {
        public Task<Movement> Save(Movement movement);
        public Task<List<Movement>> FindAll();
    }
}
