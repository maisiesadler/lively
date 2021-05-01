using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DepTree
{
    public class DependencyTreeConfig
    {
        public Assembly Assembly { get; }
        public IInterfaceResolver InterfaceResolver { get; }
        public string StartupName { get; }

        public DependencyTreeConfig(Assembly assembly, HashSet<string> skipAssemblies = null, string startupName = "Startup")
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            InterfaceResolver = new StartupInterfaceResolver(assembly, skipAssemblies, startupName);
        }

        public DependencyTreeConfig(Assembly assembly, IConfiguration configuration, HashSet<string> skipAssemblies = null, string startupName = "Startup")
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            InterfaceResolver = new StartupInterfaceResolver(assembly, configuration, skipAssemblies, startupName);
        }
    }

    public interface IInterfaceResolver
    {
        Type Resolve(Type t);
    }

    public class StartupInterfaceResolver : IInterfaceResolver
    {
        private readonly FakeServiceCollection _fakeServiceCollection;

        public StartupInterfaceResolver(Assembly assembly, HashSet<string> skipAssemblies, string startupName)
        {
            var startupType = assembly.GetTypes().FirstOrDefault(x => x.Name == startupName);
            if (startupType == null)
            {
                throw new Exception("Could not find Startup in assembly");
            }

            var startup = Activator.CreateInstance(startupType);
            MethodInfo method = startupType.GetMethod("ConfigureServices");
            _fakeServiceCollection = new FakeServiceCollection();
            method.Invoke(startup, new[] { _fakeServiceCollection });

            SkipAssemblies = skipAssemblies;
        }

        public StartupInterfaceResolver(Assembly assembly, IConfiguration configuration, HashSet<string> skipAssemblies, string startupName)
        {
            var startupType = assembly.GetTypes().FirstOrDefault(x => x.Name == startupName);
            if (startupType == null)
            {
                throw new Exception("Could not find Startup in assembly");
            }

            var startup = Activator.CreateInstance(startupType, configuration);
            MethodInfo method = startupType.GetMethod("ConfigureServices");
            _fakeServiceCollection = new FakeServiceCollection();
            method.Invoke(startup, new[] { _fakeServiceCollection });

            SkipAssemblies = skipAssemblies;
        }

        public HashSet<string> SkipAssemblies { get; }

        public Type Resolve(Type t)
        {
            if (SkipAssemblies?.Contains(t.FullName) == true) return null;

            return _fakeServiceCollection.GetImplementation(t);
        }
    }

    public class FakeServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
        public Type GetImplementation(Type type)
        {
            foreach (var x in this)
            {
                if (x.ServiceType == type)
                {
                    return x.ImplementationType;
                }
            }

            return null;
        }
    }
}
