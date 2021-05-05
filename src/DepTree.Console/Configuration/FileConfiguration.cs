using System.Collections.Generic;

namespace DepTree.Console.Configuration
{
    public class FileConfiguration
    {
        public HashSet<string> Skip { get; set; }
        public IList<string> Generate { get; set; }
    }
}
