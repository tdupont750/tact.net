using System;
using System.Collections.Generic;

namespace Tact.ComponentModel
{
    public class ModelValidationException : Exception
    {
        public ModelValidationException(string typeName, IReadOnlyDictionary<string, IReadOnlyCollection<string>> errors)
            : base("Model is invalid")
        {
            TypeName = typeName;
            Errors = errors;
        }

        public string TypeName { get; set; }

        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Errors { get; set; }
    }
}