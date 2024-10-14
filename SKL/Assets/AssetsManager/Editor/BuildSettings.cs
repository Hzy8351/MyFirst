using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{

    [Serializable]
    class DebugBundles
    {
        [Serializable]
        class SerializeBundle
        {
            public string name;
            public string hashName;
            public string[] assets;

            public SerializeBundle(AssetBundleBuild build)
            {
                name = build.assetBundleName;
                assets = build.assetNames;
            }
        }

        [SerializeField]
        List<SerializeBundle> bundles = new List<SerializeBundle>();

        static public DebugBundles Convert(AssetBundleBuild[] builds)
        {
            var ret = new DebugBundles();
            foreach (var build in builds)
            {
                ret.bundles.Add(new SerializeBundle(build));
            }
            return ret;
        }
    }

    [Serializable]
    public class BuildSetting
    {
        /// <summary>
        /// assetbundle ��׺
        /// </summary>
        public const string Extension = ".bundle";

        /// <summary>
        /// assetbundle Ŀ¼
        /// </summary>
        public string BuildPath = "./Bundles";

        /// <summary>
        /// buildsetting
        /// </summary>
        public string Name;

        /// <summary>
        /// �Ƿ񼤻�
        /// </summary>
        public bool Active = false;

        /// <summary>
        /// �汾��
        /// </summary>
        public string Version = "0.0.1";

        /// <summary>
        /// Ĭ������
        /// </summary>
        public SystemLanguage Language;

        [Tooltip("��ʼ��URL")]
        public string remoteUri;

        [Tooltip("������")]
        public string channel;

        [Tooltip("�Ƿ�֧��Console")]
        public bool enableDebugLogger;

        [Tooltip("�Ƿ�����ʹ��")]
        public bool offlineMode = false;

        [Tooltip("������ɺ����������")]
        public string startScene;

        //[Tooltip("�Ƿ�ʹ�ñ��ػ�������")]
        //public bool useLocalLanguage;

        //[Tooltip("������Դ�����ļ���")]
        //public List<LocalizationItem> ResourceSetting = new List<LocalizationItem>();
        /// <summary>
        /// ��ǰѡ��Ĵ��build �汾
        /// </summary>
        public long CurBuildData = 0;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public List<Catlog> Builds = new List<Catlog>();

        /// <summary>
        /// У������language setting
        /// </summary>
        public void CheckLanguageSettings()
        {
            //foreach (var language in BuildSettings.EnableLanguages)
            //{
            //    var resource = ResourceSetting.Find((tmp) => language == tmp.language);
            //    if (resource == null)
            //    {
            //        resource = new LocalizationItem();
            //        resource.language = language;
            //        resource.ResourceDir = "";
            //        ResourceSetting.Add(resource);
            //    }
            //}
        }

        /// <summary>
        /// ��ȡ����Ĺ���Ŀ¼
        /// </summary>
        /// <returns></returns>
        public string GetBuildPath(bool bCreate = false)
        {
            var platPath = Path.Combine(BuildPath, $"Temp/{ BuildSettings.GetPlatformName()}");
            if (bCreate && !Directory.Exists(platPath))
                Directory.CreateDirectory(platPath);
            return platPath;
        }

        /// <summary>
        /// ��ȡ���ܺ���ļ�λ��
        /// </summary>
        /// <returns></returns>
        public string GetPrePublish(bool bCreate = false)
        {
            var platPath = Path.Combine(BuildPath, $"PrePublish/{ BuildSettings.GetPlatformName()}");
            if (bCreate && !Directory.Exists(platPath))
                Directory.CreateDirectory(platPath);
            return platPath;
        }

        /// <summary>
        /// ��ȡ��󷢲�bundle Ŀ¼
        /// </summary>
        /// <returns></returns>
        public string GetPublish(bool bCreate = false)
        {
            var platPath = Path.Combine(BuildPath, $"Publish/{ BuildSettings.GetPlatformName()}");
            if (bCreate && !Directory.Exists(platPath))
                Directory.CreateDirectory(platPath);
            return platPath;
        }
    }

    public class BuildSettings : ScriptableObject
    {
        const string SavePath = "Assets/AssetsManager/BuildingSettings.asset";

        /// <summary>
        /// ��Դ����ģʽ
        /// </summary>
        public readonly static List<SystemLanguage> EnableLanguages = new List<SystemLanguage>() { SystemLanguage.Chinese, SystemLanguage.ChineseTraditional, SystemLanguage.English, SystemLanguage.Arabic };

        /// <summary>
        /// ��Դ����ģʽ
        /// </summary>
        public readonly static List<AssetsModel> PlayModelsList = new List<AssetsModel>() { AssetsModel.Editor, AssetsModel.EditorAB, AssetsModel.Runtime};

        /// <summary>
        /// ����ģʽ
        /// </summary>
        public AssetsModel PlayModel = AssetsModel.Editor;

        /// <summary>
        /// ���������б�
        /// </summary>
        public List<BuildSetting> Settings = new List<BuildSetting>();

        public void SetActive(BuildSetting setting)
        {
            var curActive = Settings.Find((t) => t.Active);
            if (curActive != null)
                curActive.Active = false;
            setting.Active = true;
        }

        public static BuildSettings GetInstance()
        {
            BuildSettings ret = File.Exists(SavePath) ? AssetDatabase.LoadAssetAtPath<BuildSettings>(SavePath) : null;
            if (ret == null)
            {
                ret = ScriptableObject.CreateInstance<BuildSettings>();
                AssetDatabase.CreateAsset(ret, SavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return ret;
        }

        /// <summary>
        /// ����
        /// </summary>
        public static void SaveAsset(BuildSettings asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            AssetDatabase.Refresh();
        }

        public BuildSetting GetActiveSetting()
        {
            return Settings.Find((t) => t.Active);
        }

        public int GetActiveIndex()
        {
            if (Settings.Count == 0)
                return -1;
            return Settings.FindIndex((t) => t.Active);
        }


        public static string GetPlatformName()
        {
            return "WebGL";
            //if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) return "Android";
            //if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX) return "OSX";
            //if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
            //    EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
            //    return "Windows";
            //if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) return "iOS";
            //return EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL
            //    ? "WebGL"
            //    : "UnSupport";
        }

        static Dictionary<BuildSetting, List<Catlog>> Catlogs = new Dictionary<BuildSetting, List<Catlog>>();

        /// <summary>
        /// ��ȡ��ǰsetting ������catlog
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static List<Catlog> GetBuilds(BuildSetting setting)
        {
            List<Catlog> ret;
            if (!Catlogs.TryGetValue(setting, out ret))
            {
                ret = new List<Catlog>();
                Catlogs[setting] = ret;
                // ȷ�ϴ��ƽ̨�ļ���
                var fromDirectory = setting.GetPrePublish();
                if (Directory.Exists(fromDirectory))
                {
                    var catlogs = Directory.GetFiles(fromDirectory, "catlog*.txt");
                    foreach (var path in catlogs)
                    {
                        var bytes = File.ReadAllBytes(path);
                        var catlog = Catlog.LoadCatlogFromBytes(bytes);
                        if (catlog != null)
                        {
                            ret.Add(catlog);
                        }
                    }
                    ret.Sort((a, b) => (int)(b.buildDate - a.buildDate));
                }
            }
            return ret;
        }

        /// <summary>
        /// ���õ�ǰ�汾�Ĺ����б�
        /// </summary>
        /// <param name="setting"></param>
        public static void ResetBuilds(BuildSetting setting)
        {
            Catlogs?.Remove(setting);
        }
    }

}