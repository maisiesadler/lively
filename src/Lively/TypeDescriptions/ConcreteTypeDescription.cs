using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lively.TypeDescriptions
{
    public class ConcreteTypeDescription : ITypeDescription
    {
        public string FullName { get; }
        public string Name { get; }
        public IReadOnlyList<TypeMethod> Methods { get; }

        public ConcreteTypeDescription(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var t = type.IsGenericType
                ? type.GetGenericTypeDefinition()
                : type;

            FullName = t.FullName;
            Name = t.Name;

            Methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => new TypeMethod(m.Name))
                .ToList();
        }
    }

    public record TypeMethod(string Name);
}
