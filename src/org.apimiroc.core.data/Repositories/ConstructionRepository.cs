using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories
{
    // Solo implementa IConstructionRepository
    public class ConstructionRepository : IConstructionRepository
    {

        private readonly AppDbContext _context;
        private readonly IPaginationRepository _paginationRepository;

        public ConstructionRepository(AppDbContext context, IPaginationRepository paginationRepository)
        {
            _context = context;
            _paginationRepository = paginationRepository;
        }

        public async Task<Construction> DeleteLogic(string name)
        {

            var entity = await _context.Constructions.FirstOrDefaultAsync(x => x.Name == name);
            if (entity == null) throw new ConstructionNotFoundException($"No se encontró construcción con nombre {name}");

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();

            return entity;

        }

        public async Task<Construction> DeletePermanent(string name)
        {

            var entity = _context.Constructions.FirstOrDefault(x => x.Name == name);

            if (entity == null) throw new ConstructionNotFoundException($"No se encontró construcción llamada {name}");

            _context.Constructions.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;

        }



        public async Task<PaginatedResponse<Construction>> FindAll(ConstructionFilter filters)
        {

            // Parametros adicionales (filtros)
            var extraParams = new Dictionary<string, object?>
            {
                { "@Q", filters.Q },
                { "@FName", filters.FName },
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
