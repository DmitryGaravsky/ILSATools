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

## Typical boxing patterns on Enum operations:

```cs
[Flags]
public enum State { One, Two }
//..
public bool IsOne(State state) {
    return state.HasFlag(State.One); // boxing here
}
```

This is how it looks in MSIL:

```
IL_0000: ldarg.1
IL_0001: box C/State
IL_0006: ldc.i4.0
IL_0007: box C/State
IL_000c: call instance bool System.Enum::HasFlag(class System.Enum)
IL_0011: ret
```

You can use the following code to avoid boxing:
```cs
public bool IsOne(State state) {
    return (state & State.One) == State.One; // No boxing here
}
```