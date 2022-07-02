namespace ASLHelper;

public partial class Unity
{
    public override bool Update()
    {
        return Loaded && base.Update();
    }

    public override bool UpdateAll(Process game)
    {
        return Loaded && base.UpdateAll(game);
    }
}
