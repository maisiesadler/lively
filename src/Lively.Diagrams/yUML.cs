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

            var flattenedNodes = FlattenedNodes.Create(nodes);

            foreach (var (nodeName, nodeFullName, methods, children) in flattenedNodes.Relationships())
            {
                foreach (var (childname, childFullName, childPlusImpl, count) in children)
                {
                    if (count == 1)
                    {
                        builder.AppendLine($"[{nodeName}]->[{childPlusImpl}]");
                    }
                    else
                    {
                        builder.AppendLine($"[{nodeName}]-{count}>[{childPlusImpl}]");
                    }
                }
            }

            return builder.ToString();
        }
    }
}
