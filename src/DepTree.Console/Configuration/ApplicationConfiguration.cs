using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CommandLine;
using Microsoft.Extensions.Configuration;

namespace DepTree.Console.Configuration
{
    public class ApplicationConfiguration
    {
        private ApplicationConfiguration() { }

        private string _assemblyConfigLocation;
        private string _configLocation;

        public IConfiguration AssemblyConfiguration { get; private set; }
        public string AssemblyLocation { get; private set; }
        public string InterfaceResolver { get; private set; }
        public HashSet<string> Skip { get; private set; } = new HashSet<string>();
        public List<string> Generate { get; private set; } = new List<string>();
        public List<string> Errors { get; private set; } = new List<string>();
        public bool IsValid => Errors.Count == 0;

        public static (ApplicationConfiguration, bool) Build(string[] args)
        {
            var config = new ApplicationConfiguration();

            config.ReadEnvironmentVariables();

            config.TryReadArgs(args);
            config.TryReadFileConfig();

            config.TryBuildAssemblyConfiguration();

            config.Validate();

            return (config, config.IsValid);
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(AssemblyLocation) || !File.Exists(AssemblyLocation))
            {
                Errors.Add($"Assembly Location '{AssemblyLocation}' is missing or invalid");
            }
        }

        private void ReadEnvironmentVariables()
        {
            _assemblyConfigLocation = Environment.GetEnvironmentVariable("ASSEMBLY_CONFIG_LOCATION");
            _configLocation = Environment.GetEnvironmentVariable("CONFIG_LOCATION");
            InterfaceResolver = Environment.GetEnvironmentVariable("INTERFACE_RESOLVER");
        }

        private void TryReadArgs(string[] args)
        {
            var parser = Parser.Default.ParseArguments<CommandLineInputs>(() => new(), args);
            parser.WithParsed(inputs =>
            {
                AssemblyLocation = inputs.AssemblyLocation;
                InterfaceResolver = inputs.InterfaceResolver;

                if (!string.IsNullOrWhiteSpace(inputs.Type))
                {
                    var types = inputs.Type.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    Generate.AddRange(types);
                }

                if (!string.IsNullOrWhiteSpace(inputs.Skip))
                {
                    var skip = inputs.Skip.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var s in skip) Skip.Add(s);
                }

                _assemblyConfigLocation = inputs.AssemblyConfigLocation;
                _configLocation = inputs.ConfigLocation;
            });
        }

        private void TryReadFileConfig()
        {
            try
            {
                if (_configLocation == null) return;
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
    }
}