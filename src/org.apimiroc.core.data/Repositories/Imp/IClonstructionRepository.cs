using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IConstructionRepository
    {
        public Task<PaginatedResponse<Construction>> FindAll(ConstructionFilter filters);
        public Task<Construction?> FindById(long id);
        public Task<Client> Save(Client client);
        public Task<Client> Update(Client client, long dniOld);
        public Task<Client> DeletePermanent(long dni);
        public Task<Client> DeleteLogic(long dni);
        public Task<Client> UpdatePartial(Client client, long dniOld);
    }
}
