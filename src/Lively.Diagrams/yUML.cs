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
