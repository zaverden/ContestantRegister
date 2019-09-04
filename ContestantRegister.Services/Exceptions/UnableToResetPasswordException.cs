using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Services.Exceptions
{
    public class UnableToResetPasswordException : IdentityException
    {
        public UnableToResetPasswordException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
        
    }
}
