using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Services.Exceptions
{
    public class UnableToChangePasswordException : IdentityException
    {
        public UnableToChangePasswordException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}
