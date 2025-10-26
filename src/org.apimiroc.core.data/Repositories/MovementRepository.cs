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
        public async Task<List<Movement>> FindAll()
        {
            return await _context.Movements
                .Include(m => m.Concept)
                .Include(m => m.Client)
                .Include(m => m.Provider)
                .Include(m => m.Employee)
                .Include(m => m.Construction)
                .ToListAsync();
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
