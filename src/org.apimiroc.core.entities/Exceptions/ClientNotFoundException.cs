﻿namespace org.apimiroc.core.entities.Exceptions
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(string dni)
            : base($"Cliente con DNI {dni} no existe") { }
    }
}
