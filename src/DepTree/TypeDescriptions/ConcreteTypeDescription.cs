using System;

namespace DepTree.TypeDescriptions
{
    public class ConcreteTypeDescription : ITypeDescription
    {
        public string FullName => _type.FullName;

        public string Name => _type.Name;

        private readonly Type _type;

        public ConcreteTypeDescription(Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));
    }
}
