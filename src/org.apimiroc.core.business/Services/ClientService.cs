﻿using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services
{
    public class ClientService : IClientService
    {

        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client> DeleteLogic(long dni)
        {
            return await _clientRepository.DeleteLogic(dni);
        }

        public async Task<Client> DeletePermanent(long dni)
        {
            return await _clientRepository.DeletePermanent(dni);
        }

        public async Task<PaginatedResponse<Client>> FindAll(int pageIndex, int pageSize)
        {
            return await _clientRepository.FindAll(pageIndex, pageSize);
        }

        public async Task<Client?> FindByDni(long dni)
        {
            return await _clientRepository.FindByDni(dni)
                ?? throw new ClientNotFoundException(dni.ToString());
        }

        public async Task<Client> Save(ClientRequest request)
        {

            // Validaciones

            var newClient = new Client
            {
                Id = Client.GenerateId(),
                Dni = request.Dni,
                FirstName = request.FirstName,
                Address = request.Address
            };

            var saveClient = await _clientRepository.Save(newClient);

            return saveClient;

        }

        public async Task<Client> Update(Client client)
        {
            return await _clientRepository.Update(client);
        }

        public async Task<Client> UpdatePartial(Client client)
        {
            return await _clientRepository.UpdatePartial(client);
        }
    }
}
