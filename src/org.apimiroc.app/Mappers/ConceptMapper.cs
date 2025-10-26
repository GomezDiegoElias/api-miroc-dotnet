using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Mappers
{
    public static class ConceptMapper
    {

        public static Concept ToEntity(ConceptRequest request)
        {
            return new Concept
            {
                Name = request.Name,
                type = request.Type,
                Description = request.Description ?? string.Empty
            };
        }

        public static ConceptResponse ToResponse(Concept entity)
        {
            return new ConceptResponse(
                entity.Id,
                entity.Name,
                entity.type,
                entity.Description
            );
        }

    }
}
