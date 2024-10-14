using System.Collections.Generic;
using UnityEngine;

namespace Assets
{

    public class Dependencies : Loadable
    {
        /// <summary>
        /// 目前使用bundle dep
        /// </summary>
        static readonly Dictionary<string, Dependencies> Cache = new Dictionary<string, Dependencies>();

        /// <summary>
        /// 对应的bundle 信息
        /// </summary>
        BundleInfo bundleInfo;

        /// <summary>
        /// 依赖项目
        /// </summary>
        List<Bundle> depBundles;

        /// <summary>
        /// 创建加载器
        /// </summary>
        /// <param name="assetInfo"></param>
        /// <returns></returns>
        public static Dependencies Load(AssetInfo assetInfo)
        {
            if (Manager.Catlog == null)
                throw new System.NullReferenceException("Assets.Manager Catlog is nil!");

            var bundleInfo = Manager.Catlog.GetBundleInfo(assetInfo.bundle);
            if(bundleInfo == null)
                throw new System.Exception($"Dependencies Load: {assetInfo.name} dep: {assetInfo.bundle} info is nil!");

            /// check cache
            if (!Cache.TryGetValue(bundleInfo.hashName, out var item))
            {
                item = new Dependencies(bundleInfo);
                Cache.Add(bundleInfo.hashName, item);
            }
            item.Load();
            return item;
        }

        public Dependencies( BundleInfo info)
        {
            bundleInfo = info;
            pathOrURL = info.hashName;
            depBundles = new List<Bundle>(1);
        }

        protected override void OnLoad()
        {
            // insert into main bundle
            var main = Bundle.LoadInternal(bundleInfo);
            depBundles.Add(main);
            // load dependencies
            if (bundleInfo.deps != null)
            {
                for( int i=0; i < bundleInfo.deps.Length; i++)
                {
                    depBundles.Add(Bundle.LoadInternal(bundleInfo.deps[i]));
                }
            }
        }

        public override void LoadImmediate()
        {
            if (IsDone())
                return;

            foreach (var request in depBundles)
            {
                request.LoadImmediate();
            }
        }

        protected override void OnUnload()
        {
            /// unload bundle
            foreach (var item in depBundles)
            {
                if (item.status == LoadableStatus.SuccessToLoad)
                    item.Release();
            }
            depBundles.Clear();
            /// unload dep cache
            Cache.Remove(bundleInfo.hashName);
        }

        protected override void OnUpdate()
        {
            if (status == LoadableStatus.Loading)
            {
                var totalProgress = 0f;
                var allDone = true;
                foreach (var child in depBundles)
                {
                    totalProgress += child.progress;
                    if (!string.IsNullOrEmpty(child.error))
                    {
                        status = LoadableStatus.FailedToLoad;
                        error = child.error;
                        progress = 1;
                        return;
                    }

                    if (child.IsDone()) 
                        continue;

                    allDone = false;
                    break;
                }

                progress = totalProgress / depBundles.Count * 0.5f;
                if (allDone)
                {
                    Finish();
                }
            }
        }

        public AssetBundle GetAssetBundle()
        {
            if (depBundles.Count >= 1)
                return depBundles[0].UnityBundle;
            return null;
        }
    }
}