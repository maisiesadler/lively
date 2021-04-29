using System.Reflection;

namespace DepTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var ass = Assembly.LoadFrom("src/Example/bin/Debug/net5.0/Example.dll");

            var dt = DependencyTree.Create(ass, "Example.Class1");

            Print(dt);
        }

        private static void Print(DependencyTreeNode node, string indent = "")
        {
            System.Console.WriteLine($"{indent}{node.Name} - {node.Type.FullName}");
            var childindent = indent + "  ";
            foreach (var c in node.Children)
            {
                Print(c, childindent);
            }
        }
    }
}
