using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lively.TypeDescriptions;

namespace Lively.Diagrams
{
    internal class PlantUmlRenderer
    {
        Func<ITypeDescription, string> _typeNameSelector;

        public PlantUmlRenderer(Func<ITypeDescription, string> typeNameSelector)
        {
            _typeNameSelector = typeNameSelector;
        }

        public string Create(IList<DependencyTreeNode> nodes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("@startuml");
            builder.AppendLine();

            var flattenedNodes = FlattenedNodes.Create(nodes);

            foreach (var (nodeType, implementationType, children) in flattenedNodes.Relationships())
            {
                var nodeTypeName = TypeNameSelector(nodeType);
                AppendClass(builder, nodeTypeName, nodeType.Methods);
                foreach (var (childType, childImplementationType, count) in children)
                {
                    var childTypeName = TypeNameSelector(childType);
                    AppendDependentRelationship(builder, nodeTypeName, childTypeName, count);
                }
            }

            var implementations = flattenedNodes.Implementations().ToList();

            if (implementations.Count > 0)
            {
                builder.AppendLine();
            }

            foreach (var (interfaceType, implementationType) in implementations)
            {
                var interfaceTypeName = TypeNameSelector(interfaceType);
                var implementationTypeName = TypeNameSelector(implementationType);

                AppendInterfaceImplementation(builder, interfaceTypeName, implementationTypeName, implementationType.Methods);
            }

            builder.AppendLine();
            builder.AppendLine("@enduml");

            return builder.ToString();
        }

        private void AppendDependentRelationship(StringBuilder builder, string parentName, string childName, int count)
        {
            if (count == 1)
            {
                builder.AppendLine($"{parentName} ---> {childName}");
            }
            else
            {
                builder.AppendLine($"{parentName} ---> \"{count}\" {childName}");
            }
        }

        private void AppendInterfaceImplementation(StringBuilder builder, string interfaceName, string implementationName, IReadOnlyList<TypeMethod> implementationMethods)
        {
            builder.AppendLine($"interface {interfaceName} {{");
            builder.AppendLine("}");

            AppendClass(builder, implementationName, implementationMethods);

            builder.AppendLine($"{interfaceName} <--- {implementationName}");
        }

        private void AppendClass(StringBuilder builder, string nodeName, IReadOnlyList<TypeMethod> methods)
        {
            builder.AppendLine("class " + nodeName + " {");
            foreach (var method in methods)
                builder.AppendLine($"  +{method.Name}()");
            builder.AppendLine("}");
        }

        private string TypeNameSelector(ITypeDescription type)
            => _typeNameSelector(type)
                .Replace('+', '.')
                .Replace("`", "");
    }
}
