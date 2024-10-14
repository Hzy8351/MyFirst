using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class Scene : Loadable, IEnumerator
    {
        public static Action<Scene> onSceneUnloaded;
        public static Action<Scene> onSceneLoaded;

        static readonly List<AsyncOperation> Progressing = new List<AsyncOperation>();

        public static Scene Main { get; protected set; }
        /// <summary>
        /// scene name
        /// </summary>
        public string SceneName { protected set; get; }

        /// <summary>
        /// 叠加场景
        /// </summary>
        List<Scene> additives = new List<Scene>();

        /// <summary>
        /// 加载完成回调
        /// </summary>
        public Action<Scene> completed;

        /// <summary>
        /// 加载中回调
        /// </summary>
        public Action<Scene> updated;

        /// <summary>
        /// 异步操作
        /// </summary>
        protected AsyncOperation operation { get; set; }

        /// <summary>
        /// 场景加载方式
        /// </summary>
        public LoadSceneMode LoadMode { get; set; }

        /// <summary>
        /// SceneObject
        /// </summary>
        public UnityEngine.SceneManagement.Scene uScene { get; protected set; }

        /// <summary>
        /// 父节点场景
        /// </summary>
        Scene parent { get; set; }

        /// <summary>
        /// 是否自动激活
        /// </summary>
        bool _auto_active = true;
        public bool AutoActive 
        { 
            get => _auto_active;
            set
            {
                _auto_active = value;
                if (_auto_active && IsDone())
                    ActiveScene();
            }
        }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public object Current => null;
        void IEnumerator.Reset() { }

        /// <summary>
        /// 异步操作
        /// </summary>
        public Task<Scene> Task
        {
            get
            {
                var tcs = new TaskCompletionSource<Scene>();
                completed += operation => { tcs.SetResult(this); };
                return tcs.Task;
            }
        }

        static bool IsBuildingScene( string path)
        {
            for(int i=0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene.IsValid() && scene.path == path)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 运行内置场景
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static Scene LoadBuildinScene( string sceneName, Action<Scene> completed = null, bool additive = false)
        {
            var scene = new Scene() { pathOrURL = sceneName, LoadMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single };
            scene.completed += completed;
            scene.Load();
            return scene;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="completed"></param>
        /// <param name="additive"></param>
        /// <returns></returns>
        public static Scene LoadAsync(string assetPath, Action<Scene> completed = null, bool additive = false)
        {
            if (string.IsNullOrEmpty(assetPath)) 
                throw new ArgumentNullException(nameof(assetPath));

            /// 如果是buildin scene 那么直接从buildin 中创建
            Scene scene;
            if (IsBuildingScene(assetPath))
            {
                scene = new Scene() { pathOrURL = assetPath, LoadMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single };
            }
            else
            {
                scene = Manager.CreateScene(assetPath, additive);
            }
            if (completed != null)
            {
                scene.completed += completed;
            }
            scene.Load();
            return scene;
        }

        public static Scene LoadAdditiveAsync(string assetPath, Action<Scene> completed = null)
        {
            return LoadAsync(assetPath, completed, true);
        }

        public static bool IsProgressing()
        {
            for (var i = 0; i < Progressing.Count; i++)
            {
                var item = Progressing[i];
                if (item != null && !item.isDone) 
                    return true;

                Progressing.RemoveAt(i);
                i--;
            }
            return false;
        }

        public void AsyncLoadScene()
        {
            status = LoadableStatus.Loading;
            operation = SceneManager.LoadSceneAsync(SceneName, LoadMode);
            if (operation != null)
            {
                Progressing.Add(operation);
            }
        }

        protected override void OnUpdate()
        {
            if (status != LoadableStatus.Loading)
                return;

            UpdateLoading();
            updated?.Invoke(this);
        }

        protected void UpdateLoading()
        {
            if (operation == null)
            {
                Finish("[Scene] UpdateLoading: operation is null!");
                return;
            }

            progress = 0.5f + operation.progress * 0.5f;

            if (operation.allowSceneActivation)
            {
                if (!operation.isDone) 
                    return;
            }
            else
            {
                // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
                if (operation.progress < 0.9f) 
                    return;
            }
            /// 调用完成
            Finish();
            /// check auto active
            ActiveScene();
        }

        protected override void OnLoad()
        {
            /// 准备场景关联数据
            PrepareToLoad();
            /// 加载场景
            AsyncLoadScene();
        }

        protected void PrepareToLoad()
        {
            SceneName = Path.GetFileNameWithoutExtension(pathOrURL);
            if (LoadMode == LoadSceneMode.Single)
            {
                Main?.Release();
                Main = this;
            }
            else
            {
                if (Main != null)
                {
                    Main.additives.Add(this);
                    parent = Main;
                }
            }
        }

        protected override void OnUnload()
        {
            completed = null;
            // do unload
            if (LoadMode == LoadSceneMode.Additive)
            {
                parent?.additives.Remove(this);
                parent = null;
                if ( uScene.IsValid())
                {
                    var opr = SceneManager.UnloadSceneAsync(uScene);
                    if (opr != null)
                    {
                        Progressing.Add(opr);
                    }
                }
            }
            else
            {
                foreach (var item in additives)
                {
                    item.Release();
                    item.parent = null;
                }
                additives.Clear();
            }

            onSceneUnloaded?.Invoke(this);
        }

        protected override void OnComplete()
        {
            onSceneLoaded?.Invoke(this);

            if (completed == null)
                return;
            var saved = completed;
            completed -= saved;
            saved.Invoke(this);
        }

        public void ActiveScene()
        {
            uScene = SceneManager.GetSceneByName(SceneName);
            if (uScene.name != SceneName || !uScene.IsValid())
                return;
            SceneManager.SetActiveScene(uScene);
        }

        /// <summary>
        /// 查找根节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UnityEngine.Object GetRootObject( string name, Type type = null)
        {
            if (!uScene.IsValid())
                return null;
            var objects = uScene.GetRootGameObjects();
            if (type == null)
            {
                foreach (var o in objects)
                {
                    if (o.name == name)
                        return o;
                }
            }
            else
            {
                foreach( var o in objects)
                {
                    var com = o.name == name ? o.GetComponent(type) : null;
                    if(com != null)
                    {
                        return com;
                    }
                }
            }
            return null;
        }
    }
}