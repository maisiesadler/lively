using CommandLine;

namespace DepTree.Console.Configuration
{
    public class CommandLineInputs
    {
        [Option('a', "assembly",
            Required = false,
            HelpText = "The location of the assembly to read")]
        public string AssemblyLocation { get; set; } = null!;

        [Option('n', "assembly-config",
            Required = false,
            HelpText = "The location of the configuration file required to build IConfiguration for Startup")]
        public string AssemblyConfigLocation { get; set; } = null!;

        [Option('t', "type",
            Required = false,
            HelpText = "The root type to use for the dependency tree, multiple values can be used as a csv input")]
        public string Type { get; set; } = null!;

        [Option('c', "config",
            Required = false,
            HelpText = "The location of application config file")]
        public string ConfigLocation { get; set; } = null!;

        [Option('s', "skip",
            Required = false,
            HelpText = "Types to skip, multiple values can be used as a csv input")]
        public string Skip { get; set; } = null!;

        [Option('i', "interface-resolver",
            Required = false,
            HelpText = "Interface Resolver type to use, Allowed Values: None, Startup. Default: Startup.")]
        public string InterfaceResolver { get; set; } = null!;
    }
}
