using System;
using UnityEngine;

namespace FXFramework
{
    public interface ISingleton { void Init(); }

    public abstract class Singleton<T> where T : ISingleton
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new Lazy<T>(true).Value;
                    mInstance.Init();
                }
                return mInstance;
            }
        }
    }
    /// <summary>
    /// Mono†ÎÀý
    /// </summary>
    public abstract class MonoSingle<T> : MonoBehaviour where T : Component
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new Lazy<GameObject>(new GameObject()).Value.AddComponent<T>();
                    mInstance.gameObject.name = typeof(T).Name;
                }
                return mInstance;
            }
        }

        protected virtual void Awake()
        {
            if (mInstance != null) GameObject.Destroy(gameObject);
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
}