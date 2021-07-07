using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CommandLine;
using Lively.Resolvers;
using Microsoft.Extensions.Configuration;

namespace Lively.Console.Configuration
{
    public class ApplicationConfiguration
    {
        private ApplicationConfiguration() { }

        private string _assemblyConfigLocation;
        private string _configLocation;

        public IConfiguration AssemblyConfiguration { get; private set; }
        public string AssemblyLocation { get; private set; }
        public string AssemblyPatternMatch { get; private set; }
        public HashSet<string> Skip { get; private set; } = new HashSet<string>();
        public List<string> Generate { get; private set; } = new List<string>();
        public List<string> Errors { get; private set; } = new List<string>();
        public Func<DependencyTreeConfig, IInterfaceResolver> CreateInterfaceResolver { get; set; } = StartupInterfaceResolver.Create;
        public string StartupName { get; private set; } = "Startup";
        public OutputFormatType OutputFormat { get; private set; } = OutputFormatType.YumlMd;
        public bool IsValid => Errors.Count == 0;

        public static (ApplicationConfiguration, bool) Build(IEnvironmentVariableProvider environmentVariableProvider, string[] args)
        {
            var config = new ApplicationConfiguration();

            config.ReadEnvironmentVariables(environmentVariableProvider);

            config.TryReadArgs(args);
            config.TryReadFileConfig();

            config.TryBuildAssemblyConfiguration();

            config.Validate();

            return (config, config.IsValid);
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(AssemblyLocation)
                || (!File.Exists(AssemblyLocation) && !Directory.Exists(AssemblyLocation)))
            {
                Errors.Add($"Assembly Location '{AssemblyLocation}' is missing or invalid");
            }
        }

        private void ReadEnvironmentVariables(IEnvironmentVariableProvider environmentVariableProvider)
        {
            AssemblyLocation = environmentVariableProvider.GetEnvironmentVariable("ASSEMBLY_LOCATION");
            AssemblyPatternMatch = environmentVariableProvider.GetEnvironmentVariable("ASSEMBLY_PATTERN_MATCH");
            _assemblyConfigLocation = environmentVariableProvider.GetEnvironmentVariable("ASSEMBLY_CONFIG_LOCATION");
            _configLocation = environmentVariableProvider.GetEnvironmentVariable("APPLICATION_CONFIG_LOCATION");
            TrySetInterfaceResolver(environmentVariableProvider.GetEnvironmentVariable("INTERFACE_RESOLVER"));
            TrySetOutputFormat(environmentVariableProvider.GetEnvironmentVariable("OUTPUT_FORMAT"));

            var startupName = environmentVariableProvider.GetEnvironmentVariable("STARTUP_NAME");
            if (!string.IsNullOrWhiteSpace(startupName)) StartupName = startupName;
            var rootTypes = environmentVariableProvider.GetEnvironmentVariable("ROOT_TYPES");
            if (!string.IsNullOrWhiteSpace(rootTypes))
            {
                var types = rootTypes.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Generate.AddRange(types);
            }

            var skipTypes = environmentVariableProvider.GetEnvironmentVariable("SKIP_TYPES");
            if (!string.IsNullOrWhiteSpace(skipTypes))
            {
                var skip = skipTypes.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var s in skip) Skip.Add(s);
            }
        }

        private void TryReadArgs(string[] args)
        {
            var parser = Parser.Default.ParseArguments<CommandLineInputs>(() => new(), args);
            parser.WithParsed(inputs =>
            {
                if (!string.IsNullOrWhiteSpace(inputs.AssemblyLocation))
                    AssemblyLocation = inputs.AssemblyLocation;

                if (!string.IsNullOrWhiteSpace(inputs.AssemblyPatternMatch))
                    AssemblyPatternMatch = inputs.AssemblyPatternMatch;

                if (!string.IsNullOrWhiteSpace(inputs.InterfaceResolver))
                    TrySetInterfaceResolver(inputs.InterfaceResolver);

                if (!string.IsNullOrWhiteSpace(inputs.StartupName))
                    StartupName = inputs.StartupName;

                if (!string.IsNullOrWhiteSpace(inputs.OutputFormat))
                    TrySetOutputFormat(inputs.OutputFormat);

                if (!string.IsNullOrWhiteSpace(inputs.RootTypes))
                {
                    var types = inputs.RootTypes.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    Generate.AddRange(types);
                }

                if (!string.IsNullOrWhiteSpace(inputs.SkipTypes))
                {
                    var skip = inputs.SkipTypes.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var s in skip) Skip.Add(s);
                }

                if (!string.IsNullOrWhiteSpace(inputs.AssemblyConfigLocation))
                    _assemblyConfigLocation = inputs.AssemblyConfigLocation;
                if (!string.IsNullOrWhiteSpace(inputs.ConfigLocation))
                    _configLocation = inputs.ConfigLocation;
            });

            parser.WithNotParsed(errors =>
            {
                Errors.Add("Invalid CLI inputs");
            });
        }

        private void TryReadFileConfig()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_configLocation)) return;
                var configContents = File.ReadAllText(_configLocation);
                var fileConfig = JsonSerializer.Deserialize<FileConfiguration>(configContents);

                if (fileConfig.Skip != null)
                    foreach (var s in fileConfig.Skip) Skip.Add(s);

                if (fileConfig.Generate != null)
                    Generate.AddRange(fileConfig.Generate);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Exception reading config: {ex.Message}.");
            }
        }

        private void TryBuildAssemblyConfiguration()
        {
            if (!string.IsNullOrWhiteSpace(_assemblyConfigLocation) && File.Exists(_assemblyConfigLocation))
            {
                var cfgBuilder = new ConfigurationBuilder();
                cfgBuilder.AddJsonFile(_assemblyConfigLocation, optional: false, reloadOnChange: false);
                AssemblyConfiguration = cfgBuilder.Build();
            }
        }

        private void TrySetInterfaceResolver(string value)
        {
            if (value == "None")
                CreateInterfaceResolver = NoInterfaceResolver.Create;
            else if (value == "Startup")
                CreateInterfaceResolver = StartupInterfaceResolver.Create;
        }

        private void TrySetOutputFormat(string value)
        {
            if (value == "yumlmd")
                OutputFormat = OutputFormatType.YumlMd;
            else if (value == "yuml")
                OutputFormat = OutputFormatType.Yuml;
            else if (value == "debug")
                OutputFormat = OutputFormatType.Debug;
            else if (value == "mermaid")
                OutputFormat = OutputFormatType.Mermaid;
            else if (value == "mermaidmd")
                OutputFormat = OutputFormatType.MermaidMd;
            else if (value == "plantuml")
                OutputFormat = OutputFormatType.PlantUml;
            else if (!string.IsNullOrWhiteSpace(value))
                System.Console.WriteLine($"Warning: output format '{value}' is unknown");
        }
    }
}
