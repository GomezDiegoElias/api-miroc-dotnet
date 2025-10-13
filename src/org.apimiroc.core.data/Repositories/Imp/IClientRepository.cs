using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IClientRepository
    {
        public Task<PaginatedResponse<Client>> FindAll(ClientFilter filters);
        public Task<Client?> FindByDni(long dni);
        public Task<Client> Save(Client client);
        public Task<Client> Update(Client client, long dniOld);
        public Task<Client> DeletePermanent(long dni);
        public Task<Client> DeleteLogic(long dni);
        public Task<Client> UpdatePartial(Client client, long dniOld);
    }
}
