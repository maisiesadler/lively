using System;

namespace DepTree.Resolvers
{
    public interface IInterfaceResolver
    {
        Type Resolve(Type t);
    }
}
