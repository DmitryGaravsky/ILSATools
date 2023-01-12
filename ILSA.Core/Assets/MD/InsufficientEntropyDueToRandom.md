# Insufficient Entropy

The software uses an algorithm or scheme that produces insufficient entropy, leaving patterns or clusters of values that are more likely to occur than others.

Standard random number generators `System.Random` do not provide a sufficient amount of entropy when used for security purposes. 
Attackers can brute force the output of pseudorandom number generators such as rand(). 

### Affected API

```
// types
System.Random

```

If this random number is used where security is a concern, such as generating a session key or session identifier, 
use a trusted cryptographic random number generator instead (like `DevExpress.Data.Utils.Security.StrongRandom`). 

Or use the `DevExpress.Data.Utils.NonCryptographicRandom` in other cases.