using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Services.Exceptions
{
    public class IdentityException : Exception
    {
        public IdentityException(IEnumerable<IdentityError> errors)
        {
            Errors = errors;
        }

        public IEnumerable<IdentityError> Errors { get; protected set; }
    }
}
