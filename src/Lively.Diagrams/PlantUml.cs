using System.Collections.Generic;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    public class PlantUml
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var renderer = new PlantUmlRenderer(type => type.Name);
            return renderer.Create(nodes); 
        }
    }
}
