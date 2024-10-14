using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Assets.Editor
{
    class EditorCatlog : Catlog
    {
        public override void InitializeAssets()
        {
            version = "0.0.0";
            buildDate = 0;
        }

        public override AssetInfo GetAssetInfo(string path)
        {
            if (File.Exists(path))
                return new AssetInfo() { name = path, bundle = 0, };// deps = null };
            return null;
        }
    }

    public class EditorAssetsImpl : IAssets
    {
        string downloadDir;
        string platformName;
        public void Initialize()
        {
            platformName = Utility.GetPlatformName();
            downloadDir = Application.persistentDataPath;
            Debug.Log($"[EditorAssetsImpl] DownloadDir: {downloadDir}");
        }

        public bool AssetExist(string path)
        {
            return File.Exists(path);
        }

        public bool BuildinExists(string file)
        {
            return File.Exists(file);
        }

        Assets.Asset IAssets.CreateAsset(AssetInfo info, Type type)
        {
            return EditorAsset.Create(info, type);
        }

        public Scene CreateScene(AssetInfo info, bool additive)
        {
            return EditorScene.Create(info, additive);
        }

        public string GetDownloadDataPath(string file)
        {
            return Path.Combine(downloadDir, file);
        }

        public string GetDownloadURL(string file)
        {
            return $"{Manager.DownloadURL}/{platformName}/{file}";
        }

        public string GetPlayerDataPath(string file)
        {
            return Path.Combine(Application.dataPath, file);
        }

        public string GetPlayerDataURL(string file)
        {
            return Path.Combine(Application.dataPath, file);
        }

        public Stream OpenRead(BundleInfo info)
        {
            var path = GetDownloadDataPath(info.name);
            if( File.Exists(path))
                return File.OpenRead(path);
            return null;
        }

        public byte[] ReadAllBytes(string file)
        {
            var path = GetDownloadDataPath(file);
            if (File.Exists(path))
                return File.ReadAllBytes(path);
            return null;
        }

    }

    class EditorAsset : Assets.Asset
    {
        public EditorAsset( AssetInfo info, Type _type)
        {
            type = _type;
            pathOrURL = info.name;
        }

        protected override void OnLoad()
        {

        }

        protected override void OnUnload()
        {
            base.OnUnload();
            if (asset == null)
                return;

            if (!(asset is GameObject) && !(asset is Component))
                Resources.UnloadAsset(asset);

            asset = null;
        }

        protected override void OnUpdate()
        {
            if (status != LoadableStatus.Loading) return;

            OnLoaded(AssetDatabase.LoadAssetAtPath(pathOrURL, type));
        }

        public override void LoadImmediate()
        {
            OnLoaded(AssetDatabase.LoadAssetAtPath(pathOrURL, type));
        }

        internal static EditorAsset Create( AssetInfo info, Type type)
        {
            return new EditorAsset(info, type);
        }
    }

    class EditorScene : Scene
    {
        internal static Scene Create( AssetInfo info, bool additive = false)
        {
            //Versions.GetActualPath(ref assetPath);
            var loadmode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var scene = new EditorScene {  pathOrURL = info.name, LoadMode = loadmode };
            return scene;
        }

        protected override void OnLoad()
        {
            PrepareToLoad();
            operation = EditorSceneManager.LoadSceneAsyncInPlayMode(pathOrURL, new LoadSceneParameters { loadSceneMode = LoadMode });
        }
    }
}
