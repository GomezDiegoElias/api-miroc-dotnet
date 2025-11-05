using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IProviderService
    {
        public Task<PaginatedResponse<Provider>> FindAll(ProviderFilter filters);
        public Task<Provider?> FindByCuit(long cuit);
        public Task<Provider?> FindById(string id);
        public Task<Provider> Save(Provider provider);
        public Task<Provider> Update(Provider provider, long cuitOld);
        public Task<Provider> DeletePermanent(long cuit);
        public Task<Provider> DeleteLogic(long cuit);
        public Task<Provider> UpdatePartial(Provider provider, long cuitLong);
    }
}
