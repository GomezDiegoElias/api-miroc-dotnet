using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories
{
    public class MovementRepository : IMovementRepository
    {

        private readonly AppDbContext _context;
        private readonly IPaginationRepository _paginationRepository;

        public MovementRepository(AppDbContext context, IPaginationRepository paginationRepository)
        {
            _context = context;
            _paginationRepository = paginationRepository;
        }

        // Obtener todos los movimientos, incluyendo los eliminados logicamente
        // en caso de que se quiera excluirlos, se deberia eliminar el .IgnoreQueryFilters()
        // tiene sentido si para historial contable o auditoria
        public async Task<PaginatedResponse<Movement>> FindAll(MovementFilter filters)
        {

            var extraParams = new Dictionary<string, object?>
            {
                { "Q", filters.Q  },
                { "FDateFrom", filters.FDateFrom },
                { "FDateTo", filters.FDateTo },
                { "FPaymentMethod", filters.FPaymentMethod },
                { "FCode", filters.FCode }
            };

            return await _paginationRepository.ExecutePaginationAsync(
                storedProcedure: "getMovementPaginationAdvanced",
                map: reader => new Movement
                {
                    CodMovement = Convert.ToInt32(reader["cod_movement"]),
                    Amount = Convert.ToDecimal(reader["amount"]),
                    Date = Convert.ToDateTime(reader["date"]),
                    PaymentMethod = Enum.Parse<PaymentMethod>(reader["payment_method"].ToString() ?? "CASH"),
                    ConceptId = Convert.ToInt32(reader["concept_id"]),
                    ClientId = reader["client_id"]?.ToString(),
                    ProviderId = reader["provider_id"]?.ToString(),
                    EmployeeId = reader["employee_id"]?.ToString(),
                    ConstructionId = reader["construction_id"]?.ToString(),
                    Concept = new Concept
                    {
                        Id = Convert.ToInt32(reader["concept_id"]),
                        Name = reader["concept_name"].ToString() ?? string.Empty,
                        type = reader["concept_type"].ToString() ?? string.Empty,
                        Description = reader["concept_description"].ToString() ?? string.Empty
                    }
                },
                filter: filters,
                extraParams: extraParams!
            );

        }

        // Obtener un movimiento por su codigo junto con la informacion completa de su relacion
        public async Task<Movement> FindByCode(int code)
        {
            var movement = await _context.Movements
                .Include(m => m.Concept)
                .Include(m => m.Client)
                .Include(m => m.Provider)
                .Include(m => m.Employee)
                .Include(m => m.Construction)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.CodMovement == code);
            return movement!;
        }

        public async Task<Movement> FindById(string id)
        {
            var movement = await _context.Movements
                .Include(m => m.Concept)
                .Include(m => m.Client)
                .Include(m => m.Provider)
                .Include(m => m.Employee)
                .Include(m => m.Construction)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id);
            return movement!;
        }

        public async Task<Movement> Save(Movement movement)
        {
            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();
            //return movement;
            return await _context.Movements
                .Include(m => m.Concept)
                .Include(m => m.Client)
                .Include(m => m.Provider)
                .Include(m => m.Employee)
                .Include(m => m.Construction)
                .FirstAsync(m => m.Id == movement.Id);
        }

        public async Task<Movement> Update(Movement movement)
        {
            await _context.SaveChangesAsync();

            return await _context.Movements
                .Include(m => m.Concept)
                .Include(m => m.Client)
                .Include(m => m.Provider)
                .Include(m => m.Employee)
                .Include(m => m.Construction)
                .FirstAsync(m => m.Id == movement.Id);
        }


        public Task<Movement> UpdatePartial(Movement movement)
        {
            throw new NotImplementedException();
        }
    }
}
