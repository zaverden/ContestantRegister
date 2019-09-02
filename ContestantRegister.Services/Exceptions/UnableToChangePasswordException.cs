using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Application.Exceptions;
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
