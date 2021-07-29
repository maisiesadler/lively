using System.Collections.Generic;
using System.Text;

namespace Lively.Diagrams
{
    public class yUMLmd
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new List<string>();

            var (relationships, types, implementations) = FlattenedNodes.Create(nodes);

            foreach (var (nodeFullName, children) in relationships)
            {
                var nodeName = types[nodeFullName].Name;
                foreach (var (childname, count) in children)
                {
                    var _childname = types.TryGetValue(childname, out var childType)
                        ? childType.Name
                        : childname;

                    _childname = implementations.TryGetValue(childname, out var implementation)
                        ? $"{_childname}|{implementation}"
                        : _childname;

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
