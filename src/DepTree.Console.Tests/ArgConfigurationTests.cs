using Xunit;
using DepTree.Console.Configuration;
using DepTree.Resolvers;

namespace DepTree.Console.Tests
{
    public class ArgConfigurationTests
    {
        [Fact]
        public void CanBuildApplicationConfigFromArgs()
        {
            var args = new[] { "-a", "assembly-location", "-t", "type", "-c", "config-location" };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.NotNull(config);
            var type = Assert.Single(config.Generate);
            Assert.Equal("type", type);
            Assert.Equal("assembly-location", config.AssemblyLocation);
        }

        [Fact]
        public void InvalidAssemblyLocationProducesError()
        {
            var args = new[] { "-a", "assembly-location", "-t", "type", "-c", "config-location" };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.False(ok);
            Assert.Contains("Assembly Location 'assembly-location' is missing or invalid", config.Errors);
        }

        [Fact]
        public void ConfigFileIsParsed()
        {
            var args = new[] { "-a", "assembly-location", "-c", "exampleappconfig.json" };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

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
            var args = new[] { "-a", "assembly-location", "-t", "type1,type2" };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.NotNull(config);
            Assert.Equal(2, config.Generate.Count);
            Assert.Equal("type1", config.Generate[0]);
            Assert.Equal("type2", config.Generate[1]);
        }

        [Fact]
        public void CsvSkipTypesAreAdded()
        {
            var args = new[] { "-a", "assembly-location", "-s", "type1,type2" };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.NotNull(config);
            Assert.Equal(2, config.Skip.Count);
            Assert.Contains("type1", config.Skip);
            Assert.Contains("type2", config.Skip);
        }

        [Theory]
        [InlineData("None", InterfaceResolverType.None)]
        [InlineData("Startup", InterfaceResolverType.Startup)]
        [InlineData("Beans", InterfaceResolverType.Startup)]
        public void InterfaceResolverCanBeSet(string interfaceResolver, InterfaceResolverType expectedType)
        {
            var args = new[] { "-a", "assembly-location", "-i", interfaceResolver };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.NotNull(config);
            Assert.Equal(expectedType, config.InterfaceResolverType);
        }

        [Fact]
        public void AssemblyConfigurationIsRead()
        {
            var args = new[] { "-a", "assembly-location", "--assembly-config", "testsettings.json" };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.NotNull(config);
            Assert.NotNull(config.AssemblyConfiguration);
            var defaultLogLevel = config.AssemblyConfiguration.GetSection("Logging:LogLevel:Default").Value;
            Assert.Equal("Information", defaultLogLevel);
        }

        [Theory]
        [InlineData("beans", "beans")]
        [InlineData("", "Startup")]
        public void CanSpecifyStartupNameOverride(string startupNameInput, string expectedStartupName)
        {
            var args = new[] { "-a", "assembly-location", "--startup-name", startupNameInput };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.NotNull(config);
            Assert.Equal(expectedStartupName, config.StartupName);
        }

        [Fact]
        public void CanSpecifyOutputFormat()
        {
            var args = new[] { "-a", "assembly-location", "--output-format", "yuml" };

            var (config, ok) = ApplicationConfiguration.Build(new TestEnvironmentVariableProvider(), args);

            Assert.NotNull(config);
            Assert.Equal(OutputFormatType.Yuml, config.OutputFormat);
        }
    }
}
