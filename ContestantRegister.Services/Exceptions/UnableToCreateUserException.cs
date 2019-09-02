using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Application.Exceptions
{
    public class UnableToCreateUserException : IdentityException
    {
        public UnableToCreateUserException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}
