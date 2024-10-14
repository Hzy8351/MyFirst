using System;
using UnityEngine;

namespace Assets
{
    public sealed class Updater : MonoBehaviour
    {
        private static float _realtimeSinceUpdateStartup;
        // frame interval
        [SerializeField] private float _maxUpdateTimeSlice = 0.022f;
        public static float maxUpdateTimeSlice { get; set; }

        public static Updater Instance { get; private set; }

        public static bool Busy()
        {
            return Time.realtimeSinceStartup - _realtimeSinceUpdateStartup >= maxUpdateTimeSlice;
        }

        private void Awake()
        {
            Instance = this;
            maxUpdateTimeSlice = _maxUpdateTimeSlice;
        }

        private void Update()
        {
            OnUpdate();
        }

        private void OnDestroy()
        {
            Manager.ResetManager();
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            var updater = FindObjectOfType<Updater>();
            if (updater != null) return;

            updater = new GameObject("Updater").AddComponent<Updater>();
            DontDestroyOnLoad(updater);
        }

        // private 
        static void OnUpdate()
        {
            _realtimeSinceUpdateStartup = Time.realtimeSinceStartup;
            Loadable.UpdateAll();
            Operation.UpdateAll();
            Download.UpdateAll();
        }

        // 
#if UNITY_EDITOR
        public static void CallOnEditorUpdate()
        {
            OnUpdate();
        }
#endif
    }
}