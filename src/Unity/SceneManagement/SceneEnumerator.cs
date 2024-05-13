namespace AslHelp.SceneManagement;

public class SceneEnumerator : IEnumerable<Scene>
{
    private readonly int _offset;
    private readonly List<Scene> _sceneCache = new(16);

    private uint _tick;

    internal SceneEnumerator(int offset)
    {
        _offset = offset;
    }

    public int Count => NeedsUpdate() ? this.Count() : _sceneCache.Count;

    public Scene this[int index]
    {
        get
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("Index must be a positive number.");
            }

            int i = 0;
            foreach (Scene scene in this)
            {
                if (i == index)
                {
                    return scene;
                }

                i++;
            }

            throw new ArgumentOutOfRangeException("Index was outside the bounds of the scenes array.");
        }
    }

    private bool NeedsUpdate()
    {
        return _tick != Unity.Instance.Tick;
    }

    private void Update()
    {
        _tick = Unity.Instance.Tick;
    }

    public IEnumerator<Scene> GetEnumerator()
    {
        if (!NeedsUpdate())
        {
            foreach (Scene scene in _sceneCache)
            {
                yield return scene;
            }

            yield break;
        }

        Update();

        lock (_sceneCache)
        {
            _sceneCache.Clear();

            for (int i = 0; i < 16; i++)
            {
                Scene scene = new(_offset, Unity.Instance.PtrSize * i);

                if (!scene.IsValid)
                {
                    yield break;
                }

                _sceneCache.Add(scene);
                yield return scene;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
