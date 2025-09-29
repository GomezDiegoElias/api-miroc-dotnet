using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories
{
    public class ClientRepository : IClientRepository
    {

        private readonly AppDbContext _context;
        private readonly IPaginationRepository _paginationRepository;

        public ClientRepository(AppDbContext context, IPaginationRepository paginationRepository)
        {
            _context = context;
            _paginationRepository = paginationRepository;
        }

        public async Task<Client?> FindByDni(long dni)
        {
            return await _context.Clients.FirstOrDefaultAsync(x => x.Dni == dni);
        }

        public async Task<PaginatedResponse<Client>> FindAll(int pageIndex, int pageSize)
        {
            return await _paginationRepository.ExecutePaginationAsync(
                "getClientPagination",
                reader => new Client
                {
                    Id = reader["id"].ToString() ?? string.Empty,
                    Dni = Convert.ToInt64(reader["dni"]),
                    FirstName = reader["first_name"].ToString() ?? string.Empty,
                    Address = reader["address"].ToString() ?? string.Empty
                },
                pageIndex,
                pageSize
            );
        }

        public async Task<Client> DeleteLogic(long dni)
        {

            var entity = await _context.Clients.FirstOrDefaultAsync(x => x.Dni == dni);
            if (entity == null) throw new ClientNotFoundException($"No se encontró cliente con DNI {dni}");

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();

            return entity;

        }

        public async Task<Client> DeletePermanent(long dni)
        {

            var entity = _context.Clients.FirstOrDefault(x => x.Dni == dni);

            if (entity == null) throw new ClientNotFoundException($"No se encontró cliente con DNI {dni}");

            _context.Clients.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;

        }

        public async Task<Client> Save(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Client> Update(Client client, long dniOld)
        {
            var existingEntity = await FindByDni(dniOld)
                ?? throw new ClientNotFoundException(dniOld.ToString());

            // Actualizar solo los campos necesarios
            existingEntity.Dni = client.Dni;
            existingEntity.FirstName = client.FirstName;
            existingEntity.Address = client.Address;
            existingEntity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<Client> UpdatePartial(Client client, long dniOld)
        {

            var existingEntity = await FindByDni(dniOld)
                ?? throw new ClientNotFoundException(dniOld.ToString());

            existingEntity.Dni = client.Dni;
            existingEntity.FirstName = client.FirstName;
            existingEntity.Address = client.Address;
            existingEntity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingEntity;

        }
    }
}
