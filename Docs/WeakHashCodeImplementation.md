# Weak (XOR-based) GetHashCode implementation

Frequently, a type has multiple data fields that can participate in generating the hash code. 
One way to generate a hash code is to combine these fields using an XOR (eXclusive OR) operation

Providing a good hash function on a class can significantly affect the performance of adding those objects to a hash table. 
In a hash table with keys that provide a good implementation of a hash function, searching for an element takes constant 
time (for example, an O(1) operation). In a hash table with a poor implementation of a hash function, the performance 
of a search depends on the number of items in the hash table (for example, an O(n) operation, where n is the number of items 
in the hash table). A malicious user can input data that increases the number of collisions, which can significantly degrade 
the performance of applications that depend on hash tables, under the following conditions:

- When hash functions produce frequent collisions.
- When a large proportion of objects in a hash table produce hash codes that are equal or approximately equal to one another.
- When users input the data from which the hash code is computed.

### Affected API

```
// members
GetHashCode

```

Consider using the `DevExpress.Data.Utils.HashCodeHelper` or `System.HashCode` instead.

It is best-practice to consider hash codes as an implementation detail, as the implementation may change across assembly versions. 
Do not store hash codes produced by HashCode in serialized structures, for example, on-disk. 
HashCode uses a statically initialized random seed to enforce this best practice, meaning that the hash codes are only deterministic 
within the scope of an operating system process.