using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lively.Diagrams
{
    public class Mermaid
    {
        private static Regex _invalidMermaidChars = new Regex("`");

        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("classDiagram");

            var flattenedNodes = FlattenedNodes.Create(nodes);

            foreach (var (nodeName, nodeFullName, methods, children) in flattenedNodes.Relationships())
            {
                var _nodeName = _invalidMermaidChars.Replace(nodeName, "");
                foreach (var (childName, childFullName, childPlusImpl, count) in children)
                {
                    var _childname = _invalidMermaidChars.Replace(childName, "");
                    if (count == 1)
                    {
                        builder.AppendLine($"  {_nodeName} --> {_childname}");
                    }
                    else
                    {
                        builder.AppendLine($"  {_nodeName} --> \"{count}\" {childName}");
                    }
                }
            }

            foreach (var (interfaceName, interfaceFullName, implementation) in flattenedNodes.Implementations())
            {
                builder.AppendLine($"  class {interfaceName} {{");
                builder.AppendLine($"    {implementation.Name}");
                builder.AppendLine("  }");
            }

            return builder.ToString();
        }
    }
}
