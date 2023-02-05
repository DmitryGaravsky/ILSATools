# HardCoded Assembly Loading

Some API can be used to load an assembly with the specified name or from the specific location.
When predefined constants for assembly names\locations are used you can threat these places as safe
when you're confident that these locations\names are not compromised.

### Affected API

```
// methods
System.Reflection.Assembly.Load
System.Reflection.Assembly.LoadFrom
System.Reflection.Assembly.LoadFile
System.Reflection.Assembly.LoadWithPartialName
System.Reflection.Assembly.UnsafeLoadFrom
```