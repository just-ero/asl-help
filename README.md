# asl-help
Welcome!

## Purpose
Asl-help was created to aid autosplitter authors in writing scripts for games that use the mono framework, specifically the Unity fork. It is packaged as a .NET framework library that can be loaded at runtime from an autosplitter script, and uses codegen to allow for the creation of more streamlined, human-readable, and update-resilient pointer paths.

## Usage 
### Devopment Setup
Download the `asl-help` file located in the `lib` directory of this repository and place it in LiveSplit's components folder. This is only necessary for development—the file will be downloaded automatically for end users, provided that you configure your autosplitter correctly (see below).
### Loading Asl-Help
Use the following snippet to load the asl-help binary in your autosplitter. This and the following examples will assume that the game runs on the Unity engine.
```cs
startup {
    Assembly.Load(File.ReadAllBytes("Components/asl-help")).CreateInstance("Unity");
}
```
### TryLoad
All pointers must be declared inside of a function that is assigned to the `TryLoad` member of the auto-generated `Helper` variable. This should be done inside the `init` block. If this function returns `true`, then asl-help has been successfully initialized.
```cs
init {
    vars.Helper.TryLoad = (Func<dynamic, bool>)(mono => {
        // Declare your pointers here.
        return true;
    });
```
Because the `mono` variable is scoped to the anonymous function, it cannot be accessed outside of the function. Do not store a reference to it that could outlive said function.
### Pointers
Pointers in asl-help are declared with the following syntax inside of the TryLoad function.
```cs
vars.Helper["WatcherName"] = mono["ClassName"].Make<Type>("FieldName", offset1, offset...);
```
`WatcherName` is the name that will be used to access the MemoryWatcher.  
`ClassName` is the name of the class that owns the field that is to be accessed.  
`Type` is the datatype of the value that is to be read from memory.  
`FieldName` is the name of a **static** field on the class. It can also be replaced by a number representing that field's offset in the static field table. Asl-help will print all the static fields of any classes used to the debug output, wich can be used to ensure the name of the field matches said output.  
`offset`, `offset2`, etc. are optional additional pointer offsets that work just like a normal pointer declaration.
### Accessing Pointers
The value of pointers declared this way can be accessed just like pointers declared in the state block: using `current` and `old`. Alternatively, the MemoryWatchers themselves can be accessed from the `vars.Helper` variable.
```cs
// The following two lines are equivalent.
return current.Level != old.Level;
return vars.Helper["Level"].Current != vars.Helper["Level"].Old;
```
### Inheritance
Sometimes, a class's parent class has a static field that must be accessed. This is very common when dealing with singletons, which are often implemented like so:
```cs
abstract class Singleton<T> {
    static T instance;
}

class Manager : Singleton<Manager> {
    // other fields
}
```
To access a field like this, specify a number indicating how many classes in the inheritance tree to go up when declaring the pointer.
```cs
vars.Helper["Manager"] = mono["Manager", 1].Make<IntPtr>("instance");
```
### Publishing
When publishing an autosplitter that depends on asl-help, add the following to the autosplitter's entry in the master XML document inside of the `<URLs>` tag to ensure that it will be automatically downloaded along with the script.
```xml
<URL>https://github.com/just-ero/asl-help/raw/main/lib/asl-help</URL>
```
---
## Third Party Notice
This repository is not responsible for scripts malfunctioning due to issues that are not caused by asl-help.
We reserve the right to immediately close any issues which report problems out of our control.

Instead, please either open an issue in the repository of the script's creator, or ask a question in the [Speedrun Tool Development Discord server](https://discord.gg/cpYsxz7).