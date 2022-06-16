state("TUNIC") {}

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
		var srd = mono.GetClass("SpeedrunData");
		vars.Helper["event"] = srd.MakeString("LastEvent");

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
