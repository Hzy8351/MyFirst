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
        /// �༭���³�ʼ��
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
        /// ���ñ༭��ģʽ����
        /// </summary>
        public static void InitilaizeWithEditor()
        {
            /// ������������
            var config = new AssetsSetting.Config() { Offline = true, BuildDate = 0, PlayModel = AssetsModel.Editor };
            config.LoadScene = StartScene;
            Manager.Initialize(new EditorAssetsImpl(), config);
            Manager.SetCatlog(new EditorCatlog());
        }

        /// <summary>
        /// ����ָ���汾ab ����
        /// </summary>
        /// <param name="activeSetting"></param>
        public static void InitializeSetting( BuildSetting activeSetting)
        {
            /// ������������
            var impl = new OtherPlatformAssetsImpl(activeSetting.GetPrePublish());
            var config = new AssetsSetting.Config() { BuildDate = activeSetting.CurBuildData, Offline = true, PlayModel = AssetsModel.EditorAB, Version = activeSetting.Version };
            config.LoadScene = StartScene;
            Manager.Initialize(impl, config);
        }
    }
}