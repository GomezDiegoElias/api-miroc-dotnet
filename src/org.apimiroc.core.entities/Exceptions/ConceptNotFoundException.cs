using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.apimiroc.core.entities.Exceptions
{
    public class ConceptNotFoundException : Exception
    {
        public ConceptNotFoundException(int id)
            : base($"Error: Concepto con ID {id} no existe") { }
        public ConceptNotFoundException(string details)
            : base($"Error: {details}") { }
    }
}
