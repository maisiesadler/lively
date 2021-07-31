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

            foreach (var (nodeType, childType, count) in flattenedNodes.Relationships())
            {
                var childImplementationType = flattenedNodes.Implementation(childType);
                var childPlusImpl = childType.Name;
                if (childImplementationType != null)
                    childPlusImpl += $"|{childImplementationType.Name}";

                if (count == 1)
                {
                    builder.AppendLine($"[{nodeType.Name}]->[{childPlusImpl}]");
                }
                else
                {
                    builder.AppendLine($"[{nodeType.Name}]-{count}>[{childPlusImpl}]");
                }
            }

            return builder.ToString();
        }
    }
}
