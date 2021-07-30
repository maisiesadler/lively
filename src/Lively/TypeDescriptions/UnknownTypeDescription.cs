using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lively.TypeDescriptions
{
    public class UnknownTypeDescription : ITypeDescription
    {
        // match Assembly.ClassName1+Nested`1[[GenericParam]]
        private static Regex _genericTypeRegex = new Regex("^([^\\[]+)\\[");
        public string FullName { get; }

        public string Name { get; }

        public IReadOnlyList<TypeMethod> Methods => Array.Empty<TypeMethod>();

        public UnknownTypeDescription(string fullName)
        {
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                if (_genericTypeRegex.IsMatch(fullName))
                {
                    var match = _genericTypeRegex.Match(fullName);

                    fullName = match.Groups[1].Value;
                }

                FullName = fullName;
                Name = fullName.Split('.', '+').Last();
            }
            else
            {
                FullName = string.Empty;
                Name = string.Empty;
            }
        }
    }
}
