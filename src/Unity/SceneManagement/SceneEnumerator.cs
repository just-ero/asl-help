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

    public int Count => Update() ? this.Count() : _sceneCache.Count;

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

    private bool Update()
    {
        if (_tick == Unity.Instance.Tick)
        {
            return false;
        }

        _tick = Unity.Instance.Tick;
        return true;
    }

    public IEnumerator<Scene> GetEnumerator()
    {
        if (!Update())
        {
            foreach (Scene scene in _sceneCache)
            {
                yield return scene;
            }

            yield break;
        }

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
