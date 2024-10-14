using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T Instance;

    public static T instance
    {
        get
        {
            if (Instance != null) return Instance;

            Instance = FindObjectOfType<T>();

            if (Instance == null)
            {
                new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
            }
            else
            {
                Instance.Init();
            }

            return Instance;

        }
    }

    private void Awake()
    {
        Instance = this as T;
        Init();
    }

    protected virtual void Init()
    {
    }

}
