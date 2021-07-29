using System.Collections.Generic;
using System.Text;

namespace Lively.Diagrams
{
    public class yUML
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder(@"// {type:class}
// {direction:topDown}
// {generate:true}

");

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
                        builder.AppendLine($"[{nodeName}]->[{_childname}]");
                    }
                    else
                    {
                        builder.AppendLine($"[{nodeName}]-{count}>[{_childname}]");
                    }
                }
            }

            return builder.ToString();
        }
    }
}
