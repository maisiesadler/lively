using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lively.Resolvers;
using Microsoft.Extensions.Configuration;

namespace Lively
{
    public static class DependencyTreeConfigExtensions
    {
        public static IConfiguration EmptyConfiguration()
        {
            var cfgBuilder = new ConfigurationBuilder();
            return cfgBuilder.Build();
        }

        // public static IConfiguration ConfigurationFromJsonFile(string assemblyConfigLocation)
        // {
        //     var cfgBuilder = new ConfigurationBuilder();
        //     cfgBuilder.AddJsonFile(assemblyConfigLocation, optional: false, reloadOnChange: false);
        //     return cfgBuilder.Build();
        // }

        public static IEnumerable<Assembly> GetAllAssembliesInDirectory(
            string path,
            Func<string, bool> patternMatchingFn = null)
        {
            patternMatchingFn ??= _ => true;
            foreach (var file in Directory.GetFiles(path))
            {
                var split = file.Split(".");
                var fileEnding = split[split.Length - 1];
                if (fileEnding == "dll" && patternMatchingFn(file))
                {
                    yield return Assembly.LoadFrom(file);
                }
            }
        }
    }
}
