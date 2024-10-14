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
            Normal, // 正常资源的引用
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
        /// 文件是否可用资源管理器加载 | 
        /// 用于减少catlog 大小 & 许多被依赖的文件是不需要加载的 例如Animator 关联的animation
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
        /// 记录依赖源
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
    /// 根据当前可用规则生成的Result 
    /// 用于BuildAssetBundle
    /// </summary>
    public class RuleResults
    {
        public class BundleInfo
        {
            public string Name { protected set; get; }
            public string Package { protected set; get; }
            public List<string> Assets { protected set; get; } = new List<string>();

            /// <summary>
            /// bundle 索引
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
        /// 所有资源列表
        /// </summary>
        public Dictionary<string, Asset> Assets { protected set; get; }

        /// <summary>
        /// 依赖的资源
        /// </summary>
        public Dictionary<string, DepAsset> DepAssets { protected set; get; }

        /// <summary>
        /// 资源对应的bundle
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
            /// 分析依赖项 生成ab 关系
            Analysis();
        }

        void Analysis()
        {
            Debug.Log($"[Analysis] assets: { Assets.Count} => deps: {DepAssets.Count}");
            Bundles = new Dictionary<string, BundleInfo>();
            Asset2Bundle = new Dictionary<string, string>();
            // 遍历所有asset
            foreach (var asset in Assets)
            {
                var rule = asset.Value.Rule;
                // 将场景文件的错误规则修复 & 场景文件需要单独成包
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
            // 遍历所有被依赖的asset
            foreach (var dep in DepAssets)
            {
                string bundleName;
                // 此处需要排序 避免造成 share 发生变化
                if (dep.Value.Parents.Count == 1)
                {
                    var together = GetDependienceAsset(dep.Value.Parents, CollectRule.DependenciesType.Together);
                    if (together != null && Asset2Bundle.TryGetValue(together.AssetPath, out bundleName))
                    {
                        Asset2Bundle.Add(dep.Key, bundleName.ToLower());
                        continue;
                    }
                }
                /// 根据文件所在目录进行合并打包
                var path_name = Path.GetDirectoryName(dep.Key);
                Asset2Bundle.Add(dep.Key, $"share_{CollectRule.GetDirectoryBundleName(path_name)}".ToLower());
                // 此处需要排序 避免造成 share 发生变化
                //dep.Value.Parents.Sort((a, b) => string.Compare(a.Rule.name, b.Rule.name));
                //var share = GetDependienceAsset(dep.Value.Parents, CollectRule.DependenciesType.Share);
                //if (share != null && Asset2Bundle.TryGetValue(share.AssetPath, out bundleName))
                //{
                //    Asset2Bundle.Add(dep.Key, $"share_{bundleName}".ToLower());
                //    continue;
                //}
                //bundleName = $"dep_{AssetDatabase.AssetPathToGUID(dep.Value.AssetPath)}".ToLower();
                //Asset2Bundle.Add(dep.Key, bundleName);
                Debug.LogWarning($"MakeBundles Asset:{dep.Key} 有多个依赖项，建议手动添加规则！");
            }
            /// 归类
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
        /// 获取Unity AssetBundleBuild[] 用于构建AssetBundle
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
    /// 资源收集规则
    /// </summary>
    [Serializable]
    public class CollectRule
    {
        /// <summary>
        /// 资源收集规则
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
        /// 分组规则
        /// </summary>
        public enum RuleBundleType
        {
            Together, // 同一个规则下收集到的文件放到一起
            File, /// 同一个规则下的文件 按文件排列
        }
        readonly public static List<RuleBundleType> RuleBundlePopList = new List<RuleBundleType> { RuleBundleType.Together, RuleBundleType.File };

        /// <summary>
        /// 分组名
        /// </summary>
        public string name;

        [SerializeField]
        UnityEngine.Object target;

        [SerializeField]
        string target_uid;

        [SerializeField]
        string target_path;

        /// <summary>
        /// 收集目标 可以是文件夹 也可以是文件
        /// </summary>
        public UnityEngine.Object Target => target;

        public string TargetPath => target_path;

        /// <summary>
        /// 资源收集类型
        /// </summary>
        //public FilterType Filter = FilterType.None;

        /// <summary>
        /// 资源收集类型
        /// </summary>
        public List<FilterType> Filters = new List<FilterType>() { FilterType.ALL };

        /// <summary>
        /// 打包分组规则
        /// </summary>
        public RuleBundleType BundleType = RuleBundleType.Together;

        /// <summary>
        /// 依赖收集
        /// </summary>
        public DependenciesType DepType = DependenciesType.Together;

        [Tooltip("勾选后 会写入Catlog 里面")]
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
        /// 从导入中还原
        /// </summary>
        public void LoadTarget()
        {
            // reload target
            if (!string.IsNullOrEmpty(target_uid))
            {
                target_path = AssetDatabase.GUIDToAssetPath(target_uid);
            }
            /// 还原 target
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
        /// 排除文件类型 目前不收集shader 让shader 自动处理
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

            /// 判断是否有文件
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
    /// 资源分组
    /// </summary>
    [Serializable]
    public class AssetGroup
    {
        /// <summary>
        /// 分组名字
        /// </summary>
        public string GroupName;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// 分组描述
        /// </summary>
        public string GroupDescription;

        /// <summary>
        /// 收集规则
        /// </summary>
        public List<CollectRule> Rules = new List<CollectRule>();
    }

    /// <summary>
    /// 资源分组
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
        /// 保存
        /// </summary>
        public static void SaveAsset(AssetGroups asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 调用导入
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
            /// 收集所有规则 然后按照文件夹深度排序 保证内层的文件有限被规则收集到
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
            // 收集所有文件
            Dictionary<string, Asset> assets = new Dictionary<string, Asset>(1000);
            /// 收集依赖
            Dictionary<string, DepAsset> depAssets = new Dictionary<string, DepAsset>(1000);
            try
            {
                for (int i = 0; i < rules.Count; i++)
                {
                    var rule = rules[i];
                    EditorUtility.DisplayProgressBar($"处理规则({i}/{rules.Count}) ", rule.TargetPath, (float)i / (float)rules.Count);
                    var tmp = rule.GetAllFiles();
                    foreach (var asset in tmp)
                    {
                        if (!assets.TryGetValue(asset.AssetPath, out var oldAsset))
                        {
                            assets[asset.AssetPath] = asset;
                            continue;
                        }
                        // TODO: 需要根据文件的精准类型来区分应该属于拿一个规则 否则属于排序更前的rule 

                        //if (oldAsset.Rule.Filter > asset.Rule.Filter)
                        //{
                        //    assets[asset.AssetPath] = asset;
                        //}
                    }
                }
                int index = 0;
                foreach (var asset in assets)
                {
                    EditorUtility.DisplayProgressBar($"收集依赖({index}/{assets.Count}) ", asset.Value.AssetPath, (float)index / (float)assets.Count);
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
                // 组织bundle
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