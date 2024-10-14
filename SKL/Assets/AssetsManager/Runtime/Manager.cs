using System;
using System.IO;
using UnityEngine;

namespace Assets
{
    public static class Manager
    {
        /// <summary>
        /// ��������
        /// </summary>
        public static string DownloadURL = null;

        /// <summary>
        /// ��ȫģʽ ������ʱ������������ 
        /// �ͻ�����Ҫ�ص�����ʼ��������
        /// ������µ������߼������޷�����
        /// </summary>
        public static bool SafeModel { private set; get; }


        /// <summary>
        /// ��ȫģʽ ������ʱ������������ 
        /// �ͻ�����Ҫ�ص�����ʼ��������
        /// ������µ������߼������޷�����
        /// </summary>
        public static bool VersionDirty { private set; get; } = false;

        /// <summary>
        /// ����ʱ����
        /// </summary>
        static AssetsSetting.Config setting;

        /// <summary>
        /// ��ǰ����ģʽ
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
        /// �Ƿ�Ϊ����ģʽ
        /// </summary>
        public static bool OfflineMode = true;

        /// <summary>
        /// ��ǰ��Դ�汾
        /// </summary>
        public static string Version => Catlog != null ? Catlog.version : Application.version;

        /// <summary>
        /// ��ǰʹ����Դ��������
        /// </summary>
        public static long BuildDate => Catlog != null ? Catlog.buildDate : 0;
        /// <summary>
        /// ��ȡ��������
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
        /// ��ǰcatlog
        /// </summary>
        internal static Catlog Catlog;

        /// <summary>
        /// ��Դ����ʵ��
        /// </summary>
        static IAssets _Impl;

        /// <summary>
        /// ��Դ����ʵ��
        /// </summary>
        static SafeModelHandle _safeModelHandle;

        /// <summary>
        /// ��ʼ����Դ������ 
        /// </summary>
        /// <param name="impl"> ��Դ����</param>
        /// <param name="config"></param>
        /// <param name="safeModelHandle"></param>
        public static void Initialize( IAssets impl = null, AssetsSetting.Config config = null, SafeModelHandle safeModelHandle = null)
        {
            /// ��ǰ汾�Ƿ����
            VersionDirty = false;
            // ��������ʵ��
            if ( _Impl == null)
            {
                /// ���밲ȫģʽ
                EnterSafeModel(safeModelHandle);
                /// check impl
                _Impl = impl == null? GetAssetsImpl(): impl;
                /// ���ó�ʼ��
                _Impl.Initialize();
                ///��ȡ����ʱ����
                setting = GetConfig(config);
                if (setting == null)
                {
                    //throw new Exception("Assets Intialize Failed: setting is nil!");
                    return;
                }

                /// �Ƿ�Ϊ����ģʽ
                OfflineMode = setting.Offline;
            }
            /// �༭��ģʽ
            if (setting.PlayModel == AssetsModel.Editor)
                return;

            /// ��ʼ����ԴĿ¼
            var buildin = Catlog.GetBuildinCatlog(setting.BuildDate);
            if (buildin == null)
            {
                //Debug.Log("Assets Initialize Failed: buildin catlog is nil!");
                //throw new Exception("Assets Initialize Failed: buildin catlog is nil!");
                return;
            }
               
            /// ��ʼ�� ��������
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
        /// ����Catlog �汾
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
        /// ������Դ
        /// </summary>
        /// <returns></returns>
        public static ClearHistory ClearAsync()
        {
            var clearAsync = new ClearHistory();
            clearAsync.Start();
            return clearAsync;
        }

        /// <summary>
        /// ���浱ǰ�汾Ϊ����ʱ���صİ汾
        /// </summary>
        public static void SaveCatlog()
        {
            /// ����client �����汾
            Catlog.SetUpdateCatlog(BuildDate);
            // �˳���ȫģʽ
            ExitSafeModel();
        }

        /// <summary>
        /// ����Manager
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
        /// ���밲ȫģʽ
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
        /// �����ð汾ʱ �˳���ȫģʽ
        /// </summary>
        static void ExitSafeModel()
        {
            _safeModelHandle = null;
            SafeModel = false;
            Application.logMessageReceived -= OnUncatchExceptions;
        }

        /// <summary>
        /// ����δ�����쳣
        /// </summary>
        static void OnUncatchExceptions(string condition, string stackTrace, LogType type)
        {
            // �ǰ�ȫģʽ
            if (SafeModel != true)
                return;
            // �����쳣����
            if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert)
                return;
            /// ���ø��°汾
            Catlog.SetUpdateCatlog(0); 
            /// call safe
            _safeModelHandle?.ErrorOrException(condition, stackTrace);
        }
    }

    /// <summary>
    /// ��ȫģʽ������
    /// </summary>
    public interface SafeModelHandle
    {
        void ErrorOrException(string condition, string stackTrace);
    }


    public interface IAssets
    {
        void Initialize();

        /// <summary>
        /// ��ȡ�����ļ�·��
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetDownloadDataPath(string file);

        /// <summary>
        /// �����ļ�·�� url
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetPlayerDataURL(string file);

        /// <summary>
        /// �����ļ�·��
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetPlayerDataPath(string file);

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetDownloadURL(string file);

        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Asset CreateAsset(AssetInfo info, Type type);

        /// <summary>
        /// ���س���
        /// </summary>
        /// <param name="path"></param>
        /// <param name="additive"></param>
        /// <returns></returns>
        Scene CreateScene(AssetInfo info, bool additive);

        /// <summary>
        /// ��ȡbuildin ���ļ�
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream OpenRead(BundleInfo info);

        /// <summary>
        /// ��ȡbuildin ���ļ�
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        byte[] ReadAllBytes(string file);

        /// <summary>
        /// �����ļ��Ƿ����
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool BuildinExists(string file);
    }

    /// <summary>
    /// Android ƽ̨����Դ������
    /// </summary>

    class AndroidAssetsImpl : IAssets
    {
        /// <summary>
        /// ƽ̨����
        /// </summary>
        string platformName;
        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        string downloadDir;

        /// <summary>
        /// ��ʼ������
        /// </summary>
        public void Initialize()
        {
            platformName = Utility.GetPlatformName();
            // ��ʼ��BetterStreaming ���ڶ�ȡAndroid jar �ڲ� �ļ�
            BetterStreamingAssets.Initialize();
            // ��������Ŀ¼
            downloadDir = Application.persistentDataPath;
            // ��������Ŀ¼
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
    /// ����ƽ̨ iOS��Windows��Mac ��
    /// </summary>

    public class OtherPlatformAssetsImpl : IAssets
    {
        /// <summary>
        /// ƽ̨����
        /// </summary>
        string platformName;

        /// <summary>
        /// ��·��
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