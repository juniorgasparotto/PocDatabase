using System;
using System.Runtime.Serialization;

namespace PocDatabase
{
    public class PropertyIdNotExistsException : Exception
    {
        public PropertyIdNotExistsException(Type type) : base($"The type '{type.FullName}' dosen't has a property: 'public Guid Id {{get; set;}}'")
        {
        }
    }
}