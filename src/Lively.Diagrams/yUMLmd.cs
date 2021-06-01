using System.Collections.Generic;
using System.Text;

namespace Lively.Diagrams
{
    public class yUMLmd
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new List<string>();

            var (relationships, implementations) = FlattenedNodes.Create(nodes);

            foreach (var (nodeName, children) in relationships)
            {
                foreach (var (childname, count) in children)
                {
                    var _childname = implementations.TryGetValue(childname, out var implementation)
                        ? $"{childname}|{implementation}"
                        : childname;

                    if (count == 1)
                    {
                        builder.Add($"[{nodeName}]-&gt;[{_childname}]");
                    }
                    else
                    {
                        builder.Add($"[{nodeName}]-{count}&gt;[{_childname}]");
                    }
                }
            }

            var img = string.Join(", ", builder);
            return $"<img src=\"http://yuml.me/diagram/scruffy/class/{img}\" />";
        }
    }
}
