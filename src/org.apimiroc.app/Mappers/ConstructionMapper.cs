using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Mappers
{
    public static class ConstructionMapper
    {

        public static ConstructionResponse ToResponse(Construction construction)
        {
            return new ConstructionResponse(construction.Name, construction.StartDate, construction.EndDate, construction.Address, construction.Description);
        }

        public static ConstructionRequest ToRequest(Construction construction)
        {
            return new ConstructionRequest(construction.Name, construction.StartDate, construction.EndDate, construction.Address, construction.Description);
        }

        public static Construction ToEntityForUpdate(ConstructionRequest request, Construction construction)
        {
            return new Construction(construction.Id, request.Name, request.StartDate, request.EndDate, request.Address, request.Description);
        }

        public static Construction ToEntityForPatch(ConstructionRequest request, Construction construction)
        {
            return new Construction(construction.Id, request.Name, request.StartDate, request.EndDate, request.Address, request.Description);
        }

    }
}
