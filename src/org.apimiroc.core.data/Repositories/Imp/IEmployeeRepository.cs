using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IEmployeeRepository
    {
        //public Task<PaginatedResponse<Employee>> FindAll(int pageIndex, int pageSize, int? dni, string? firstname, string? lastname, string? workstation);
        //public Task<PaginatedResponse<Employee>> FindAll(EmployeeFilter filter);
        public Task<PaginatedResponse<Employee>> FindAll(int pageIndex, int pageSize);
        public Task<Employee?> FindByDni(long dni);
        public Task<Employee> Save(Employee employee);
        public Task<Employee> Update(Employee employee, long dniOld);
        public Task<Employee> UpdatePartial(Employee employee, long dniOld);
        public Task<Employee> DeletePermanent(long dni);
        public Task<Employee> DeleteLogic(long dni);
    }
}
