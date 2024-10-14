using System;
using System.IO;
using UnityEngine;

namespace Assets
{
    public static class Manager
    {
        /// <summary>
        /// 下载链接
        /// </summary>
        public static string DownloadURL = null;

        /// <summary>
        /// 安全模式 当启动时发生致命错误 
        /// 客户端需要回档到初始包来启动
        /// 避免更新到错误逻辑导致无法启动
        /// </summary>
        public static bool SafeModel { private set; get; }


        /// <summary>
        /// 安全模式 当启动时发生致命错误 
        /// 客户端需要回档到初始包来启动
        /// 避免更新到错误逻辑导致无法启动
        /// </summary>
        public static bool VersionDirty { private set; get; } = false;

        /// <summary>
        /// 运行时配置
        /// </summary>
        static AssetsSetting.Config setting;

        /// <summary>
        /// 当前运行模式
        /// </summary>
        public static AssetsModel PlayModel
        {
            get
            {
                if (setting == null)
                    return AssetsModel.Runtime;
                return setting.PlayModel;
            }
        }

        /// <summary>
        /// 是否为离线模式
        /// </summary>
        public static bool OfflineMode = true;

        /// <summary>
        /// 当前资源版本
        /// </summary>
        public static string Version => Catlog != null ? Catlog.version : Application.version;

        /// <summary>
        /// 当前使用资源构建日期
        /// </summary>
        public static long BuildDate => Catlog != null ? Catlog.buildDate : 0;
        /// <summary>
        /// 获取启动场景
        /// </summary>
        public static string LoadScene
        {
            get
            {
                const string StartScene = "Assets/Scenes/LoadScene.unity";
                if ( setting != null)
                    return setting.LoadScene;
                return StartScene;
            }
        }

        /// <summary>
        /// 当前catlog
        /// </summary>
        internal static Catlog Catlog;

        /// <summary>
        /// 资源加载实例
        /// </summary>
        static IAssets _Impl;

        /// <summary>
        /// 资源加载实例
        /// </summary>
        static SafeModelHandle _safeModelHandle;

        /// <summary>
        /// 初始化资源管理器 
        /// </summary>
        /// <param name="impl"> 资源管理</param>
        /// <param name="config"></param>
        /// <param name="safeModelHandle"></param>
        public static void Initialize( IAssets impl = null, AssetsSetting.Config config = null, SafeModelHandle safeModelHandle = null)
        {
            /// 标记版本是否更新
            VersionDirty = false;
            // 创建运行实例
            if ( _Impl == null)
            {
                /// 进入安全模式
                EnterSafeModel(safeModelHandle);
                /// check impl
                _Impl = impl == null? GetAssetsImpl(): impl;
                /// 调用初始化
                _Impl.Initialize();
                ///获取运行时配置
                setting = GetConfig(config);
                if (setting == null)
                {
                    //throw new Exception("Assets Intialize Failed: setting is nil!");
                    return;
                }

                /// 是否为离线模式
                OfflineMode = setting.Offline;
            }
            /// 编辑器模式
            if (setting.PlayModel == AssetsModel.Editor)
                return;

            /// 初始化资源目录
            var buildin = Catlog.GetBuildinCatlog(setting.BuildDate);
            if (buildin == null)
            {
                //Debug.Log("Assets Initialize Failed: buildin catlog is nil!");
                //throw new Exception("Assets Initialize Failed: buildin catlog is nil!");
                return;
            }
               
            /// 初始化 更新配置
            var update = setting.PlayModel == AssetsModel.Runtime? Catlog.GetUpdateCatlog(): null;
            CompareCatlog(buildin, update);
        }

        //static void UnloadAllBundle()
        //{
        //    /// check unload bundles
        //    var bundles = Resources.FindObjectsOfTypeAll<AssetBundle>();
        //    foreach(var bundle in bundles)
        //    {
        //        if (string.IsNullOrEmpty(bundle.name))
        //            continue;
        //        try
        //        {
        //            bundle.Unload(true);
        //        }
        //        catch { }
        //    }
        //}
        static IAssets GetAssetsImpl()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new AndroidAssetsImpl();
#else
            return new OtherPlatformAssetsImpl(Application.streamingAssetsPath);
#endif
        }

        static AssetsSetting.Config GetConfig(AssetsSetting.Config config)
        {
            if (config != null)
                return config;

            var setting = AssetsSetting.GetInstance();
            if (setting != null)
                return setting.config;

            return null;
        }

        /// <summary>
        /// 设置Catlog 版本
        /// </summary>
        /// <param name="buildin"></param>
        /// <param name="update"></param>
        static void CompareCatlog( Catlog buildin, Catlog update)
        {
            if( update == null || buildin.buildDate >= update.buildDate)
            {
                SetCatlog(buildin);
                return;
            }
            SetCatlog(update);
        }

        public static void SetCatlog( Catlog catlog, bool bCheckDirty = false)
        {
            if (bCheckDirty && Catlog != null && catlog != null)
                VersionDirty = catlog.buildDate != Catlog.buildDate;
            Catlog = catlog;
            Catlog?.InitializeAssets();
        }

        public static bool ExistAsset(string assetPath)
        {
            var assetInfo = Catlog?.GetAssetInfo(assetPath);
            return assetInfo != null;
        }

        public static AssetInfo GetAsset( string assetPath)
        {
            var assetInfo = Catlog?.GetAssetInfo(assetPath);
            return assetInfo;
        }

        internal static Asset CreateAsset(string path, Type type)
        {
            var info = Catlog?.GetAssetInfo(path);
            return CreateAsset(info, type);
        }

        internal static Asset CreateAsset(AssetInfo info, Type type)
        {
            if (_Impl != null)
                return _Impl.CreateAsset(info, type);
            return null;
        }

        internal static Scene CreateScene(string path, bool additive)
        {
            if (_Impl == null)
                return null;
            var info = Catlog?.GetAssetInfo(path);
            return _Impl.CreateScene(info, additive);
        }

        public static string GetDownloadDataPath(string file)
        {
            if (_Impl == null)
                return file;
            return _Impl.GetDownloadDataPath( file);
        }

        public static string GetPlayerDataURL(string file)
        {
            if (_Impl == null)
                return file;
            return _Impl.GetPlayerDataURL( file);
        }

        public static string GetPlayerDataPath(string file)
        {
            if (_Impl == null)
                return file;
            return _Impl.GetPlayerDataPath(file);
        }

        public static string GetDownloadURL(string file)
        {
            if (_Impl == null)
                return file;
            return _Impl.GetDownloadURL(file);
        }

        public static Stream OpenRead(BundleInfo info)
        {
            if (_Impl == null)
                return null;
            return _Impl.OpenRead(info);
        }

        public static byte[] ReadAllBytes(string file)
        {
            if (_Impl == null)
                return null;
            return _Impl.ReadAllBytes(file);
        }
        
        public static bool BuildinExists( string file)
        {
            if (_Impl == null)
                return false;
            return _Impl.BuildinExists(file);
        }

        public static bool HasUpdateBundle( string name)
        {
            var path = GetDownloadDataPath(name);
            return File.Exists(path);
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        /// <returns></returns>
        public static ClearHistory ClearAsync()
        {
            var clearAsync = new ClearHistory();
            clearAsync.Start();
            return clearAsync;
        }

        /// <summary>
        /// 保存当前版本为启动时加载的版本
        /// </summary>
        public static void SaveCatlog()
        {
            /// 设置client 启动版本
            Catlog.SetUpdateCatlog(BuildDate);
            // 退出安全模式
            ExitSafeModel();
        }

        /// <summary>
        /// 重置Manager
        /// </summary>

        public static void ResetManager()
        {
            _Impl = null;
            /// clear all asset
            // Loadable.ClearAll();
            /// reset downloads
            Download.ClearAllDownloads();
        }

        /// <summary>
        /// 进入安全模式
        /// </summary>
        static void EnterSafeModel( SafeModelHandle handle)
        {
            if (handle == null)
                return;
            _safeModelHandle = handle;
            SafeModel = true;
            Application.logMessageReceived -= OnUncatchExceptions;
            Application.logMessageReceived += OnUncatchExceptions;
        }

        /// <summary>
        /// 当设置版本时 退出安全模式
        /// </summary>
        static void ExitSafeModel()
        {
            _safeModelHandle = null;
            SafeModel = false;
            Application.logMessageReceived -= OnUncatchExceptions;
        }

        /// <summary>
        /// 触发未捕获异常
        /// </summary>
        static void OnUncatchExceptions(string condition, string stackTrace, LogType type)
        {
            // 非安全模式
            if (SafeModel != true)
                return;
            // 触发异常错误
            if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert)
                return;
            /// 重置更新版本
            Catlog.SetUpdateCatlog(0); 
            /// call safe
            _safeModelHandle?.ErrorOrException(condition, stackTrace);
        }
    }

    /// <summary>
    /// 安全模式处理器
    /// </summary>
    public interface SafeModelHandle
    {
        void ErrorOrException(string condition, string stackTrace);
    }


    public interface IAssets
    {
        void Initialize();

        /// <summary>
        /// 获取下载文件路径
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetDownloadDataPath(string file);

        /// <summary>
        /// 包内文件路径 url
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetPlayerDataURL(string file);

        /// <summary>
        /// 包内文件路径
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetPlayerDataPath(string file);

        /// <summary>
        /// 获取下载链接
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetDownloadURL(string file);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Asset CreateAsset(AssetInfo info, Type type);

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="path"></param>
        /// <param name="additive"></param>
        /// <returns></returns>
        Scene CreateScene(AssetInfo info, bool additive);

        /// <summary>
        /// 读取buildin 的文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream OpenRead(BundleInfo info);

        /// <summary>
        /// 读取buildin 的文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        byte[] ReadAllBytes(string file);

        /// <summary>
        /// 内置文件是否存在
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool BuildinExists(string file);
    }

    /// <summary>
    /// Android 平台的资源管理器
    /// </summary>

    class AndroidAssetsImpl : IAssets
    {
        /// <summary>
        /// 平台名字
        /// </summary>
        string platformName;
        /// <summary>
        /// 下载目录
        /// </summary>
        string downloadDir;

        /// <summary>
        /// 初始化操作
        /// </summary>
        public void Initialize()
        {
            platformName = Utility.GetPlatformName();
            // 初始化BetterStreaming 用于读取Android jar 内部 文件
            BetterStreamingAssets.Initialize();
            // 设置下载目录
            downloadDir = Application.persistentDataPath;
            // 创建下载目录
            if (!Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }
        }

        public Asset CreateAsset(string path, Type type)
        {
            if (Manager.Catlog == null)
                return null;
            return CreateAsset(Manager.Catlog.GetAssetInfo(path), type);
        }

        public Asset CreateAsset(AssetInfo info, Type type)
        {
            if (info == null)
                return null;
            return new BundledAsset(info, type);
        }

        public Scene CreateScene(AssetInfo info, bool additive)
        {
            if (info == null)
                return null;
            return BundledScene.Create(info, additive);
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
            return file;// Path.Combine(Application.streamingAssetsPath, file);
        }

        public string GetPlayerDataURL(string file)
        {
            return Path.Combine(Application.streamingAssetsPath, file);
        }

        public Stream OpenRead(BundleInfo info)
        {
            if (info.buildin)
            {
                var tmp = info.hashName.ToLower();
                if (BetterStreamingAssets.FileExists(tmp))
                {
                    return BetterStreamingAssets.OpenRead(tmp);
                }
            }
            else
            {
                var tmp = GetDownloadDataPath(info.hashName);
                if (File.Exists(tmp))
                {
                    return File.Open(tmp, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
            }
            return null;
        }

        public byte[] ReadAllBytes(string file)
        {
            var tmp = file.ToLower();
            if (BetterStreamingAssets.FileExists(tmp))
                return BetterStreamingAssets.ReadAllBytes(tmp);
            return null;
        }

        public bool BuildinExists(string file)
        {
            return BetterStreamingAssets.FileExists(file);
        }
    }

    /// <summary>
    /// 其他平台 iOS、Windows、Mac 等
    /// </summary>

    public class OtherPlatformAssetsImpl : IAssets
    {
        /// <summary>
        /// 平台名字
        /// </summary>
        string platformName;

        /// <summary>
        /// 根路径
        /// </summary>
        string playerDataPath;

        public OtherPlatformAssetsImpl(string dataPath)
        {
            playerDataPath = dataPath;
        }

        public void Initialize()
        {
            platformName = Utility.GetPlatformName();
        }

        public Asset CreateAsset(AssetInfo info, Type type)
        {
            if (info == null)
                return null;
            return new BundledAsset(info, type);
        }

        public Scene CreateScene(AssetInfo assetInfo, bool additive)
        {
            if (assetInfo == null)
                return null;
            return BundledScene.Create(assetInfo, additive);
        }

        public string GetDownloadDataPath(string file)
        {
            return Path.Combine(Application.persistentDataPath, file);
        }

        public string GetDownloadURL(string file)
        {
            //return $"{Manager.DownloadURL}/{platformName}/{file}";
            return $"{Manager.DownloadURL}/{file}";
        }

        public string GetPlayerDataPath(string file)
        {
            return Path.Combine(playerDataPath, file);
        }


        public string GetPlayerDataURL(string file)
        {
            return GetPlayerDataPath(file);// $"file://{GetPlayerDataPath(file)}";
        }

        public string GetTemporaryPath(string file)
        {
            return Application.temporaryCachePath;
        }

        public Stream OpenRead(BundleInfo info)
        {
            string path = info.buildin ? GetPlayerDataPath(info.hashName) : GetDownloadDataPath(info.hashName);
            if (File.Exists(path))
            {
                return File.OpenRead(path);
            }
            return null;
        }

        public byte[] ReadAllBytes(string file)
        {
            var tmp = GetPlayerDataPath(file);
            if (File.Exists(tmp))
                return File.ReadAllBytes(tmp);
            return null;
        }

        public bool BuildinExists(string file)
        {
            var path = GetPlayerDataPath(file);
            return File.Exists(path);
        }
    }
    


}