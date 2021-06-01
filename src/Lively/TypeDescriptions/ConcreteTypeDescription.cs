using System;

namespace Lively.TypeDescriptions
{
    public class ConcreteTypeDescription : ITypeDescription
    {
        public string FullName { get; }
        public string Name { get; }

        public ConcreteTypeDescription(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var t = type.IsGenericType
                ? type.GetGenericTypeDefinition()
                : type;
            
            FullName = t.FullName;
            Name = t.Name;
        }
    }
}
