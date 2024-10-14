
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
            //�ͷ�����ab
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
            // ��ɾ����ǰ���������нڵ�
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

        // ������Ϸ ��Ҫ����һ�ѻ���
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
            // �Ƴ������л��Ѿ����ص�������Դ
            //Assets.Manager.ResetManager();
            // ���� wait for 
            await Awaiters.Seconds(0.1f);
            // ���¼��س�ʼ������
            if (!Application.isPlaying)
                return;
            await Assets.Scene.LoadBuildinScene(START_UP_SCENE, null, false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RunOnStart()
        {
            Application.quitting += Quit;
            // PC ƽ̨�Ż�ȡwants to quit ����
#if UNITY_STANDALONE
            Application.wantsToQuit += OnRequestQuit;
#endif
            // �����˳���������
            ApplicationHelper go = GameObject.FindObjectOfType<ApplicationHelper>();
            if (go == null)
            {
                var tmp = new GameObject(nameof(ApplicationHelper), typeof(ApplicationHelper));
                go = tmp.GetComponent<ApplicationHelper>();
                GameObject.DontDestroyOnLoad(tmp);
            }
        }

        // ���� �ص�����Ϊnull ʱ����� onRequestQuite
        public static System.Action onRequestQuite;

        // �Ƿ�����ESC �������غ���
        public static bool AllowESC = false;
        // �Ƿ񷵻ص��ú���������Ϊnull
        public static bool ImmediatelySetNull = true;
        // android �������ʱ����
        public static System.Action onBackClicked;

        // �ж��Ƿ����˳���Ϸ ��windows �ϲ���Ҫ
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

                // �ж��Ƿ�Ϊpc ƽ̨
                bool isStandlone = Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer;

                // �������pc ƽ̨ �� ��pc ƽ̨������ESC 
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