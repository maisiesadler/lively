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

            var (relationships, types, implementations) = FlattenedNodes.Create(nodes);

            foreach (var (nodeFullName, children) in relationships)
            {
                var type = types[nodeFullName];
                var _nodeName = _invalidMermaidChars.Replace(type.Name, "");
                foreach (var (childname, count) in children)
                {
                    var _childname = _invalidMermaidChars.Replace(childname, "");
                    if (count == 1)
                    {
                        builder.AppendLine($"  {_nodeName} --> {_childname}");
                    }
                    else
                    {
                        builder.AppendLine($"  {_nodeName} --> \"{count}\" {_childname}");
                    }
                }
            }

            foreach (var (@interface, implementation) in implementations)
            {
                builder.AppendLine($"  class {@interface} {{");
                builder.AppendLine($"    {implementation}");
                builder.AppendLine("  }");
            }

            return builder.ToString();
        }
    }
}
