using System;

namespace Lively.Resolvers
{
    public class NoInterfaceResolver : IInterfaceResolver
    {
        public Type Resolve(Type t)
        {
            return null;
        }

        public static IInterfaceResolver Create(DependencyTreeConfig config = null) => new NoInterfaceResolver();
    }
}
