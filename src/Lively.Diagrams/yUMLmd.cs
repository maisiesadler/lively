using System.Collections.Generic;
using System.Text;

namespace Lively.Diagrams
{
    public class yUMLmd
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new List<string>();

            var flattenedNodes = FlattenedNodes.Create(nodes);

            foreach (var (nodeName, nodeFullName, methods, children) in flattenedNodes.Relationships())
            {
                foreach (var (childname, childFullName, childPlusImpl, count) in children)
                {
                    if (count == 1)
                    {
                        builder.Add($"[{nodeName}]-&gt;[{childPlusImpl}]");
                    }
                    else
                    {
                        builder.Add($"[{nodeName}]-{count}&gt;[{childPlusImpl}]");
                    }
                }
            }

            var img = string.Join(", ", builder);
            return $"<img src=\"http://yuml.me/diagram/scruffy/class/{img}\" />";
        }
    }
}
