# Insecure Type Resolution

Some API can be used to dynamically obtain a type by its name. 

### Affected API

```
// methods
System.Type.GetType
System.Activator.CreateInstance
System.Reflection.Assembly.GetType
System.Runtime.Serialization.FormatterServices.GetTypeFromAssembly
```

You can use the `GetType` method to obtain a Type object for a type in another assembly if you know 
its assembly-qualified name, which can be obtained from `AssemblyQualifiedName`. GetType causes **loading** 
of the assembly specified in typeName. 
You can also load an assembly using the `Assembly.Load` method, and then use the `Assembly.GetType` method to get Type objects. 