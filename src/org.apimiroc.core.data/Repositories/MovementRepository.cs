using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.core.data.Repositories
{
    public class MovementRepository : IMovementRepository
    {

        private readonly AppDbContext _context;

        public MovementRepository(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los movimientos, incluyendo los eliminados logicamente
        // en caso de que se quiera excluirlos, se deberia eliminar el .IgnoreQueryFilters()
        // tiene sentido si para historial contable o auditoria
        public async Task<List<Movement>> FindAll()
        {
            return await _context.Movements
                .Include(m => m.Concept)
                .Include(m => m.Client)
                .Include(m => m.Provider)
                .Include(m => m.Employee)
                .Include(m => m.Construction)
                .IgnoreQueryFilters() // ignora todos los filtros de cualquier entidad independiente
                .ToListAsync();
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
                .FirstOrDefaultAsync(m => m.CodMovement == code);
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
    }
}
