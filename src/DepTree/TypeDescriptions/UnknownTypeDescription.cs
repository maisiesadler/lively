using System;

namespace DepTree.TypeDescriptions
{
    public class UnknownTypeDescription : ITypeDescription
    {
        public string FullName { get; }

        public UnknownTypeDescription(string fullName) => FullName = fullName;
    }
}
