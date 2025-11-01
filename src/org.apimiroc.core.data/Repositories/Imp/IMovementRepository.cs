using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IMovementRepository
    {
        public Task<Movement> Save(Movement movement);
        public Task<List<Movement>> FindAll();
        public Task<Movement> FindByCode(int code);
        public Task<Movement> FindById(string id);
        public Task<Movement> Update(Movement movement);
        public Task<Movement> UpdatePartial(Movement movement);
    }
}
