state("Goosebumps_DeadOfNight") {}

startup
{
	vars.Log = (Action<object>)(output => print("[Unity Helper Test] " + output));

	#region Helper Setup
	var bytes = File.ReadAllBytes(@"Components\LiveSplit.ASLHelper.bin");
	var type = Assembly.Load(bytes).GetType("ASLHelper.Unity");
	vars.Helper = Activator.CreateInstance(type, timer, this);
	vars.Helper.GameName = "Goosebumps_DeadOfNight";
	#endregion
}

init
{
	vars.Helper.TryOnLoad = (Func<dynamic, bool>)(mono =>
	{
		var srd = mono.GetClass("Progression");
		vars.Helper["event"] = srd.Make<int>("night");

		return true;
	});

	vars.Helper.Load();
}

update
{
	if (!vars.Helper.Update())
		return false;

	vars.Helper.IO.Log(vars.Helper["event"].Current);
}

exit
{
	vars.Helper.Dispose();
}

shutdown
{
	vars.Helper.Dispose();
}
