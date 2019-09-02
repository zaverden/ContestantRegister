using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Utils.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }
    }
}
