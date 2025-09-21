using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Mappers
{
    public static class EmployeeMapper
    {

        public static EmployeeResponse ToResponse(Employee employee)
        {
            return new EmployeeResponse(employee.Dni, employee.FirstName, employee.LastName, employee.WorkStation);
        }

        public static EmployeeRequest ToRequest(Employee employee)
        {
            return new EmployeeRequest(employee.Dni, employee.FirstName, employee.LastName, employee.WorkStation);
        }

        public static Employee ToEntityForUpdate(EmployeeRequest request, Employee employee)
        {
            return new Employee(employee.Id, request.Dni, request.FirstName, request.LastName, request.WorkStation);
        }

        public static Employee ToEntityForPatch(EmployeeRequest request, Employee employee)
        {
            return new Employee(employee.Id, request.Dni, request.FirstName, request.LastName, request.WorkStation);
        }

    }
}
