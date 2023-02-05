# Avoid cultural-dependent string comparison

As explained in [Best Practices for Using Strings](https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings), we recommend that you avoid calling
string comparison methods that substitute default values and instead call methods that require parameters to be explicitly specified.
To determine whether a string begins with a particular substring by using the string comparison rules of the current culture, call
the `StartsWith(String, StringComparison)` method overload with a value of CurrentCulture for its comparisonType parameter.

```cs
if(name.StartsWith("a")){
    // ...
}
```

vs

```cs
if(name.StartsWith("a", StringComparison.Ordinal)){
    // ...
}
```