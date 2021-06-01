using System.Collections.Generic;

namespace Lively.Console.Configuration
{
    public class FileConfiguration
    {
        public HashSet<string> Skip { get; set; }
        public IList<string> Generate { get; set; }
    }
}
