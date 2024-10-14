using System;
using System.IO;
using UnityEngine;

namespace Assets.Editor
{
    public class EmptyScene : Assets.Scene
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void InitializeMainScene()
        {
            if (Manager.PlayModel != AssetsModel.Editor)
                return;
            var cur_sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var scene = new EmptyScene() { SceneName = cur_sc.name, pathOrURL = cur_sc.path, LoadMode = UnityEngine.SceneManagement.LoadSceneMode.Single };
            scene._reference.Retain();
            Assets.Scene.Main = scene;
        }
    }

    public static class Initializer
    {
        const string StartScene = "Assets/Scenes/LoadScene.unity";
        /// <summary>
        /// 编辑器下初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var settings = BuildSettings.GetInstance();
            var playModel = settings == null ? AssetsModel.Editor : settings.PlayModel;
            if( playModel == AssetsModel.Editor)
            {
                InitilaizeWithEditor();
            }
            else
            {
                var active = settings.GetActiveSetting();
                if( active == null)
                {
                    InitilaizeWithEditor();
                }
                else if( playModel == AssetsModel.EditorAB)
                {
                    InitializeSetting(active);
                }
                else
                {
                    Manager.Initialize( new OtherPlatformAssetsImpl( Application.streamingAssetsPath));
                }
            }
        }

        /// <summary>
        /// 采用编辑器模式启动
        /// </summary>
        public static void InitilaizeWithEditor()
        {
            /// 设置启动配置
            var config = new AssetsSetting.Config() { Offline = true, BuildDate = 0, PlayModel = AssetsModel.Editor };
            config.LoadScene = StartScene;
            Manager.Initialize(new EditorAssetsImpl(), config);
            Manager.SetCatlog(new EditorCatlog());
        }

        /// <summary>
        /// 采用指定版本ab 启动
        /// </summary>
        /// <param name="activeSetting"></param>
        public static void InitializeSetting( BuildSetting activeSetting)
        {
            /// 设置启动配置
            var impl = new OtherPlatformAssetsImpl(activeSetting.GetPrePublish());
            var config = new AssetsSetting.Config() { BuildDate = activeSetting.CurBuildData, Offline = true, PlayModel = AssetsModel.EditorAB, Version = activeSetting.Version };
            config.LoadScene = StartScene;
            Manager.Initialize(impl, config);
        }
    }
}