# asl-help

### Contributing

> [!IMPORTANT]
> | Required software |    |
> | ----------------: | :- |
> | *.NET SDK 10.0+* | `winget install -i Microsoft.DotNet.Sdk.10` |

1. [Fork this repository <sup>↗︎</sup>](https://github.com/ero-qt/wasmux/fork)
2. Clone this repository **recursively**: `git clone --recursive https://github.com/<your-username>/asl-help` → `cd asl-help`
3. Create a new branch for your changes: `git branch issue-67`, `git branch some-feature`
4. Implement your changes:
   1. Keep your changes focused: don't address style changes in a feature or bug fix.
   2. Consider adding tests, especially when fixing a bug: create a test to prove the failure, fix the bug, prove the test turns green.
5. Commit your changes:
   1. `git add -A` (add all changed files at once)  
      `git add ./src/AslHelp/AslHelp.csproj` (add individual files for a more clear commit story)
   2. `git commit -m '<message>'`  
      imperative mood: `'Adds <feature> to <class>'`, `'Fixes <bug>'`, `'Removes unused <thing>'`
6. Push your changes: `git push`
7. Come back here and open a PR!

The wiki is handled automatically.
