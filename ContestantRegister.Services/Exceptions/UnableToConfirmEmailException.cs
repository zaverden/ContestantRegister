using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Application.Exceptions
{
    public class UnableToConfirmEmailException : IdentityException
    {
        public UnableToConfirmEmailException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}
