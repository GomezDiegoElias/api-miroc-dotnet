using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.apimiroc.core.entities.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException(string roleName)
            : base($"Rol '{roleName}' no existe.") { }
    }
}
