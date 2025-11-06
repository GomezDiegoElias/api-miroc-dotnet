namespace org.apimiroc.core.entities.Exceptions
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(long dni)
            : base($"Error: Cliente con DNI n°: {dni} no existe") { }
        public ClientNotFoundException(string detail) 
            : base($"Error: {detail}") { }
    }
}
