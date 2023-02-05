# Avoid using the Assembly.GetExecutingAssembly() method

- At-first, `typeof(T).Assembly` is significantly faster. 
- At-second, `Assembly.GetExecutingAssembly()` returns the currently executing assembly, which can be (in case of an inlined methodcall/property access) something unpredictable different.