using Xunit;
using DepTree.Console.Configuration;
using DepTree.Resolvers;
using System;

namespace DepTree.Console.Tests
{
    public class EnvConfigurationTests
    {
        [Fact]
        public void CanBuildApplicationConfigFromEnvironmentVariables()
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "ROOT_TYPES", "type" }
            };

            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            var type = Assert.Single(config.Generate);
            Assert.Equal("type", type);
            Assert.Equal("assembly-location", config.AssemblyLocation);
        }

        [Fact]
        public void InvalidAssemblyLocationProducesError()
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.False(ok);
            Assert.Contains("Assembly Location 'assembly-location' is missing or invalid", config.Errors);
        }

        [Fact]
        public void ConfigFileIsParsed()
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "APPLICATION_CONFIG_LOCATION", "exampleappconfig.json" }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            Assert.Equal(2, config.Generate.Count);
            Assert.Equal("DepTree.DependencyTree", config.Generate[0]);
            Assert.Equal("DepTree.AnotherRootType", config.Generate[1]);
            var skip = Assert.Single(config.Skip);
            Assert.Equal("Serilog.IDiagnosticContext", skip);
        }

        [Fact]
        public void CsvRootTypesAreAdded()
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "ROOT_TYPES", "type1,type2" }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            Assert.Equal(2, config.Generate.Count);
            Assert.Equal("type1", config.Generate[0]);
            Assert.Equal("type2", config.Generate[1]);
        }

        [Fact]
        public void CsvSkipTypesAreAdded()
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "SKIP_TYPES", "type1,type2" }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            Assert.Equal(2, config.Skip.Count);
            Assert.Contains("type1", config.Skip);
            Assert.Contains("type2", config.Skip);
        }

        [Theory]
        [InlineData("None", typeof(NoInterfaceResolver))]
        [InlineData("Startup", typeof(StartupInterfaceResolver))]
        [InlineData("Beans", typeof(StartupInterfaceResolver))]
        public void InterfaceResolverCanBeSet(string interfaceResolver, Type expectedType)
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "INTERFACE_RESOLVER", interfaceResolver }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            var resolver = config.CreateInterfaceResolver(new DependencyTreeConfig(GetType().Assembly));
            Assert.Equal(expectedType, resolver.GetType());
        }

        [Fact]
        public void AssemblyConfigurationIsRead()
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "ASSEMBLY_CONFIG_LOCATION", "testsettings.json" }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            Assert.NotNull(config.AssemblyConfiguration);
            var defaultLogLevel = config.AssemblyConfiguration.GetSection("Logging:LogLevel:Default").Value;
            Assert.Equal("Information", defaultLogLevel);
        }

        [Fact]
        public void CanSpecifyStartupNameOverride()
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "STARTUP_NAME", "TestStartup" }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            Assert.Equal("TestStartup", config.StartupName);
        }

        [Theory]
        [InlineData("yuml", OutputFormatType.Yuml)]
        [InlineData("yumlmd", OutputFormatType.YumlMd)]
        [InlineData("debug", OutputFormatType.Debug)]
        [InlineData("beans", OutputFormatType.YumlMd)]
        [InlineData("", OutputFormatType.YumlMd)]
        public void CanSpecifyOutputFormat(string format, OutputFormatType expectedType)
        {
            var provider = new TestEnvironmentVariableProvider
            {
                { "ASSEMBLY_LOCATION", "assembly-location" },
                { "OUTPUT_FORMAT", format }
            };
            var (config, ok) = ApplicationConfiguration.Build(provider, new string[] { });

            Assert.NotNull(config);
            Assert.Equal(expectedType, config.OutputFormat);
        }
    }
}
