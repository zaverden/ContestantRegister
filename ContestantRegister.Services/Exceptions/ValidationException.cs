using System;
using System.Collections.Generic;

namespace ContestantRegister.Services.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(List<KeyValuePair<string, string>> validationResult)
        {
            ValidationResult = validationResult;
        }

        public ValidationException(string key, string message)
        {
            ValidationResult = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create(key, message)
            };
        }

        public List<KeyValuePair<string, string>> ValidationResult { get; }
    }
}
