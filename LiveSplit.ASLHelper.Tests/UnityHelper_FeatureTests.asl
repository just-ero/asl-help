state("LiveSplit") {}

startup
{
	vars.Log = (Action<object>)(output => print("[Unity Helper Test] " + output));

	#region Helper Setup
	var bytes = File.ReadAllBytes(@"Components\LiveSplit.ASLHelper.bin");
	var type = Assembly.Load(bytes).GetType("ASLHelper.Unity");
	vars.Helper = Activator.CreateInstance(type, timer, this);
	#endregion
}

init
{
	vars.Helper.TryOnLoad = (Func<dynamic, bool>)(mono =>
	{
		var myClass = mono.GetClass(/* assembly name */, /* class name */, /* optional: inheritance depth */);
		return true;
	});

	vars.Helper.Load();
}

update
{
	if (!vars.Helper.Loaded)
		return false;
}

exit
{
	vars.Helper.Dispose();
}

shutdown
{
	vars.Helper.Dispose();
}
