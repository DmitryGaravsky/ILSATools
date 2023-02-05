# Breaking changes in .NET 6

If you're migrating an app to .NET 6, the breaking changes listed here might affect you. 

### Affected API

```
// types
System.Net.WebClient
System.Net.WebRequest
System.Net.ServicePoint
System.Security.IPermission
// members
System.AppDomain.CreateDomain
System.AppDomain.Unload
// members
System.Type.ReflectionOnlyGetType
System.Reflection.Assembly.ReflectionOnlyLoad
System.Reflection.Assembly.GlobalAssemblyCache
// members
System.Threading.Thread.Abort
System.Threading.Thread.ResetAbort
// members
System.Uri.EscapeUriString
// members
System.Text.Encoding.UTF7
```

[Breaking changes in .NET 6](https://learn.microsoft.com/en-us/dotnet/core/compatibility/6.0)