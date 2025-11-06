using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Mappers
{
    public static class ConstructionMapper
    {

        public static ConstructionResponse ToResponse(Construction construction)
        {
            return new ConstructionResponse(
                construction.Name,
                construction.StartDate,
                construction.EndDate,
                construction.Address,
                construction.Description,
                construction.Client.Dni
            );
        }

        public static ConstructionRequest ToRequest(Construction construction)
        {
            return new ConstructionRequest(
                construction.Name,
                construction.StartDate,
                construction.EndDate,
                construction.Address,
                construction.Description,
                construction.Client.Dni
            );
        }

        public static ConstructionResponseV2 ToResponseV2(Construction construction)
        {
            return new ConstructionResponseV2(
                construction.Name,
                construction.StartDate,
                construction.EndDate,
                construction.Address,
                construction.Description,
                construction.ClientId
            );
        }

        public static ConstructionRequestV2 ToRequestV2(Construction construction)
        {
            return new ConstructionRequestV2(
                construction.Name,
                construction.StartDate,
                construction.EndDate,
                construction.Address,
                construction.Description,
                construction.ClientId
            );
        }

        public static Construction ToEntityForUpdate(ConstructionRequest request, Construction construction, string clientId)
        {
            return new Construction(
                construction.Id,
                request.Name,
                request.StartDate,
                request.EndDate,
                request.Address,
                request.Description,
                clientId
            );
        }

        public static Construction ToEntityForPatch(ConstructionRequest request, Construction construction, string clientId)
        {
            return new Construction(
                construction.Id,
                request.Name,
                request.StartDate,
                request.EndDate,
                request.Address,
                request.Description,
                clientId
            );
        }

        public static Construction ToEntityForUpdateV2(ConstructionRequestV2 request, Construction construction)
        {
            return new Construction(
                construction.Id,
                request.Name,
                request.StartDate,
                request.EndDate,
                request.Address,
                request.Description,
                request.ClientId
            );
        }

        public static Construction ToEntityForPatchV2(ConstructionRequestV2 request, Construction construction)
        {
            return new Construction(
                construction.Id,
                request.Name,
                request.StartDate,
                request.EndDate,
                request.Address,
                request.Description,
                request.ClientId
            );
        }

    }
}
