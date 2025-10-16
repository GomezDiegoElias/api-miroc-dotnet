namespace org.apimiroc.core.entities.Exceptions
{
    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(string cuit)
            : base($"Proveedor con Cuit {cuit} no existe") { }
    }
}
