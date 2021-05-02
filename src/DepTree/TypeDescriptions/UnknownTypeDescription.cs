using System.Linq;

namespace DepTree.TypeDescriptions
{
    public class UnknownTypeDescription : ITypeDescription
    {
        public string FullName { get; }

        public string Name { get; }

        public UnknownTypeDescription(string fullName)
        {
            FullName = fullName;
            Name = string.IsNullOrWhiteSpace(fullName)
                ? string.Empty
                : fullName.Split('.', '+').Last();
        }
    }
}
