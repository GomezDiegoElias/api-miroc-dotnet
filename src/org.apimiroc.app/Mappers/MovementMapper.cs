using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response.Movements;
using org.apimiroc.core.shared.Dto.Response.Movements.V2;

namespace org.apimiroc.app.Mappers
{
    public static class MovementMapper
    {

        // mappers para la version 1 con relaciones por claves unicas
        // Mapper para convertir Movement a MovementRequest (V1)
        public static MovementRequest ToRequest(Movement movement)
        {
            long? clientDni = null;
            long? providerCuit = null;
            long? employeeDni = null;
            string? constructionName = null;

            if (!string.IsNullOrEmpty(movement.ClientId) && movement.Client != null)
            {
                clientDni = movement.Client.Dni;
            }
            else if (!string.IsNullOrEmpty(movement.ProviderId) && movement.Provider != null)
            {
                providerCuit = movement.Provider.Cuit;
            }
            else if (!string.IsNullOrEmpty(movement.EmployeeId) && movement.Employee != null)
            {
                employeeDni = movement.Employee.Dni;
            }
            else if (!string.IsNullOrEmpty(movement.ConstructionId) && movement.Construction != null)
            {
                constructionName = movement.Construction.Name;
            }

            return new MovementRequest(
                movement.Amount,
                movement.PaymentMethod.ToString(),
                movement.ConceptId,
                clientDni,
                providerCuit,
                employeeDni,
                constructionName
            );
        }

        public static MovementResponse ToResponse(Movement movement)
        {
            AssociatedEntity? associated = null;

            if (!string.IsNullOrEmpty(movement.ClientId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.CLIENTE.ToString(), movement.Client!.Dni);
            }
            else if (!string.IsNullOrEmpty(movement.ProviderId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.PROVEEDOR.ToString(), movement.Provider!.Cuit);
            }
            else if (!string.IsNullOrEmpty(movement.EmployeeId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.EMPLEADO.ToString(), movement.Employee!.Dni);
            }
            else if (!string.IsNullOrEmpty(movement.ConstructionId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.OBRA.ToString(), movement.Construction!.Name);
            }

            if (movement.Concept == null)
            {
                throw new InvalidOperationException($"Movement {movement.CodMovement} no tiene un Concept asociado");
            }

            return new MovementResponse(
                CodeMovement: movement.CodMovement,
                Date: movement.Date,
                Amount: movement.Amount,
                PaymentMethod: movement.PaymentMethod.ToString(),
                ConceptName: movement.Concept.Name,
                ConceptType: movement.Concept.type,
                ConceptDescription: movement.Concept.Description,
                AssociatedEntity: associated
            );
        }

        // mappers para la version 2 con relaciones por Ids
        public static Movement ToEntityV2(MovementRequestV2 request)
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

        public static MovementResponseV2 ToResponseV2(Movement movement)
        {
            AssociatedEntityV2? associated = null;

            if (!string.IsNullOrEmpty(movement.ClientId))
            {
                associated = new AssociatedEntityV2(AssociatedEntityType.CLIENTE.ToString(), movement.ClientId);
            }
            else if (!string.IsNullOrEmpty(movement.ProviderId))
            {
                associated = new AssociatedEntityV2(AssociatedEntityType.PROVEEDOR.ToString(), movement.ProviderId);
            }
            else if (!string.IsNullOrEmpty(movement.EmployeeId))
            {
                associated = new AssociatedEntityV2(AssociatedEntityType.EMPLEADO.ToString(), movement.EmployeeId);
            }
            else if (!string.IsNullOrEmpty(movement.ConstructionId))
            {
                associated = new AssociatedEntityV2(AssociatedEntityType.OBRA.ToString(), movement.ConstructionId);
            }

            if (movement.Concept == null)
            {
                throw new InvalidOperationException($"Movement {movement.CodMovement} no tiene un Concept asociado");
            }

            return new MovementResponseV2(
                CodeMovement: movement.CodMovement,
                Date: movement.Date,
                Amount: movement.Amount,
                PaymentMethod: movement.PaymentMethod.ToString(),
                ConceptName: movement.Concept.Name,
                ConceptType: movement.Concept.type,
                ConceptDescription: movement.Concept.Description,
                AssociatedEntity: associated
            );
        }

        public static MovementRequestV2 ToRequestV2(Movement movement)
        {
            return new MovementRequestV2(
                movement.Amount,
                movement.PaymentMethod.ToString(),
                movement.ConceptId,
                movement.ClientId,
                movement.ProviderId,
                movement.EmployeeId,
                movement.ConstructionId
            );
        }

        public static Movement ToEntityForPatchV2(MovementRequestV2 request, Movement existingMovement)
        {
            return new Movement
            {
                Id = existingMovement.Id,
                CodMovement = existingMovement.CodMovement,
                Amount = request.Amount,
                Date = DateTime.Now,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod),
                ConceptId = request.ConceptId,
                ClientId = request.ClientId,
                ProviderId = request.ProviderId,
                EmployeeId = request.EmployeeId,
                ConstructionId = request.ConstructionId,
                IsDeleted = existingMovement.IsDeleted
            };
        }

    }
}
