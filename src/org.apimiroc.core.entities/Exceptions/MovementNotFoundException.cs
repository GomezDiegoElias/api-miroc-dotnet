namespace org.apimiroc.core.entities.Exceptions
{
    public class MovementNotFoundException : Exception
    {
        public MovementNotFoundException(int id)
            : base($"Error: Movimiento con ID {id} no existe") { }
        public MovementNotFoundException(string details) 
            : base($"Error: {details}") { }
    }
}
