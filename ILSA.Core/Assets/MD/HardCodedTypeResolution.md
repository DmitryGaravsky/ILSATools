# Hardcoded Type Resolution

Some API can be used to dynamically obtain a type by its name. 
When predefined constants for type names are used you can threat these places as safe
if you're confident that these types are known\not compromised.

### Affected API

```
// methods
System.Type.GetType
System.Activator.CreateInstance
System.Reflection.Assembly.GetType
System.Runtime.Serialization.FormatterServices.GetTypeFromAssembly
```