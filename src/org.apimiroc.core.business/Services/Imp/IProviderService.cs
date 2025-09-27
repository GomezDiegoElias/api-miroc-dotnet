using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IProviderService
    {
        public Task<PaginatedResponse<Provider>> FindAll(int pageIndex, int pageSize);
        public Task<Provider?> FindByCuit(long cuit);
        public Task<Provider> Save(ProviderRequest request);
        public Task<Provider> Update(Provider provider);
        public Task<Provider> DeletePermanent(long cuit);
        public Task<Provider> DeleteLogic(long cuit);
        public Task<Provider> UpdatePartial(Provider provider);
    }
}
