using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories
{
    // Solo implementa IProviderRepository
    public class ProviderRepository : IProviderRepository
    {

        private readonly AppDbContext _context;
        private readonly IPaginationRepository _paginationRepository;

        public ProviderRepository(AppDbContext context, IPaginationRepository paginationRepository)
        {
            _context = context;
            _paginationRepository = paginationRepository;
        }

        public async Task<Provider> DeleteLogic(long cuit)
        {

            var entity = await _context.Providers.FirstOrDefaultAsync(x => x.Cuit == cuit);
            if (entity == null) throw new ProviderNotFoundException($"No se encontró proveedor con CUIT {cuit}");

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();

            return entity;

        }

        public async Task<Provider> DeletePermanent(long cuit)
        {

            var entity = _context.Providers.FirstOrDefault(x => x.Cuit == cuit);

            if (entity == null) throw new ProviderNotFoundException($"No se encontró proveedor con CUIT {cuit   }");

            _context.Providers.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;

        }

        public async Task<bool> ExistCuit(long cuit)
        {
            return await _context.Providers.AnyAsync(x => x.Cuit == cuit);
        }

        // cuit, first_name, address

        public async Task<PaginatedResponse<Provider>> FindAll(ProviderFilter filters)
        {

            // Parametros adicionales (filtros)
            var extraParams = new Dictionary<string, object?>
            {
                { "@Q", filters.Q },
                { "@FCuit", filters.FCuit },
                { "@FFirstName", filters.FFirstName },
                { "@FAddress", filters.FAddress }
            };

            return await _paginationRepository.ExecutePaginationAsync(
                storedProcedure: "getProviderPaginationAdvanced",
                map: reader => new Provider
                {
                    Id = reader["id"].ToString() ?? string.Empty,
                    Cuit = Convert.ToInt64(reader["cuit"]),
                    FirstName = reader["first_name"].ToString() ?? string.Empty,
                    Address = reader["address"].ToString() ?? string.Empty,
                    Description = reader["description"].ToString() ?? string.Empty,
                },
                filter: filters,
                extraParams: extraParams
            );

        }

        public async Task<Provider?> FindByCuit(long cuit)
        {
            return await _context.Providers.FirstOrDefaultAsync(x => x.Cuit == cuit);
        }

        public async Task<Provider?> FindById(string id)
        {
            return await _context.Providers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Provider> Save(Provider provider)
        {
            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();
            return provider;
        }

        public async Task<Provider> Update(Provider provider, long cuitOld)
        {
            var existingEntity = await FindByCuit(cuitOld)
                ?? throw new ProviderNotFoundException(cuitOld.ToString());

            // Actualizar solo los campos necesarios
            existingEntity.Cuit = provider.Cuit;
            existingEntity.FirstName = provider.FirstName;
            existingEntity.Address = provider.Address;
            existingEntity.Description = provider.Description;
            existingEntity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<Provider> UpdatePartial(Provider provider, long cuitOld)
        {

            var existingEntity = await FindByCuit(cuitOld)
                ?? throw new ProviderNotFoundException(cuitOld.ToString());

            existingEntity.Cuit = provider.Cuit;
            existingEntity.FirstName = provider.FirstName;
            existingEntity.Address = provider.Address;
            existingEntity.Description = provider.Description;
            existingEntity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingEntity;

        }
    }
}
