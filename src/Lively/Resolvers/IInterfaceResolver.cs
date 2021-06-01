using System;

namespace Lively.Resolvers
{
    public interface IInterfaceResolver
    {
        Type Resolve(Type t);
    }
}
