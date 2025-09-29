using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services
{
    public class ProviderService : IProviderService
    {

        private readonly IProviderRepository _providerRepository;

        public ProviderService(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public async Task<Provider> DeleteLogic(long dni)
        {
            return await _providerRepository.DeleteLogic(dni);
        }

        public async Task<Provider> DeletePermanent(long dni)
        {
            return await _providerRepository.DeletePermanent(dni);
        }

        public async Task<PaginatedResponse<Provider>> FindAll(int pageIndex, int pageSize)
        {
            return await _providerRepository.FindAll(pageIndex, pageSize);
        }

        public async Task<Provider?> FindByCuit(long cuit)
        {
            return await _providerRepository.FindByCuit(cuit)
                ?? throw new ProviderNotFoundException(cuit.ToString());
        }

        public async Task<Provider> Save(ProviderRequest request)
        {

            // Validaciones

            var newProvider = new Provider
            {
                Id = Provider.GenerateId(),
                Cuit = request.Cuit,
                FirstName = request.FirstName,
                Address = request.Address,
                Description = request.Description
            };

            var saveProvider = await _providerRepository.Save(newProvider);

            return saveProvider;

        }

        public async Task<Provider> Update(Provider provider, long cuitLong)
        {
            return await _providerRepository.Update(provider, cuitLong);
        }

        public async Task<Provider> UpdatePartial(Provider provider, long cuitLong)
        {
            return await _providerRepository.UpdatePartial(provider, cuitLong);
        }
    }
}
