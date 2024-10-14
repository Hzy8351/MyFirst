using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{

    public class Asset
    {
        public enum AssetType
        {
            Normal, // ������Դ������
        }

        public CollectRule Rule { protected set; get; }

        public string AssetGUID { protected set; get; }

        public string AssetPath { protected set; get; }

        public string[] SubAssets { protected set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        public Asset(string path, CollectRule rule)
        {
            AssetPath = path;
            Rule = rule;
        }

        public void ResetRule(CollectRule rule)
        {
            Rule = rule;
        }

        static List<string> DepCache = new List<string>();
        public void CollectSubAsset()
        {
            DepCache.Clear();
            SubAssets = null;
            if (Rule == null || !NeedCheckDeps(AssetPath))
                return;
            var deps = AssetDatabase.GetDependencies(AssetPath);
            foreach (var dep in deps)
            {
                if (dep == AssetPath)
                    continue;
                if (!CollectRule.IsValidateAsset(dep))
                    continue;
                DepCache.Add(dep);
            }
            SubAssets = DepCache.Count > 0 ? DepCache.ToArray() : null;
        }

        /// <summary>
        /// �ļ��Ƿ������Դ���������� | 
        /// ���ڼ���catlog ��С & ��౻�������ļ��ǲ���Ҫ���ص� ����Animator ������animation
        /// </summary>
        /// <returns></returns>
        public bool Loadable()
        {
            return Rule.EnableLoad;
        }

        static HashSet<string> Extensions = new HashSet<string>()
        {
            ".asset",
            ".unity",
            //".fbx",
            //".mat",
            ".controller",
            ".prefab"
        };

        static bool NeedCheckDeps(string file)
        {
            var extension = Path.GetExtension(file);
            if (Extensions.Contains(extension))
                return true;
            return false;
        }
    }

    public class DepAsset
    {
        public string AssetPath { protected set; get; }

        /// <summary>
        /// ��¼����Դ
        /// </summary>
        public List<Asset> Parents = new List<Asset>();
        public DepAsset(string path)
        {
            AssetPath = path;
        }

        public void AddParent(Asset parent)
        {
            if (Parents.Contains(parent))
                return;
            Parents.Add(parent);
        }
    }

    /// <summary>
    /// ���ݵ�ǰ���ù������ɵ�Result 
    /// ����BuildAssetBundle
    /// </summary>
    public class RuleResults
    {
        public class BundleInfo
        {
            public string Name { protected set; get; }
            public string Package { protected set; get; }
            public List<string> Assets { protected set; get; } = new List<string>();

            /// <summary>
            /// bundle ����
            /// </summary>
            public int BundleID = -1;

            public BundleInfo(string name)
            {
                this.Name = name;
            }

            public void AddAsset(string asset)
            {
                if (Assets.Contains(asset))
                    return;
                Assets.Add(asset);
            }
        }

        /// <summary>
        /// ������Դ�б�
        /// </summary>
        public Dictionary<string, Asset> Assets { protected set; get; }

        /// <summary>
        /// ��������Դ
        /// </summary>
        public Dictionary<string, DepAsset> DepAssets { protected set; get; }

        /// <summary>
        /// ��Դ��Ӧ��bundle
        /// </summary>
        public Dictionary<string, string> Asset2Bundle { protected set; get; }

        /// <summary>
        /// AssetBundles
        /// </summary>
        public Dictionary<string, BundleInfo> Bundles { protected set; get; }

        public RuleResults(Dictionary<string, Asset> assets, Dictionary<string, DepAsset> deps)
        {
            Assets = assets;
            DepAssets = deps;
            /// ���������� ����ab ��ϵ
            Analysis();
        }

        void Analysis()
        {
            Debug.Log($"[Analysis] assets: { Assets.Count} => deps: {DepAssets.Count}");
            Bundles = new Dictionary<string, BundleInfo>();
            Asset2Bundle = new Dictionary<string, string>();
            // ��������asset
            foreach (var asset in Assets)
            {
                var rule = asset.Value.Rule;
                // �������ļ��Ĵ�������޸� & �����ļ���Ҫ�����ɰ�
                if (asset.Key.EndsWith(".unity") && (rule.BundleType != CollectRule.RuleBundleType.File || rule.DepType != CollectRule.DependenciesType.Share))
                {
                    rule = new CollectRule() { name = rule.name, DepType = CollectRule.DependenciesType.Share, BundleType = CollectRule.RuleBundleType.File };
                    asset.Value.ResetRule(rule);
                }
                if (asset.Value.Rule.BundleType == CollectRule.RuleBundleType.File)
                {
                    var bundleName = $"{rule.name}_{ Path.GetFileNameWithoutExtension(asset.Value.AssetPath)}".ToLower();
                    Asset2Bundle.Add(asset.Key, bundleName);
                }
                else
                {
                    Asset2Bundle.Add(asset.Key, rule.name.ToLower());
                }
            }
            // �������б�������asset
            foreach (var dep in DepAssets)
            {
                string bundleName;
                // �˴���Ҫ���� ������� share �����仯
                if (dep.Value.Parents.Count == 1)
                {
                    var together = GetDependienceAsset(dep.Value.Parents, CollectRule.DependenciesType.Together);
                    if (together != null && Asset2Bundle.TryGetValue(together.AssetPath, out bundleName))
                    {
                        Asset2Bundle.Add(dep.Key, bundleName.ToLower());
                        continue;
                    }
                }
                /// �����ļ�����Ŀ¼���кϲ����
                var path_name = Path.GetDirectoryName(dep.Key);
                Asset2Bundle.Add(dep.Key, $"share_{CollectRule.GetDirectoryBundleName(path_name)}".ToLower());
                // �˴���Ҫ���� ������� share �����仯
                //dep.Value.Parents.Sort((a, b) => string.Compare(a.Rule.name, b.Rule.name));
                //var share = GetDependienceAsset(dep.Value.Parents, CollectRule.DependenciesType.Share);
                //if (share != null && Asset2Bundle.TryGetValue(share.AssetPath, out bundleName))
                //{
                //    Asset2Bundle.Add(dep.Key, $"share_{bundleName}".ToLower());
                //    continue;
                //}
                //bundleName = $"dep_{AssetDatabase.AssetPathToGUID(dep.Value.AssetPath)}".ToLower();
                //Asset2Bundle.Add(dep.Key, bundleName);
                Debug.LogWarning($"MakeBundles Asset:{dep.Key} �ж������������ֶ���ӹ���");
            }
            /// ����
            foreach (var asset in Asset2Bundle)
            {
                BundleInfo info;
                if (!Bundles.TryGetValue(asset.Value, out info))
                {
                    info = new BundleInfo(asset.Value);
                    Bundles.Add(asset.Value, info);
                }
                info.AddAsset(asset.Key);
            }
            Debug.Log($"[Analysis] result:{Bundles.Count}");
        }

        Asset GetDependienceAsset(List<Asset> assets, CollectRule.DependenciesType type)
        {
            foreach (var asset in assets)
            {
                if (asset.Rule.DepType == type)
                {
                    return asset;
                }
            }
            return null;
        }

        /// <summary>
        /// ��ȡUnity AssetBundleBuild[] ���ڹ���AssetBundle
        /// </summary>
        /// <returns></returns>
        public AssetBundleBuild[] GetBundleBuild(string extension)
        {
            List<AssetBundleBuild> ret = new List<AssetBundleBuild>(Bundles.Count);
            foreach (var bundle in Bundles)
            {
                var build = new AssetBundleBuild();
                build.assetBundleName = $"{bundle.Value.Name}{extension}";
                build.assetNames = bundle.Value.Assets.ToArray();
                ret.Add(build);
            }
            return ret.ToArray();
        }

        public int GetBundle(string asset)
        {
            if (Asset2Bundle.TryGetValue(asset, out var bundleName))
            {
                if (Bundles.TryGetValue(bundleName, out var info))
                    return info.BundleID;
            }
            return -1;
        }
    }


    /// <summary>
    /// ��Դ�ռ�����
    /// </summary>
    [Serializable]
    public class CollectRule
    {
        /// <summary>
        /// ��Դ�ռ�����
        /// </summary>
        public enum FilterType
        {
            ALL = 0,
            Font, // font asset
            Mesh, // mesh asset
            Model,// model asset
            Prefab,// prefab asset
            Scene, // scene asset
            Shader,// shader asset
            Sprite, // sprite asset
            Texture, // texture asset
            TextAsset,// text asset
            Material, // material asset
            TMP_FontAsset, // tmp asset
            VideoClip, // video asset
            AudioClip, // audio clip asset
            AudioMixer, // audio mixer
            PhysicMaterial,
            RuntimeAnimatorController,
            AnimationClip,
            ScriptableObject,
        }

        readonly public static List<FilterType> FilterPopUpList = new List<FilterType> { FilterType.ALL, FilterType.Font, FilterType.Mesh, FilterType.Model, FilterType.Prefab, FilterType.Scene, FilterType.Shader, FilterType.Sprite, FilterType.Texture, FilterType.VideoClip, FilterType.TextAsset, FilterType.Material, FilterType.TMP_FontAsset, FilterType.AudioClip, FilterType.AudioMixer, FilterType.PhysicMaterial, FilterType.RuntimeAnimatorController, FilterType.AnimationClip, FilterType.ScriptableObject };

        public enum DependenciesType
        {
            None,
            Together,
            Share,
        }

        readonly public static List<DependenciesType> DepPopList = new List<DependenciesType> { DependenciesType.None, DependenciesType.Together, DependenciesType.Share };

        /// <summary>
        /// �������
        /// </summary>
        public enum RuleBundleType
        {
            Together, // ͬһ���������ռ������ļ��ŵ�һ��
            File, /// ͬһ�������µ��ļ� ���ļ�����
        }
        readonly public static List<RuleBundleType> RuleBundlePopList = new List<RuleBundleType> { RuleBundleType.Together, RuleBundleType.File };

        /// <summary>
        /// ������
        /// </summary>
        public string name;

        [SerializeField]
        UnityEngine.Object target;

        [SerializeField]
        string target_uid;

        [SerializeField]
        string target_path;

        /// <summary>
        /// �ռ�Ŀ�� �������ļ��� Ҳ�������ļ�
        /// </summary>
        public UnityEngine.Object Target => target;

        public string TargetPath => target_path;

        /// <summary>
        /// ��Դ�ռ�����
        /// </summary>
        //public FilterType Filter = FilterType.None;

        /// <summary>
        /// ��Դ�ռ�����
        /// </summary>
        public List<FilterType> Filters = new List<FilterType>() { FilterType.ALL };

        /// <summary>
        /// ����������
        /// </summary>
        public RuleBundleType BundleType = RuleBundleType.Together;

        /// <summary>
        /// �����ռ�
        /// </summary>
        public DependenciesType DepType = DependenciesType.Together;

        [Tooltip("��ѡ�� ��д��Catlog ����")]
        public bool EnableLoad = true;

        public string GetSearchFilter()
        {
            if (Filters == null || Filters.Count == 0)
                return string.Empty;
            var ret = string.Empty;
            foreach (var item in Filters)
            {
                if (item == FilterType.ALL)
                    continue;
                ret += $"t:{item} ";
            }
            return ret;
        }

        public void SetTarget(UnityEngine.Object obj)
        {
            target = obj;
            target_path = obj != null ? AssetDatabase.GetAssetPath(obj) : null;
            target_uid = obj != null ? AssetDatabase.AssetPathToGUID(target_path) : null;
        }

        /// <summary>
        /// �ӵ����л�ԭ
        /// </summary>
        public void LoadTarget()
        {
            // reload target
            if (!string.IsNullOrEmpty(target_uid))
            {
                target_path = AssetDatabase.GUIDToAssetPath(target_uid);
            }
            /// ��ԭ target
            target = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(target_path);
        }

        public int FilterValue()
        {
            int ret = 0;
            foreach (var item in Filters)
                ret += (int)item;
            return ret;
        }

        /// <summary>
        /// �ų��ļ����� Ŀǰ���ռ�shader ��shader �Զ�����
        /// </summary>
        static List<string> ExcludeFiles = new List<string>() { ".cs", ".spriteatlas", ".giparams", "LightingData.asset", ".dll", ".js", ".boo", ".meta", ".cginc", ".shader", ".lighting" };
        static public bool IsValidateAsset(string assetPath)
        {
            //if (AssetDatabase.IsValidFolder(assetPath))
            //    return false;

            if (assetPath.StartsWith("Assets/") == false && assetPath.StartsWith("Packages/") == false)
                return false;

            if (assetPath.Contains("/Resources/") || assetPath.Contains("Editor Resources/") || assetPath.Contains("Gizmos/"))
                return false;

            /// �ж��Ƿ����ļ�
            var extension = Path.GetFileName(assetPath);
            var index = ExcludeFiles.FindIndex(a => extension.EndsWith(a, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
                return false;

            return true;
        }
        public static string NoramlPath(string input)

        {
            return input.Replace("\\", "/");
        }

        public static string GetDirectoryBundleName(string input)
        {
            input = NoramlPath(input);
            var splits = input.Split('/');
            var ret = splits.Length > 0 ? splits[splits.Length - 1] : "";
            for (int i = splits.Length - 2; i > 0; i--)
            {
                ret = splits[i] + "_" + ret;
                if (splits.Length - i >= 3)
                    break;
            }
            return ret;
        }

        public List<Asset> GetAllFiles()
        {
            var ret = new List<Asset>(10);
            if (File.Exists(target_path))
            {
                var asset = new Asset(target_path, this);
                ret.Add(asset);
                return ret;
            }
            var files = AssetDatabase.FindAssets(GetSearchFilter(), new string[1] { target_path });
            foreach (var guid in files)
            {
                var path = NoramlPath(AssetDatabase.GUIDToAssetPath(guid));
                if (!IsValidateAsset(path))
                    continue;
                var asset = new Asset(path, this);
                ret.Add(asset);
            }
            return ret;
        }

    }

    /// <summary>
    /// ��Դ����
    /// </summary>
    [Serializable]
    public class AssetGroup
    {
        /// <summary>
        /// ��������
        /// </summary>
        public string GroupName;

        /// <summary>
        /// �Ƿ񼤻�
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// ��������
        /// </summary>
        public string GroupDescription;

        /// <summary>
        /// �ռ�����
        /// </summary>
        public List<CollectRule> Rules = new List<CollectRule>();
    }

    /// <summary>
    /// ��Դ����
    /// </summary>
    public class AssetGroups : ScriptableObject
    {
        const string SavePath = "Assets/AssetsManager/AssetGroups.asset";

        public List<AssetGroup> groups = new List<AssetGroup>();

        public static AssetGroups GetInstance()
        {
            AssetGroups ret = File.Exists(SavePath) ? AssetDatabase.LoadAssetAtPath<AssetGroups>(SavePath) : null;
            if (ret == null)
            {
                ret = ScriptableObject.CreateInstance<AssetGroups>();
                AssetDatabase.CreateAsset(ret, SavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return ret;
        }

        /// <summary>
        /// ����
        /// </summary>
        public static void SaveAsset(AssetGroups asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// ���õ���
        /// </summary>
        public void OnImport()
        {
            foreach (var group in groups)
            {
                if (group.Rules == null)
                    continue;
                foreach (var rule in group.Rules)
                {
                    rule.LoadTarget();
                }
            }
        }

        public static RuleResults CollectAssets( bool bDebug = false)
        {
            var groupSetting = GetInstance();
            /// �ռ����й��� Ȼ�����ļ���������� ��֤�ڲ���ļ����ޱ������ռ���
            List<CollectRule> rules = new List<CollectRule>();
            foreach (var group in groupSetting.groups)
            {
                if (group.Active == false)
                    continue;
                foreach (var rule in group.Rules)
                {
                    rules.Add(rule);
                }
            }
            rules.Sort((a, b) => {
                if( a.TargetPath == b.TargetPath)
                {
                    return b.FilterValue() - a.FilterValue();
                }
                return string.Compare(b.TargetPath, a.TargetPath);
            });
            if(bDebug)
            {
                var builder = new StringBuilder();
                foreach (var item in rules)
                    builder.AppendLine($"{item.TargetPath}:{item.EnableLoad}");
                Debug.Log(builder);
            }
            // �ռ������ļ�
            Dictionary<string, Asset> assets = new Dictionary<string, Asset>(1000);
            /// �ռ�����
            Dictionary<string, DepAsset> depAssets = new Dictionary<string, DepAsset>(1000);
            try
            {
                for (int i = 0; i < rules.Count; i++)
                {
                    var rule = rules[i];
                    EditorUtility.DisplayProgressBar($"�������({i}/{rules.Count}) ", rule.TargetPath, (float)i / (float)rules.Count);
                    var tmp = rule.GetAllFiles();
                    foreach (var asset in tmp)
                    {
                        if (!assets.TryGetValue(asset.AssetPath, out var oldAsset))
                        {
                            assets[asset.AssetPath] = asset;
                            continue;
                        }
                        // TODO: ��Ҫ�����ļ��ľ�׼����������Ӧ��������һ������ �������������ǰ��rule 

                        //if (oldAsset.Rule.Filter > asset.Rule.Filter)
                        //{
                        //    assets[asset.AssetPath] = asset;
                        //}
                    }
                }
                int index = 0;
                foreach (var asset in assets)
                {
                    EditorUtility.DisplayProgressBar($"�ռ�����({index}/{assets.Count}) ", asset.Value.AssetPath, (float)index / (float)assets.Count);
                    index++;
                    // collect 
                    asset.Value.CollectSubAsset();
                    if (asset.Value.SubAssets != null)
                    {
                        foreach (var item in asset.Value.SubAssets)
                        {
                            var path = CollectRule.NoramlPath(item);
                            if (!CollectRule.IsValidateAsset(path) || assets.ContainsKey(path))
                                continue;
                            DepAsset dep;
                            if (!depAssets.TryGetValue(path, out dep))
                            {
                                dep = new DepAsset(path);
                                depAssets.Add(path, dep);
                            }
                            dep.AddParent(asset.Value);
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
                // ��֯bundle
                return new RuleResults(assets, depAssets);
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError(e);
                return null;
            }
        }

    }
}