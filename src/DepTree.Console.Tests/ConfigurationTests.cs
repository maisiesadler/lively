using Xunit;
using DepTree.Console.Configuration;

namespace DepTree.Console.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void CanBuildApplicationConfigFromArgs()
        {
            var args = new[] { "-a", "assembly-location", "-t", "type", "-c", "config-location" };

            var (config, ok) = ApplicationConfiguration.Build(args);

            Assert.NotNull(config);
            var type = Assert.Single(config.Generate);
            Assert.Equal("type", type);
            Assert.Equal("assembly-location", config.AssemblyLocation);
        }

        [Fact]
        public void InvalidAssemblyLocationProducesError()
        {
            var args = new[] { "-a", "assembly-location", "-t", "type", "-c", "config-location" };

            var (config, ok) = ApplicationConfiguration.Build(args);

            Assert.False(ok);
            Assert.Contains("Assembly Location 'assembly-location' is missing or invalid", config.Errors);
        }

        [Fact]
        public void ConfigFileIsParsed()
        {
            var args = new[] { "-a", "assembly-location", "-c", "exampleappconfig.json" };

            var (config, ok) = ApplicationConfiguration.Build(args);

            Assert.NotNull(config);
            Assert.Equal(2, config.Generate.Count);
            Assert.Equal("DepTree.DependencyTree", config.Generate[0]);
            Assert.Equal("DepTree.AnotherRootType", config.Generate[1]);
            var skip = Assert.Single(config.Skip);
            Assert.Equal("Serilog.IDiagnosticContext", skip);
        }

        [Fact]
        public void CsvTypesAreAdded()
        {
            var args = new[] { "-a", "assembly-location", "-t", "type1,type2" };

            var (config, ok) = ApplicationConfiguration.Build(args);

            Assert.NotNull(config);
            Assert.Equal(2, config.Generate.Count);
            Assert.Equal("type1", config.Generate[0]);
            Assert.Equal("type2", config.Generate[1]);
            Assert.Equal("assembly-location", config.AssemblyLocation);
        }
    }
}
