﻿using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

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

    }
}
