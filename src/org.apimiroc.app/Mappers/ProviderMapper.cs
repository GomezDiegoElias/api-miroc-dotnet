using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Mappers
{
    public static class ProviderMapper
    {

        public static ProviderResponse ToResponse(Provider provider)
        {
            return new ProviderResponse(provider.Cuit, provider.FirstName, provider.Address, provider.Description);
        }

        public static ProviderRequest ToRequest(Provider provider)
        {
            return new ProviderRequest(provider.Cuit, provider.FirstName, provider.Address, provider.Description);
        }

        public static Provider ToEntityForUpdate(ProviderRequest request, Provider provider)
        {
            return new Provider(provider.Id, request.Cuit, request.FirstName, request.Address, request.Description);
        }

        public static Provider ToEntityForPatch(ProviderRequest request, Provider provider)
        {
            return new Provider(provider.Id, request.Cuit, request.FirstName, request.Address, request.Description);
        }

    }
}
