namespace org.apimiroc.core.entities.Exceptions
{
    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(long cuit)
            : base($"Error: Proveedor con Cuit {cuit} no existe") { }
        public ProviderNotFoundException(string detail)
            : base($"Error: {detail}") { }
    }
}
