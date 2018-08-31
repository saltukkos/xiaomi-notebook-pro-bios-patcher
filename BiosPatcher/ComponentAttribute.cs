using System;
using JetBrains.Annotations;

namespace BiosPatcher
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class ComponentAttribute : Attribute
    {
        
    }
}