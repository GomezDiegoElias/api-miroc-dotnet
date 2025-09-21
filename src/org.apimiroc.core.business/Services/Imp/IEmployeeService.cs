using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IEmployeeService
    {
        //public Task<PaginatedResponse<Employee>> FindAll(int pageIndex, int pageSize, int? dni, string? firstname, string? lastname, string? workstation);
        //public Task<PaginatedResponse<Employee>> FindAll(EmployeeFilter filter);
        public Task<PaginatedResponse<Employee>> FindAll(int pageIndex, int pageSize);
        public Task<Employee?> FindByDni(long dni);
        public Task<Employee> Save(EmployeeRequest request);
        public Task<Employee> Update(Employee employee);
        public Task<Employee> UpdatePartial(Employee employee);
        public Task<Employee> DeletePermanent(long dni);
        public Task<Employee> DeleteLogic(long dni);
    }
}
