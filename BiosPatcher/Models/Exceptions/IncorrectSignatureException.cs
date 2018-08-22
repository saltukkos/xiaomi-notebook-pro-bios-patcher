using System;
using JetBrains.Annotations;

namespace BiosPatcher.Models.Exceptions
{
    public class IncorrectSignatureException : Exception
    {
        public IncorrectSignatureException([NotNull] string message)
            : base(message)
        {
        }
    }
}