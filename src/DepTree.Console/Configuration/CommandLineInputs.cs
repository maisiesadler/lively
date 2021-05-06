using CommandLine;

namespace DepTree.Console.Configuration
{
    public class CommandLineInputs
    {
        [Option('a', "assembly",
            Required = false,
            HelpText = "The location of the assembly to read")]
        public string AssemblyLocation { get; set; } = null!;

        [Option("assembly-config",
            Required = false,
            HelpText = "The location of the configuration file required to build IConfiguration for Startup")]
        public string AssemblyConfigLocation { get; set; } = null!;

        [Option('t', "root-types",
            Required = false,
            HelpText = "The root type to use for the dependency tree, multiple values can be used as a csv input")]
        public string RootTypes { get; set; } = null!;

        [Option('c', "config",
            Required = false,
            HelpText = "The location of application config file")]
        public string ConfigLocation { get; set; } = null!;

        [Option('s', "skip-types",
            Required = false,
            HelpText = "Types to skip, multiple values can be used as a csv input")]
        public string SkipTypes { get; set; } = null!;

        [Option('i', "interface-resolver",
            Required = false,
            HelpText = "Interface Resolver type to use, Allowed Values: None, Startup. Default: Startup.")]
        public string InterfaceResolver { get; set; } = null!;

        [Option("startup-name",
            Required = false,
            HelpText = "Startup Type Name or FullName. Default: Startup.")]
        public string StartupName { get; set; } = null!;

        [Option("output-format",
            Required = false,
            HelpText = "Format to print out the result. Allowed values: debug, yumlmd, yuml. Default: yuml.")]
        public string OutputFormat { get; set; } = null!;
    }
}
