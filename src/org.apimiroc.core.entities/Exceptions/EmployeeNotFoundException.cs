namespace org.apimiroc.core.entities.Exceptions
{
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(long dni)
            : base($"Empleado con DNI {dni} no existe.") { }
    }
}
