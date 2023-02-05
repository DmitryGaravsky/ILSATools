# Insecure Type Activation

Some API can be used to dynamically create an instances of the specific type. 

### Affected API

```
// methods
System.Type.InvokeMember
System.Activator.CreateInstance
System.Reflection.Assembly.CreateInstance
System.Reflection.Activator.CreateInstanceFrom
```

The `CreateInstance` methods of various classes, such as `Activator.CreateInstance` and `Assembly.CreateInstance`, 
are specialized forms of `InvokeMember` that create new instances of the specified type.