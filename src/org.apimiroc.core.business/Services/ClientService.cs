using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
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

        public async Task<PaginatedResponse<Client>> FindAll(ClientFilter filters)
        {
            return await _clientRepository.FindAll(filters);
        }

        public async Task<Client?> FindByDni(long dni)
        {
            return await _clientRepository.FindByDni(dni)
                ?? throw new ClientNotFoundException(dni.ToString());
        }

        public async Task<Client?> FindById(string id)
        {
            return await _clientRepository.FindById(id) 
                ?? throw new ClientNotFoundException(id);
        }

        public async Task<Client> Save(Client client)
        {

            // Validaciones
            if (await _clientRepository.ExistDni(client.Dni)) throw new ClientNotFoundException($"El DNI ingresado {client.Dni} ya existe");

            return await _clientRepository.Save(client);

        }

        public async Task<Client> Update(Client client, long dniOld)
        {
            return await _clientRepository.Update(client, dniOld);
        }

        public async Task<Client> UpdatePartial(Client client, long dniOld)
        {
            return await _clientRepository.UpdatePartial(client, dniOld);
        }
    }
}
