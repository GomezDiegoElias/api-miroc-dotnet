namespace org.apimiroc.core.entities.Exceptions
{
    public class MovementNotFoundException : Exception
    {
        public MovementNotFoundException(int id)
            : base($"Movimiento con ID {id} no existe") { }
    }
}
