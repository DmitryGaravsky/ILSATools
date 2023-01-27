# Simple static analysis(SA) library for .Net

Library for creating simple static analysis(SA) diagnostics and analyzing .Net assemblies at the IL-code level.
Library provides a predefined subset of patterns for analyzing security\performance or compatibility threats.

<a href="https://www.nuget.org/packages/ILSA.Core/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/ILSA.Core.svg" data-canonical-src="https://img.shields.io/nuget/v/ILSA.Core.svg" style="max-width:100%;" /></a>

Powered by [ILReader](https://github.com/DmitryGaravsky/ILReader) library.

## How to create a simple pattern:

At first, create a class library and reference to [ILReader.Core](https://www.nuget.org/packages/ILReader.Core/) and [ILSA.Core](https://www.nuget.org/packages/ILSA.Core/) nuget packages:

```xml
  <ItemGroup>
    <PackageReference Include="ILReader.Core" Version="1.0.0.*" />
    <PackageReference Include="ILSA.Core" Version="1.0.0.*" />
  </ItemGroup>
```

Once you have the class library set up and the necessary nuget packages referenced, you can create a static class for diagnostic with
a single static `Match` method. This method will define a pattern for specific rule and pass this pattern into
the static `MethodBodyPattern.Match` method available from the `ILSA.Core` library:

```cs
public static class BoxingOnStringMethodCalls {
  static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
      new Func<IInstruction, bool>(i => Boxing.IsBoxing(i.OpCode) && Boxing.IsValueType(i.Operand)),
      new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsFormatOrConcatMethod(i.Operand)),
  };
  static bool IsFormatOrConcatMethod(object? operand) {
      if(!(operand is MethodBase method) || method.DeclaringType != typeof(string))
          return false;
      return method.Name == "Format" || method.Name == "Concat";
  }
  public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
    return MethodBodyPattern.Match(matches, instructions, errors, out captures);
  }
}
```

To implement some rules you can also use some helper methods from the `ILSA.Core` library:

```cs
static readonly Type hashsetType = typeof(HashSet<>);
static bool IsHashSetMethod(object? operand) {
  return operand is MethodBase method && Call.IsMethodOfGenericType(method, hashsetType);
}
```

With the use of the annotation attribute `Display`, you can decorate your diagnostic with a name and a descriptive text (in markdown format)
for further displaying in visual client.

```cs
[Display(Order = (int)ProcessingSeverity.Informational, 
  Name = @"Avoid using HashSet.Add\HashSet.Remove after HashSet.Contains call",
  Description = "ILSA.Core.Assets.MD.AvoidUsingHashSetAddRemoveAfterContainsCheck.md")]
public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) { ... }
```

The corresponding markdown file should be embedded into you library as `EmbeddedResource`.

You can test your diagnostic by [downloading a Visual Client (free)](https://github.com/DmitryGaravsky/ILSATools/releases/tag/EAP) and loading a library with diagnostic into patterns section.

## Real usage

- [Simple static analysis(SA) tool to analyze .Net assemblies at the IL-code level with visual client](https://github.com/DmitryGaravsky/ILSATools) 

Read the [Why simple Static Analysys tools are important and how easy to use they are](/Articles/01-Simple-Static-Analysis.md) article to learn how it works and how to use this tool effectively.

### NuGet

To install [ILSA.Core](https://www.nuget.org/packages/ILSA.Core), run the following command in the Package Manager Console:

    Install-Package ILSA.Core


### License

The ILSA.Core library is licensed under the [MIT](https://github.com/DmitryGaravsky/ILSA.Core/blob/master/LICENSE.TXT) license.
