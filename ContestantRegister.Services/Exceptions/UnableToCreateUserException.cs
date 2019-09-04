using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Services.Exceptions
{
    public class UnableToCreateUserException : IdentityException
    {
        public UnableToCreateUserException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}
