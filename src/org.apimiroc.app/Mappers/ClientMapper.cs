using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;
using org.apimiroc.core.shared.Dto.Response.Movements;

namespace org.apimiroc.app.Mappers
{
    public static class ClientMapper
    {

        public static ClientResponse ToResponse(Client client)
        {
            return new ClientResponse(client.Dni, client.FirstName, client.Address);
        }

        public static ClientRequest ToRequest(Client client)
        {
            return new ClientRequest(client.Dni, client.FirstName, client.Address);
        }

        public static Client ToEntityForUpdate(ClientRequest request, Client client)
        {
            return new Client(client.Id, request.Dni, request.FirstName, request.Address);
        }

        public static Client ToEntityForPatch(ClientRequest request, Client client)
        {
            return new Client(client.Id, request.Dni, request.FirstName, request.Address);
        }
        
        //public static ClientMovementResponse ToMovementResponse (Movement movement)
        //{
        //    return new ClientMovementResponse(
        //        CodeMovement: movement.CodMovement,
        //        Date: movement.Date,
        //        Amount: movement.Amount,
        //        PaymentMethod: movement.PaymentMethod.ToString(),
        //        ConceptName: movement.Concept?.Name ?? "Concepto eliminado",
        //        Dni: movement.Client?.Dni ?? 0,
        //        FullName: movement.Client?.FirstName is null ? "Sin cliente" : $"{movement.Client.FirstName}"
        //    );
        //}

    }
}
