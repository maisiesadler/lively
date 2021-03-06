using Lively.Console.Configuration;
using System.Collections.Generic;

namespace Lively.Console.Tests
{
    public class TestEnvironmentVariableProvider : Dictionary<string, string>,  IEnvironmentVariableProvider
    {
        public string GetEnvironmentVariable(string variable)
        {
            return TryGetValue(variable, out var value)
                ? value
                : string.Empty;
        }
    }
}
