(based on [dotnet/runtime/CONTRIBUTING.md](https://github.com/dotnet/runtime/blob/main/CONTRIBUTING.md))

# Contributing to asl-help
You can contribute to asl-help by creating issues or opening pull requests. Simply filing issues for problems you encounter is a great way to contribute. Contributing or fixing implementations is greatly appreciated.

## Creating Issues
asl-help always welcomes bug reports, API proposals, and overall feedback. Here are a few tips on how you can make reporting your issue as effective as possible.

### Finding Existing Issues
Before filing a new issue, please search the [open issues](https://github.com/just-ero/asl-help/issues) to check whether a similar one already exists.  
If you do find an existing issue, please include your own feedback in the discussion. Consider upvoting the original post, as this indicates a higher priority.

### Writing a Good Bug Report
Good bug reports make it easier for contributors to verify the root cause and underlying problems. The better a bug report, the faster the problem will be resolved. Ideally, a bug report should contain the following information:

* A high-level description of the problem.
* A *minimal reproduction*, i.e. the smallest size of code required to reproduce the wrong behavior.
* A description of the *expected behavior*, contrasted with the *actual behavior* observed.
* Information about the configuration: helper used, game, LiveSplit version and layout, etc.
* Additional information: is it a regression from previous versions? Are there any known workarounds?

When ready to submit a bug report, please use the Bug Report issue template.

#### Why are Minimal Reproductions Important?
A reproduction lets contributors verify the presence of a bug, and diagnose the issue quicker. A minimal reproduction is the smallest possible code/configuration demonstrating that bug. Minimal reproductions are generally preferable since they:

1. Focus debugging efforts on a simple code snippet.
2. Ensure that the problem is not caused by unrelated configurations.
3. Avoid the need to share unwanted code.

#### How to Create a Minimal Reproduction
The best way to create a minimal reproduction is gradually removing code and configurations from a reproducing app, until the problem no longer occurs. A good minimal reproduction:

* Excludes all unnecessary code blocks, methods, variables, files, and components.
* Contains documentation or code comments illustrating expected vs. actual behavior.
* If possible, avoids performing any unneeded calls. Does behavior happen specifically on asl-help Unity, or is asl-help Basic enough?

## Contributing Changes
Project maintainers will merge changes that improve asl-help in any way and align with the goal of the project.

Maintainers will not merge changes that have narrowly-defined benefits, due to risk of compatibility and bloat. There are hundreds of speedrunners who rely on the functionality of asl-help. Changes may be reverted if they are found to be breaking.

### DOs and DON'Ts
Please:
* **DO** follow the coding style (Visual Studio and the .editorconfig should be of help).
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** keep discussions focused. When a new or adjacent topic comes up in an existing issue, it is often better to create a new issue than to side-track the current one.
* **DO** run `dotnet format`, followed by another build of the solution before committing your changes.

Please:
* **DO NOT** create any pull requests without prior filing an issue. Maintainers and other contributors will want to talk about the changes you want to make and come to an agreement.
* **DO NOT** commit code that you did not write. If you come across code that you think may be a good fit for asl-help, file an issue and start a discussion.

### Breaking Changes
Contributions should maintain API signature as best as possible. Contributions that include breaking changes may be rejected immediately. Please file an issue to discuss your idea or change if you believe that it may affect managed code compatibility.

### Suggested Workflow
The following workflow is recommended:

1. Create an issue for your work (this step can be skipped for trivial changes).
   * Reuse an existing issue on the topic, if there is one.
   * Get agreement from maintainers, contributors, and the community that your proposed change is a good one.
   * Clearly state that you are going to take on implementing it, if that's the case. You can request that the issue be assigned to you.  
     *Note:* The issue filer and the implementer don't have to be the same person.
2. Create a personal fork of the repository on GitHub (if you don't already have one).
3. In your fork, create a branch off of main (`git checkout -b mybranch`).
   * Name the branch so that it clearly communicates your intentions, such as `issue-123` or `name-of-new-feature`.
   * Branches are useful since they isolate your changes from incoming changes from upstream. They also enable you to create multiple PRs from the same fork.
   * Create a copy of the file `example.lsdir`, rename the copy to just `.lsdir`, and edit the path in the first line to the full directory path of where your LiveSplit install is placed.
     * This will create a copy of asl-help in the `Components` folder such that you can test more easily.
4. Make and commit your changes to your branch.
5. Build the solution with your changes.
   * Before building, it is recommended that you run `dotnet format` in the directory of the solution.
   * Afterwards, copy the output file `asl-help` into the `Components` folder in your LiveSplit install directory (setting that directory in the `.lsdir` file will do this automatically).
   * Test your changes thoroughly by creating an auto splitter which loads your relevant class from the library and calling your new or changed methods.
6. Create a pull request (PR) against the **`main`** branch.
   * State in the PR description what issue or improvement your change is addressing.
   * Provide a minimal `.asl` script to test your changes.
   * Check if all Continuous Integration checks (WIP) are passing.
7. Wait for feedback or approval of your changes from maintainers, contributors, or community members.
8. If all checks pass and there is no negative feedback on your changes, your PR will be merged.
   * Given you have built the library, your new changes should automatically be distributed to users (a restart of LiveSplit may be necessary).
   * You can safely delete the branch and fork used for making the changes.

### Help Wanted
Issues marked with this label are currently not possible to be worked on due to a lack of knowledge on the maintainers' part. Please, if you have ideas to share, do not hesitate to add a comment on the issue.

### License Agreement
All code contributed via PRs will fall under the same license asl-help uses as a whole.
