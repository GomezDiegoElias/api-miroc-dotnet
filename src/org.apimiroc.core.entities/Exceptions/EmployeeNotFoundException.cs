namespace org.apimiroc.core.entities.Exceptions
{
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(long dni)
            : base($"Error: Empleado con DNI {dni} no existe.") { }
        public EmployeeNotFoundException(string detail)
            : base($"Error: {detail}") { }
    }
}
