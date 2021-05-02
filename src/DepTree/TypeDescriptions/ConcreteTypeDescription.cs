using System;

namespace DepTree.TypeDescriptions
{
    public class ConcreteTypeDescription : ITypeDescription
    {
        public string FullName { get; }

        public ConcreteTypeDescription(Type type) => FullName = type.FullName;
    }
}
