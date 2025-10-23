namespace org.apimiroc.core.entities.Exceptions
{
    public class ConstructionNotFoundException : Exception
    {
        public ConstructionNotFoundException(string name)
            : base($"La obra {name} no existe") { }
    }
}
