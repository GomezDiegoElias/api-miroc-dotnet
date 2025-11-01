namespace org.apimiroc.core.entities.Exceptions
{
    public class MovementNotFoundException : Exception
    {
        public MovementNotFoundException(int code)
            : base($"Error: Movimiento con codigo {code} no existe") { }
        public MovementNotFoundException(string details) 
            : base($"Error: {details}") { }
    }
}
