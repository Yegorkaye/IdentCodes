using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentCodes
{
    class PartException : Exception
    {
        public PartException(Tekla.Structures.Model.Part part)
        {
            Part = part;
        }

        public Tekla.Structures.Model.Part Part { get; private set; }
    }
}
