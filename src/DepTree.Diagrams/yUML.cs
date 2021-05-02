using System.Text;

namespace DepTree.Diagrams
{
    public class yUML
    {
        public static string Create(DependencyTreeNode node)
        {
            var builder = new StringBuilder(@"// {type:class}
// {direction:topDown}
// {generate:true}

");

            AddAllNodes(builder, node);

            return builder.ToString();
        }

        private static void AddAllNodes(StringBuilder builder, DependencyTreeNode node)
        {
            foreach (var child in node.Children)
            {
                builder.AppendLine($"[{node.Type?.Name}]->[{child.Type?.Name}]");

                AddAllNodes(builder, child);
            }
        }
    }
}
