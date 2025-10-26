using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Mappers
{
    public static class MovementMapper
    {

        public static Movement ToEntity(MovementRequest request)
        {
            return new Movement
            {
                Id = Movement.GenerateId(),
                Amount = request.Amount,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod),
                ConceptId = request.ConceptId,
                ClientId = request.ClientId,
                ProviderId = request.ProviderId,
                EmployeeId = request.EmployeeId,
                ConstructionId = request.ConstructionId
            };
        }

        public static MovementResponse ToResponse(Movement movement)
        {
            return new MovementResponse(
                movement.CodMovement,
                movement.Date,
                movement.Amount,
                movement.PaymentMethod.ToString(),
                movement.Concept.Name
            );
        }

        //public static MovementRequest ToRequest(Movement movement)
        //{
        //    return new MovementRequest(
        //        movement.Amount,
        //        movement.PaymentMethod.ToString(),
        //        movement.ConceptId,
        //        movement.ClientId!,
        //        movement.ProviderId!,
        //        movement.EmployeeId!,
        //        movement.ConstructionId!
        //    );
        //}

    }
}
