using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IClientService
    {
        public Task<PaginatedResponse<Client>> FindAll(int pageIndex, int pageSize);
        public Task<Client?> FindByDni(long dni);
        public Task<Client> Save(ClientRequest request);
        public Task<Client> Update(Client client, long dniOld);
        public Task<Client> DeletePermanent(long dni);
        public Task<Client> DeleteLogic(long dni);
        public Task<Client> UpdatePartial(Client client, long dniOld);
    }
}
