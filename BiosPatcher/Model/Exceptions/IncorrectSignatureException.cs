using System;
using JetBrains.Annotations;

namespace BiosPatcher.Model.Exceptions
{
    public class IncorrectSignatureException : Exception
    {
        public IncorrectSignatureException([NotNull] string message)
            : base(message)
        {
        }
    }
}