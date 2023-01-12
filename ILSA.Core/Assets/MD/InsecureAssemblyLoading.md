# Insecure Assembly Loading

Some API can be used to load an assembly with the specified name or from the specific location. 

### Affected API

```
// methods
System.Reflection.Assembly.Load
System.Reflection.Assembly.LoadFrom
System.Reflection.Assembly.LoadFile
System.Reflection.Assembly.LoadWithPartialName
System.Reflection.Assembly.UnsafeLoadFrom
```

Depending on way of passing parameters into these methods this type of weakness can be threated as critical.
Consider using only predefined constants for assembly names\locations.