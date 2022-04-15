using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _current;

    public static T Current
    {
        get { return _current ?? (_current = FindT()); }
    }

    static T FindT()
    {
        T t = FindObjectOfType<T>();
        if (!t)
        {
            t = GetSceneObjectOfType<T>();
        }
        return t;
    }

    protected virtual void Awake()
    {
        _current = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (_current == this) _current = null;
    }

    public static T GetSceneObjectOfType<T>() where T : Component
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene != null)
        {
            var rootObjs = scene.GetRootGameObjects();
            var obj = rootObjs.Select(ro => ro.GetComponentInChildren<T>(true)).FirstOrDefault(u => u);
            return obj;
        }
        return default;
    }
}