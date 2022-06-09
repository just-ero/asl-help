state("LiveSplit") {}

startup
{
	vars.Log = (Action<object>)(output => print("[Helper Test] " + output));

	#region Helper Setup
	var bytes = File.ReadAllBytes(@"Components\LiveSplit.ASLHelper.bin");
	var type = Assembly.Load(bytes).GetType("ASLHelper.Main");
	vars.Helper = Activator.CreateInstance(type, timer, this);
	#endregion
}

shutdown
{
	vars.Helper.Dispose();
}