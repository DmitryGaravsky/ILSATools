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

## Typical boxing patterns on Dictionary\ConcurrentDictionary\SortedDictionary\SortedList operations:

```cs
Dictionary<Key,string> cache = new Dictionary<Key,string>();
//..
struct Key {
    public Key(int value){
        this.value = value;
    }
    int value;
}
//
cache.TryGetValue(new Key(42), out string name); // boxing inside TryGetValue
```

You can avoid boxing if you provide the Key struct with `GetHashCode\Equals` implementation:

```cs
struct Key : IEquatable<Key>{
    readonly int value;
    public Key(int value){
        this.value = value;
    }
    public override int GetHashCode() {
        return value;
    }
    public override bool Equals(object obj) {
        return (obj is Key key) && Equals(key);
    }
    public bool Equals(Key other) {
        return value == other.value;
    }
}
```