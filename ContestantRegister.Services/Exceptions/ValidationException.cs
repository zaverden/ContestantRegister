using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(List<KeyValuePair<string, string>> validationResult)
        {
            ValidationResult = validationResult;
        }

        public List<KeyValuePair<string, string>> ValidationResult { get; }
    }
}
