using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private readonly AppDbContext _context;
        private readonly IPaginationRepository _paginationRepository;

        public EmployeeRepository(AppDbContext context, IPaginationRepository paginationRepository)
        {
            _context = context;
            _paginationRepository = paginationRepository;
        }

        public async Task<Employee> DeleteLogic(long dni)
        {
            var existingEntity = _context.Employees.FirstOrDefault(e => e.Dni == dni);
            existingEntity!.IsDeleted = true;
            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<Employee> DeletePermanent(long dni)
        {
            var existingEntity = await _context.Employees.FirstOrDefaultAsync(e => e.Dni == dni);
            _context.Employees.Remove(existingEntity!);
            await _context.SaveChangesAsync();
            return existingEntity!;
        }

        public async Task<PaginatedResponse<Employee>> FindAll(EmployeeFilter filters)
        {

            // Parametros adicionales (filtros)
            var extraParams = new Dictionary<string, object?>
            {
                { "@Q", filters.Q  },
                { "@FDni", filters.FDni },
                { "@FFirstName", filters.FFirstName },
                { "@FLastName", filters.FLastName },
                { "@FWorkStation", filters.FWorkStation }
            };

            return await _paginationRepository.ExecutePaginationAsync(
                storedProcedure: "getEmployeePaginationAdvanced",
                map: reader => new Employee
                {
                    Id = reader["id"].ToString() ?? string.Empty,
                    Dni = Convert.ToInt64(reader["dni"]),
                    FirstName = reader["first_name"].ToString() ?? string.Empty,
                    LastName = reader["last_name"].ToString() ?? string.Empty,
                    WorkStation = reader["workstation"].ToString() ?? string.Empty,
                },
                filter: filters,
                extraParams: extraParams
            );

        }

        public async Task<Employee?> FindByDni(long dni)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Dni == dni);
        }

        public async Task<Employee> Save(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> Update(Employee employee, long dniOld)
        {
            
            var employeeToUpdate = await FindByDni(dniOld);

            employeeToUpdate!.Dni = employee.Dni;
            employeeToUpdate.FirstName = employee.FirstName;
            employeeToUpdate.LastName = employee.LastName;
            employeeToUpdate.WorkStation = employee.WorkStation;
            employeeToUpdate.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return employeeToUpdate;

        }

        public async Task<Employee> UpdatePartial(Employee employee, long dniOld)
        {

            var employeeToUpdate = await FindByDni(dniOld);

            employeeToUpdate!.Dni = employee.Dni;
            employeeToUpdate.FirstName = employee.FirstName;
            employeeToUpdate.LastName = employee.LastName;
            employeeToUpdate.WorkStation = employee.WorkStation;
            employeeToUpdate.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return employeeToUpdate;
        
        }

    }
}
