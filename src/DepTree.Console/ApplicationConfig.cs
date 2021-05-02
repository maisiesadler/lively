using System.Collections.Generic;

namespace DepTree.Console
{
    public class ApplicationConfig
    {
        public HashSet<string> Skip { get; set; }
        public IList<string> Generate { get; set; }
    }
}
