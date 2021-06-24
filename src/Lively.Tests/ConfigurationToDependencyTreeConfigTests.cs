using Xunit;
using System;
using System.Reflection;

namespace Lively.Tests
{
    public class DependencyTreeConfigTests
    {
        [Fact]
        public void NullAssemblyThrows()
        {
            Assembly assembly = null;
            Assert.Throws<ArgumentNullException>(() => new DependencyTreeConfig(assembly));
        }

        [Fact]
        public void NoAssemblyThrows()
        {
            var assemblies = new Assembly[] { };
            Assert.Throws<InvalidOperationException>(() => new DependencyTreeConfig(assemblies));
        }

        // [Fact]
        // public void NoNonNullAssemblyThrows()
        // {
        //     var assemblies = new Assembly[] { null };
        //     Assert.Throws<InvalidOperationException>(() => new DependencyTreeConfig(assemblies));
        // }

        [Fact]
        public void StartupNotSpecifiedDefaultIsStartup()
        {
            var config = new DependencyTreeConfig(GetType().Assembly);

            Assert.NotNull(config);
            Assert.Equal("Startup", config.StartupName);
        }
    }
}
