namespace org.apimiroc.core.entities.Exceptions
{
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(string dni)
            : base($"Error: Empleado buscado por {dni} no existe.") { }
    }
}
