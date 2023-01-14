namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using BF = System.Reflection.BindingFlags;

    partial class ClassesFactory {
        sealed class TypeNode : Node<Type> {
            public TypeNode(IClassesFactory factory, Type type)
                : base(factory, type) {
            }
            protected sealed override string GetName() {
                return source.Name;
            }
            protected override string GetGroup() {
                return source.Namespace ?? string.Empty;
            }
            public bool IsNested {
                get { return source.IsNested; }
            }
            readonly StringBuilder ErrorsBuilder = new StringBuilder();
            protected internal override StringBuilder GetErrorsBuilder() {
                return ErrorsBuilder;
            }
            string? errors;
            [Display(AutoGenerateField = false)]
            public string Errors {
                get { return errors ?? (errors = ErrorsBuilder.ToString()); }
            }
            protected internal override bool HasErrors {
                get { return ErrorsBuilder.Length > 0; }
            }
            protected internal override void Reset() {
                errors = null;
                ErrorsBuilder.Clear();
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var flags = BF.Instance | BF.Static | BF.Public | BF.NonPublic | BF.DeclaredOnly;
                var methods = source.GetMethods(flags);
                Array.Sort(methods, Diagnostics.MethodNamesComparer.Instance);
                var ctors = source.GetConstructors(flags);
                var nestedTypes = source.GetNestedTypes(BF.Public | BF.NonPublic);
                Array.Sort(nestedTypes, Diagnostics.TypeNamesComparer.Instance);
                int methodsCount = nestedTypes.Length + ctors.Length + methods.Length;
                bool hasBaseTypes = (source.BaseType != null) && (source.BaseType != typeof(object));
                var nodes = new Node[methodsCount + (hasBaseTypes ? 1 : 0)];
                int index = 0;
                if(hasBaseTypes)
                    nodes[index++] = factory.BaseTypes(source);
                foreach(var n in nestedTypes)
                    nodes[index++] = factory.Create(n);
                foreach(var c in ctors)
                    nodes[index++] = factory.Create(c);
                foreach(var m in methods)
                    nodes[index++] = factory.Create(m);
                return nodes;
            }
            public sealed override int TypeCode {
                get {
                    if(source.IsValueType) {
                        if(source.IsEnum)
                            return source.IsPublic ? (int)NodeType.Enumeration : (int)NodeType.EnumerationPrivate;
                        else
                            return source.IsPublic ? (int)NodeType.ValueType : (int)NodeType.ValueTypePrivate;
                    }
                    if(source.IsInterface)
                        return source.IsPublic ? (int)NodeType.Interface : (int)NodeType.InterfacePrivate;
                    else
                        return source.IsPublic ? (int)NodeType.Type : (int)NodeType.TypePrivate;
                }
            }
        }
    }
}