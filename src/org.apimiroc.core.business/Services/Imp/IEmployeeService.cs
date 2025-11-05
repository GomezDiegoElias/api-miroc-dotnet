using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IEmployeeService
    {
        public Task<PaginatedResponse<Employee>> FindAll(EmployeeFilter filters);
        public Task<Employee?> FindByDni(long dni);
        public Task<Employee?> FindById(string id);
        public Task<Employee> Save(Employee employee);
        public Task<Employee> Update(Employee employee, long dniOld);
        public Task<Employee> UpdatePartial(Employee employee, long dniOld);
        public Task<Employee> DeletePermanent(long dni);
        public Task<Employee> DeleteLogic(long dni);
    }
}
