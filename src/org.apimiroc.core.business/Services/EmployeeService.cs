using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> DeleteLogic(long dni)
        {
            var employee = await FindByDni(dni);
            return await _employeeRepository.DeleteLogic(employee!.Dni);
        }

        public async Task<Employee> DeletePermanent(long dni)
        {
            var employee = await FindByDni(dni);
            return await _employeeRepository.DeletePermanent(employee!.Dni);
        }

        public async Task<PaginatedResponse<Employee>> FindAll(EmployeeFilter filters)
        {
            return await _employeeRepository.FindAll(filters);
        }

        public async Task<Employee?> FindByDni(long dni)
        {
            return await _employeeRepository.FindByDni(dni)
                ?? throw new EmployeeNotFoundException(dni);
        }

        public async Task<Employee?> FindById(string id)
        {
            return await _employeeRepository.FindById(id)
                ?? throw new EmployeeNotFoundException($"Empleado con ID {id} no existe");
        }

        public async Task<Employee> Save(Employee employee)
        {
            if (await _employeeRepository.ExistDni(employee.Dni)) 
                throw new EmployeeNotFoundException($"El DNI ingresado {employee.Dni} ya existe");
            return await _employeeRepository.Save(employee);
        }

        public async Task<Employee> Update(Employee employee, long dniOld)
        {
            return await _employeeRepository.Update(employee, dniOld);
        }

        public async Task<Employee> UpdatePartial(Employee employee, long dniOld)
        {
            return await _employeeRepository.UpdatePartial(employee, dniOld);
        }
    }
}
