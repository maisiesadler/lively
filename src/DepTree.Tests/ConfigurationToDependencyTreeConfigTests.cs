using Xunit;
using System;

namespace DepTree.Tests
{
    public class DependencyTreeConfigTests
    {
        [Fact]
        public void NullAssemblyThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new DependencyTreeConfig(null));
        }

        [Fact]
        public void StartupNotSpecifiedDefaultIsStartup()
        {
            var config = new DependencyTreeConfig(GetType().Assembly);

            Assert.NotNull(config);
            Assert.Equal("Startup", config.StartupName);
        }
    }
}
