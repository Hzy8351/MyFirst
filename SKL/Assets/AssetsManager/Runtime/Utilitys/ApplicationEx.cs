
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class ApplicationEx
    {
        const string START_UP_SCENE = "LoadScene";
        static public bool isQuiting = false;

        static void Quit()
        {
            isQuiting = true;
            Debug.Log("Application is Quiting!");
            //Network.Connection.RemoveAll();
#if UNITY_EDITOR
            //释放所有ab
            Assets.Bundle.ApplicationStop();
#endif
        }

        public static void QuiteGame()
        {
            WantToQuit = true;
#if UNITY_EDITOR
            if (Application.isPlaying)
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void RemoveAllGameObject()
        {
            // 先删掉当前场景的所有节点
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    if (root.hideFlags > 0)
                        continue;
                    GameObject.DestroyImmediate(root);
                }
            }
        }

        // 重启游戏 需要重置一堆环境
        public static void RestartGame()
        {
            // clear somthing neet delay
            DelayRestart();
        }

        static async void DelayRestart()
        {
            await Awaiters.Seconds(0.1f);
            // release assets
            onRequestQuite = onBackClicked = null;
            // 移除加载中或已经加载的所有资源
            //Assets.Manager.ResetManager();
            // 重启 wait for 
            await Awaiters.Seconds(0.1f);
            // 重新加载初始化场景
            if (!Application.isPlaying)
                return;
            await Assets.Scene.LoadBuildinScene(START_UP_SCENE, null, false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RunOnStart()
        {
            Application.quitting += Quit;
            // PC 平台才获取wants to quit 函数
#if UNITY_STANDALONE
            Application.wantsToQuit += OnRequestQuit;
#endif
            // 创建退出按键监听
            ApplicationHelper go = GameObject.FindObjectOfType<ApplicationHelper>();
            if (go == null)
            {
                var tmp = new GameObject(nameof(ApplicationHelper), typeof(ApplicationHelper));
                go = tmp.GetComponent<ApplicationHelper>();
                GameObject.DontDestroyOnLoad(tmp);
            }
        }

        // 当后撤 回调函数为null 时会调用 onRequestQuite
        public static System.Action onRequestQuite;

        // 是否允许ESC 触发返回函数
        public static bool AllowESC = false;
        // 是否返回调用后立即设置为null
        public static bool ImmediatelySetNull = true;
        // android 点击返回时触发
        public static System.Action onBackClicked;

        // 判断是否想退出游戏 在windows 上才需要
        static bool WantToQuit = false;
        static bool OnRequestQuit()
        {
            if (onRequestQuite != null && WantToQuit == false)
            {
                onRequestQuite.Invoke();
                return false;
            }
            return true;
        }


        class ApplicationHelper : MonoBehaviour
        {
            void Update()
            {
                if (!Input.GetKeyDown(KeyCode.Escape))
                    return;

                // 判断是否为pc 平台
                bool isStandlone = Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer;

                // 如果不是pc 平台 或 是pc 平台且允许ESC 
                if ( !isStandlone || (isStandlone && AllowESC))
                {
                    if (onBackClicked != null)
                    {
                        onBackClicked();
                        if (ImmediatelySetNull) onBackClicked = null;
                        return;
                    }
                }
                onRequestQuite?.Invoke();
            }

        }
    }
}