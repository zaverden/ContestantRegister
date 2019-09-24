using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Services.Exceptions
{
    public class UnableToConfirmEmailException : IdentityException
    {
        public UnableToConfirmEmailException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}
