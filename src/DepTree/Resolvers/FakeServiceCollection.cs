using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DepTree.Resolvers
{
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
