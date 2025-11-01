namespace org.apimiroc.core.entities.Exceptions
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(string dni)
            : base($"Error: Cliente buscado por {dni} no existe") { }
    }
}
