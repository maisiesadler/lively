using System.Collections.Generic;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    public class FullNamePlantUml
    {
        public static string Create(IList<DependencyTreeNode> nodes)
        {
            var renderer = new PlantUmlRenderer(type => type.FullName);
            return renderer.Create(nodes); 
        }
    }
}
