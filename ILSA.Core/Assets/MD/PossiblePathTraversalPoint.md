# Possible Path Traversal point

Many file operations are intended to take place within a restricted directory.
By using special elements such as ".." and "/" separators, attackers can escape outside of
the restricted location to access files or directories that are elsewhere on the system. 
ne of the most common special elements is the "../" sequence, which in most modern operating
systems is interpreted as the parent directory of the current location. This is referred to as
relative path traversal. Path traversal also covers the use of absolute pathnames such as "/usr/local/bin",
which may also be useful in accessing unexpected files. This is referred to as absolute path traversal.

### Affected API

```
// Methods
File.OpenText
File.CreateText
File.AppendText
File.Create
File.Delete
File.Open
File.OpenRead
File.OpenWrite
File.ReadAllText
File.WriteAllText
File.ReadAllBytes
File.WriteAllBytes
File.ReadAllLines
File.ReadLines
File.WriteAllLines
File.AppendAllText
File.AppendAllLines
File.Decrypt
File.Encrypt
Path.Combine
// Constructors
.ctor StreamReader
.ctor StreamWriter
.ctor FileStream
```

## List of Mapped CWEs

- [CWE-22: Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')](https://cwe.mitre.org/data/definitions/22.html)
- [CWE-23: Relative Path Traversal](https://cwe.mitre.org/data/definitions/23.html)
- [CWE-35: Path Traversal: '.../...//'](https://cwe.mitre.org/data/definitions/35.html)
