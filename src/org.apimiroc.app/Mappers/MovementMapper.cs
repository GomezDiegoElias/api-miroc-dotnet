using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response.Movements;

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

        //public static MovementResponse ToResponse(Movement movement)
        //{

        //    AssociatedEntity associated = null;

        //    if (movement.Client is not null)
        //    {
        //        associated = new AssociatedEntity("Cliente", movement.Client.Id);
        //    }
        //    else if (movement.Provider is not null)
        //    {
        //        associated = new AssociatedEntity("Proveedor", movement.Provider.Id);
        //    }
        //    else if (movement.Employee is not null)
        //    {
        //        associated = new AssociatedEntity("Empleado", movement.Employee.Id);
        //    } else if (movement.Construction is not null)
        //    {
        //        associated = new AssociatedEntity("Obra", movement.Construction.Id);
        //    }

        //    return new MovementResponse(
        //        CodeMovement: movement.CodMovement,
        //        Date: movement.Date,
        //        Amount: movement.Amount,
        //        PaymentMethod: movement.PaymentMethod.ToString(),
        //        ConceptName: movement.Concept.Name,
        //        ConceptType: movement.Concept.type,
        //        ConceptDescription: movement.Concept.Description,
        //        AssociatedEntity: associated
        //    );

        //}

        public static MovementResponse ToResponse(Movement movement)
        {
            AssociatedEntity? associated = null;

            // ⭐ CAMBIAR: Verificar por ID en lugar del objeto completo
            if (!string.IsNullOrEmpty(movement.ClientId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.CLIENTE.ToString(), movement.ClientId);
            }
            else if (!string.IsNullOrEmpty(movement.ProviderId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.PROVEEDOR.ToString(), movement.ProviderId);
            }
            else if (!string.IsNullOrEmpty(movement.EmployeeId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.EMPLEADO.ToString(), movement.EmployeeId);
            }
            else if (!string.IsNullOrEmpty(movement.ConstructionId))
            {
                associated = new AssociatedEntity(AssociatedEntityType.OBRA.ToString(), movement.ConstructionId);
            }

            // ⭐ AGREGAR: Validar que Concept no sea null
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

    }
}
