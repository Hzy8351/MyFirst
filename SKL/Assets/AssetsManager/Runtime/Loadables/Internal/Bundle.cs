using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Bundle : Loadable
    {
        /// <summary>
        /// 当前所有 Bundle cache
        /// </summary>
        static readonly Dictionary<string, Bundle> Cache = new Dictionary<string, Bundle>();

        /// <summary>
        /// 当前所有 用于避免同名的bundle 被重复加载
        /// </summary>
        static readonly Dictionary<string, string> Bundle2Hash = new Dictionary<string, string>();

        /// <summary>
        /// 当前AssetBundle
        /// </summary>
        public AssetBundle UnityBundle { get; protected set; }

        /// <summary>
        /// bundle 信息
        /// </summary>
        protected BundleInfo bundleInfo;

        /// <summary>
        /// 释放旧的AssetBundle 引用
        /// </summary>
        /// <param name="info"></param>
        static void ReleaseOldBundle(BundleInfo info)
        {
            /// 没有bundle name 对应的引用
            if (!Bundle2Hash.TryGetValue(info.name, out string hash))
                return;
            /// 判断是否有引用
            Logger.I($"[Bundle] ReleaseOldBundle: Try Get OldAssetBundle => {info.name}:{info.hashName}!");
            if (!Cache.TryGetValue(hash, out var bundle))
                return;
            // 触发强制卸载
            Logger.W($"[Bundle] ReleaseOldBundle: Force Unload Asset Bundle => ({bundle.bundleInfo.name}:{bundle.bundleInfo.hashName}) new hash:{info.hashName}");
            if (bundle.UnityBundle != null)
                bundle.UnityBundle.Unload(false);
            bundle.UnityBundle = null;
        }

        /// <summary>
        /// 缓存当前加载的Bundle
        /// </summary>
        /// <param name="info"></param>
        /// <param name="bundle"></param>
        static void RecordBundle(BundleInfo info, Bundle bundle)
        {
            Cache[info.hashName] = bundle;
            Bundle2Hash[info.name] = info.hashName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info"></param>
        public Bundle( BundleInfo info)
        {
            bundleInfo = info;
            pathOrURL = info.name;
        }

        protected void OnLoaded(AssetBundle bundle)
        {
            UnityBundle = bundle;
            //Debug.Log($"[Bundle]~ OnLoaded: {bundleInfo.name}:{bundleInfo.hashName}=>{bundle}");
            Finish(UnityBundle == null? $"[Bundle] OnLoaded: { this.bundleInfo.name} => {this.bundleInfo.hashName} null!": null);
        }

        protected override void OnUnload()
        {
            //Debug.LogWarning($"[Bundle]~ OnUnload: {bundleInfo.name}:{bundleInfo.hashName}=>{UnityBundle}!");
            if (UnityBundle != null)
                UnityBundle.Unload(true);
            else
                Debug.LogWarning($"[Bundle]~ {bundleInfo.name}:{bundleInfo.hashName} is nil!");
            UnityBundle = null;
            // unload
            Cache.Remove(bundleInfo.hashName);
        }

        /// <summary>
        /// 加载Bundle
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        internal static Bundle LoadInternal(int bundle)
        {
            if (Manager.Catlog == null)
                throw new NullReferenceException();

            var bundleInfo = Manager.Catlog.GetBundleInfo(bundle);
            if (bundleInfo == null)
                throw new Exception($"[Bundle] LoadInternal: {bundle} bundle info is nil!");
            
            return LoadInternal(bundleInfo);
        }

        internal static Bundle LoadInternal(BundleInfo bundleInfo)
        {
            /// 保证同名的新的assetbundle会被加载 同时卸载原来的assetbundle
            if (!Cache.TryGetValue(bundleInfo.hashName, out var item))
            {
                /// 判断线程异常
                AssetBundle[] abs = Resources.FindObjectsOfTypeAll<AssetBundle>();
                foreach (var _item in abs)
                {
                    if (_item.name.StartsWith(bundleInfo.name))
                    {
                        Debug.LogError("heihei");
                    }
                }


                // 卸载掉以前的bundle
                ReleaseOldBundle(bundleInfo);
                /// 加载bundle
                if (bundleInfo.NeedDownload())
                    item = new DownloadBundle(bundleInfo);
                else
                    item = new LocalBundle(bundleInfo);
                RecordBundle(bundleInfo, item);
            }
            item.Load();
            return item;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器停止运行时释放资源 文件引用
        /// </summary>
        public static void ApplicationStop()
        {
            foreach( var bundle in Cache)
            {
                if (bundle.Value is IDisposable disposable)
                    disposable.Dispose();
            }
        }
#endif
    }
}