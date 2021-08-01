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

            foreach (var (nodeType, implementationType) in flattenedNodes.TypesAndImplementations())
            {
                AppendClassOrInterfaceImplementation(builder, nodeType, implementationType);
            }

            builder.AppendLine();

            foreach (var (nodeType, childType, count) in flattenedNodes.Relationships())
            {
                var implementationType = flattenedNodes.Implementation(nodeType);
                var nodeTypeName = TypeNameSelector(implementationType ?? nodeType);
                var childTypeName = TypeNameSelector(childType);

                AppendDependentRelationship(builder, nodeTypeName, childTypeName, count);
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

        private void AppendClassOrInterfaceImplementation(StringBuilder builder, ITypeDescription nodeType, ITypeDescription implementationType)
        {
            if (implementationType == null)
            {
                AppendClass(builder, nodeType);
                return;
            }

            var interfaceTypeName = TypeNameSelector(nodeType);
            var implementationTypeName = TypeNameSelector(implementationType);

            builder.AppendLine($"interface {interfaceTypeName} {{");
            foreach (var method in nodeType.Methods)
                builder.AppendLine($"  +{method.Name}()");
            builder.AppendLine("}");

            AppendClass(builder, implementationType);

            builder.AppendLine($"{interfaceTypeName} <--- {implementationTypeName}");
        }

        private void AppendClass(StringBuilder builder, ITypeDescription nodeType)
        {
            var nodeName = TypeNameSelector(nodeType);
            builder.AppendLine("class " + nodeName + " {");
            foreach (var method in nodeType.Methods)
                builder.AppendLine($"  +{method.Name}()");
            builder.AppendLine("}");
        }

        private string TypeNameSelector(ITypeDescription type)
            => _typeNameSelector(type)
                .Replace('+', '.')
                .Replace("`", "");
    }
}
