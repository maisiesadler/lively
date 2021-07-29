using System.Collections.Generic;

namespace Lively.TypeDescriptions
{
    public interface ITypeDescription
    {
        string Name { get; }
        string FullName { get; }
        IReadOnlyList<TypeMethod> Methods { get; }
    }
}
