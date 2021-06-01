using System;

namespace Lively.Console.Configuration
{
    public interface IEnvironmentVariableProvider
    {
        string GetEnvironmentVariable(string variable);
    }

    public class EnvironmentVariableProvider : IEnvironmentVariableProvider
    {
        public string GetEnvironmentVariable(string variable) => Environment.GetEnvironmentVariable(variable);
    }
}
