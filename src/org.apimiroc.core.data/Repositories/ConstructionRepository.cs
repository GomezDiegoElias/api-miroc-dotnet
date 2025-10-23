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
                map: reader => new Construction
                {
                    Id = reader["id"].ToString() ?? string.Empty,
                    Name = reader["name"].ToString() ?? string.Empty,
                    StartDate = Convert.ToDateTime(reader["start_date"]),
                    EndDate = Convert.ToDateTime(reader["end_date"]),
                    Address = reader["address"].ToString() ?? string.Empty,
                    Description = reader["description"].ToString() ?? string.Empty,
                },
                filter: filters,
                extraParams: extraParams
            );

        }

        public async Task<Construction?> FindByName(string name)
        {
            return await _context.Constructions.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<Construction> Save(Construction construction)
        {
            _context.Constructions.Add(construction);
            await _context.SaveChangesAsync();
            return construction;
        }

        public async Task<Construction> Update(Construction construction, string nameOld)
        {
            var existingEntity = await FindByName(nameOld)
                ?? throw new ConstructionNotFoundException(nameOld);

            // Actualizar solo los campos necesarios
            existingEntity.Name = construction.Name;
            existingEntity.StartDate = construction.StartDate;
            existingEntity.EndDate = construction.EndDate;
            existingEntity.Address = construction.Address;
            existingEntity.Description = construction.Description;
            existingEntity.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<Construction> UpdatePartial(Construction construction, string nameOld)
        {

            var existingEntity = await FindByName(nameOld)
                ?? throw new ConstructionNotFoundException(nameOld.ToString());

            existingEntity.Name = construction.Name;
            existingEntity.StartDate = construction.StartDate;
            existingEntity.EndDate = construction.EndDate;
            existingEntity.Address = construction.Address;
            existingEntity.Description = construction.Description;
            existingEntity.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingEntity;

        }
    }
}
