using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories
{
    public class ConceptRepository : IConceptRepository
    {

        private readonly AppDbContext _context;
        private readonly IPaginationRepository _paginationRepository;

        public ConceptRepository(AppDbContext context, IPaginationRepository paginationRepository)
        {
            _context = context;
            _paginationRepository = paginationRepository;
        }

        public async Task Delete(Concept concept)
        {
            concept.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        //public async Task DeletePermanent(Concept concept)
        //{
        //    _context.Concepts.Remove(concept);
        //    await _context.SaveChangesAsync();
        //}

        public async Task<PaginatedResponse<Concept>> FindAll(ConceptFilter filters)
        {

            var extraParams = new Dictionary<string, object?>
            {
                { "Q", filters.Q },
                { "FName", filters.FName },
                { "FType", filters.FType } 
            };

            return await _paginationRepository.ExecutePaginationAsync(
                storedProcedure: "getConceptPaginationAdvanced",
                map: reader => new Concept
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString() ?? string.Empty,
                    type = reader["type"].ToString() ?? string.Empty,
                    Description = reader["description"].ToString() ?? string.Empty
                },
                filter: filters,
                extraParams: extraParams
            );

        }

        public async Task<Concept?> FindById(int id)
        {
            return await _context.Concepts.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Concept?> FindByName(string name)
        {
            return await _context.Concepts.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Concept?> FindByType(string name)
        {
            return await _context.Concepts.FirstOrDefaultAsync(c => c.type == name);
        }

        public async Task<Concept> Save(Concept concept)
        {
            _context.Concepts.Add(concept);
            await _context.SaveChangesAsync();
            return concept;
        }

        public async Task<Concept> Update(Concept concept)
        {
            concept.Name = concept.Name;
            concept.type = concept.type;
            concept.Description = concept.Description;
            await _context.SaveChangesAsync();
            return concept;
        }

        //public Task<Concept> UpdatePartial(Concept concept)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
