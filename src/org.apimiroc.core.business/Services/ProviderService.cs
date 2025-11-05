using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
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

        public async Task<PaginatedResponse<Provider>> FindAll(ProviderFilter filters)
        {
            return await _providerRepository.FindAll(filters);
        }

        public async Task<Provider?> FindByCuit(long cuit)
        {
            return await _providerRepository.FindByCuit(cuit)
                ?? throw new ProviderNotFoundException(cuit);
        }

        public async Task<Provider?> FindById(string id)
        {
            return await _providerRepository.FindById(id);
        }

        public async Task<Provider> Save(Provider provider)
        {
            // Validaciones
            if (await _providerRepository.ExistCuit(provider.Cuit)) throw new ProviderNotFoundException($"El CUIT ingresado {provider.Cuit} ya existe");
            return await _providerRepository.Save(provider);
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
