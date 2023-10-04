# BinaryFormatter Obsoletion Strategy

Full article: [BinaryFormatter Obsoletion Strategy](https://github.com/dotnet/designs/blob/main/accepted/2020/better-obsoletion/binaryformatter-obsoletion.md)

## Timeline overview (Related to .NET8+)

### .NET 8 (Nov 2023)

- All first-party dotnet org code bases complete migration away from `BinaryFormatter`
- `BinaryFormatter` disabled by default across all project types
- All not-yet-obsolete `BinaryFormatter` APIs marked obsolete _as warning_
- Additional legacy serialization infrastructure marked obsolete _as warning_
- No new `[Serializable]` types introduced (all target frameworks)

### .NET 9 (Nov 2024)

- Remainder of legacy serialization infrastructure marked obsolete _as warning_
- `BinaryFormatter` infrastructure removed from .NET
  - Back-compat switches also removed

### New obsoletions in .NET 8

In .NET 8.0, the following APIs will be marked `[Obsolete]` _as error_ by default. These will utilize the existing `SYSLIB0011` warning code. The errors can be converted back to warnings by using the `<EnableUnsafeBinaryFormatterSerialization>` compat switch within the project file. 

- The entirety of the `BinaryFormatter` type.
- The entirety of the `IFormatter` interface.
- The entirety of the `Formatter` type.

The following APIs will be marked `[Obsolete]` _as warning_ using a new warning code (tentatively `SYSLIB0050`).

**Newly obsolete types**

- `IFormatterConverter` interface and `FormatterConverter` concrete type
- `FormatterServices` static class
- `IObjectReference` interface
- `ISurrogateSelector` and `ISerializationSurrogate` interfaces, and the `SurrogateSelector` concrete type
- `ISafeSerializationData` and `SafeSerializationEventArgs`
- `StreamingContextStates` enum
- `ObjectIDGenerator`
- `ObjectManager`
- `SerializationObjectManager`
- `FormatterTypeStyle`, `FormatterAssemblyStyle`, and `TypeFilterLevel` enums
- `IFieldInfo` interface

(All APIs which expose these types through their public API surface would also be obsoleted.)

**Newly obsolete members on existing types**

- `Type.IsSerializable` public property (and all overridden implementations)
- `TypeAttributes.Serializable` enum value
- `FieldInfo.IsNotSerialized` public property
- `FieldAttributes.NotSerialized` enum value
- `ISerializable.GetObjectData` method, but not the type itself
- `SerializationInfo` and `StreamingContext`, all ctors, but not the types themselves

The following APIs will be marked `[Obsolete]` _as warning_ using a new warning code (tentatively `SYSLIB0051`) and marked `[EditorBrowsable(Never)]`.

- All externally visible (public or protected) serialization ctors: `.ctor(SerializationInfo, StreamingContext)`.
- All public (not explicit or protected) implementations of `IObjectReference.GetRealObject`.
- All public (not explicit or protected) implementations of `ISerializable.GetObjectData`.

The following APIs will not be obsoleted but will be marked `[EditorBrowsable(Never)]`.

- The `SerializableAttribute` type
- The `NonSerializedAttribute` type

These changes only affect applications targeting .NET 8+. If a project targets multiple runtimes, these new annotations will be `#if` conditioned to apply only to .NET 8+.
