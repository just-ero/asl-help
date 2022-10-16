namespace AslHelp.Data;

internal static class Messages
{
    public const string WELCOME = """
        Thank you for using asl-help, created by Ero! For more information, see https://github.com/just-ero/asl-help.
        This library contains many features created for helping out ASL developers.

        If you would like to opt out of code generation, please use the following code in 'startup {}' instead.
        Make sure to call GetType() with the name of the specific helper you would like to use.
            var type = Assembly.Load(File.ReadAllBytes(@"Components\asl-help")).GetType("Basic");
            vars.Helper = Activator.CreateInstance(type, args: false);

        If you have any questions, please tag @Ero#1111 in the #auto-splitters channel
        of the Speedrun Tool Development Discord server: https://discord.gg/cpYsxz7.
        """;
}
