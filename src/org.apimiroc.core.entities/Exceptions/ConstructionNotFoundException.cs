namespace org.apimiroc.core.entities.Exceptions
{
    public class ConstructionNotFoundException : Exception
    {
        public ConstructionNotFoundException(string details)
            : base($"Error: {details}") { }
    }
}
