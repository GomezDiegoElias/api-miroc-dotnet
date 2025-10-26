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

        public async Task<Concept> Save(Concept concept)
        {
            _context.Concepts.Add(concept);
            await _context.SaveChangesAsync();
            return concept;
        }
    }
}
