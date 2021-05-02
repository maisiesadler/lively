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
            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                var childname = child.Type?.Name;
                if (child.Implementation != null) childname += "|" + child.Implementation.Name;
                builder.AppendLine($"[{node.Type?.Name}]->[{childname}]");

                AddAllNodes(builder, child);
            }
        }
    }
}
