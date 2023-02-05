# About boxing

Boxing is the process of converting a value type to the type object or to any interface type 
implemented by this value type. When the common language runtime (CLR) boxes a value type, it 
wraps the value inside a System.Object instance and stores it on the managed heap. Unboxing 
extracts the value type from the object. Boxing is implicit; unboxing is explicit. The concept 
of boxing and unboxing underlies the C# unified view of the type system in which a value of 
any type can be treated as an object.

In relation to simple assignments, boxing and unboxing are computationally expensive processes. 
When a value type is boxed, a new object must be allocated and constructed. To a lesser degree, 
the cast required for unboxing is also expensive computationally.

## Typical boxing patterns on string operations:

```cs
int a = 42;
string answer = string.Format("{0:c2}", a); // boxing here
// ...
int a = 2; int b = 3;
string question = string.Concat(a," + ", b); // boxing here
```

This is how it looks in MSIL:

```
 IL_0000: ldstr "{0}"
IL_0005: ldarg.1
IL_0006: box [System.Runtime]System.Int32
IL_000b: call string [System.Runtime]System.String::Format(string, object)
// ...
IL_0000: ldarg.1
IL_0001: box [System.Runtime]System.Int32
IL_0006: ldstr " + "
IL_000b: ldarg.2
IL_000c: box [System.Runtime]System.Int32
IL_0011: call string [System.Runtime]System.String::Concat(object, object, object)
```

You can use the following code to avoid boxing:
```cs
string answer = a.ToString("c2");
```