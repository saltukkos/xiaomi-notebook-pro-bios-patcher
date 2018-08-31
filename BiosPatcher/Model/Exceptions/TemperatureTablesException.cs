using System;
using JetBrains.Annotations;

namespace BiosPatcher.Model.Exceptions
{
    public class TemperatureTablesException : Exception
    {
        public TemperatureTablesException([NotNull] string message) 
            : base(message)
        {
        }
    }
}