Addresses Issue # *(remove if not applicable)*

# Description
Describe what changes you are introducing. If you are not addressing an issue, please also explain your motivation behind these changes.

# Testing
*(remove if not applicable)*
Please provide a minimal code example which tests all of the features you modified or added.
```cs
startup
{
  Assembly.Load(File.ReadAllBytes("Components/asl-help")).CreateInstance("Basic");
}

// ...
```

# Risk
Do your changes risk breaking existing methods or auto splitters? If so, can this easily be fixed? Please provide those fixes.

---
- [x] I have read and accept asl-help's license.
