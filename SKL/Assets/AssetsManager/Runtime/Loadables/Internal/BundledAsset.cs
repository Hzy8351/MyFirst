using System;
using UnityEngine;

namespace Assets
{

    public class BundledAsset : Asset
    {
        /// <summary>
        /// 资源信息
        /// </summary>
        AssetInfo assetInfo;

        /// <summary>
        /// 依赖处理
        /// </summary>
        Dependencies dependencies;

        /// <summary>
        /// 资源加载器
        /// </summary>
        AssetBundleRequest request;

        public BundledAsset(AssetInfo info, Type t)
        {
            type = t;
            assetInfo = info;
            pathOrURL = info.name;
        }

        protected override void OnLoad()
        {
            dependencies = Dependencies.Load(assetInfo);
            status = LoadableStatus.DependentLoading;
        }

        protected override void OnUnload()
        {
            //Logger.I("Unload BundleAsset:{0} {1}.", assetInfo.name, error);
            dependencies?.Release();
            dependencies = null;
            asset = null;
            request = null;
            // call base
            base.OnUnload();
        }

        public override void LoadImmediate()
        {
            if (IsDone()) 
                return;

            if (dependencies == null)
            {
                Finish($"[BundleAsset] LoadImmediate: {this.assetInfo.name} dependencies is null");
                return;
            }

            if (!dependencies.IsDone())
            {
                dependencies.LoadImmediate();
            }

            var assetbundle = dependencies.GetAssetBundle();
            if (assetbundle == null)
            {
                Finish($"[BundleAsset] LoadImmediate: {this.assetInfo.name} assetBundle is null");
                return;
            }

            OnLoaded(assetbundle.LoadAsset(pathOrURL, type));
        }

        protected override void OnUpdate()
        {
            if (status == LoadableStatus.Loading)
                UpdateLoading();
            else if (status == LoadableStatus.DependentLoading)
                UpdateDependencies();
        }

        void UpdateLoading()
        {
            if (request == null)
            {
                Finish($"[BundleAsset] UpdateLoading: {this.assetInfo.name} request is null");
                return;
            }

            progress = 0.5f + request.progress * 0.5f;
            if (!request.isDone)
                return;

            OnLoaded(request.asset);
        }

        void UpdateDependencies()
        {
            if (dependencies == null)
            {
                Finish($"[BundleAsset] UpdateDependencies: {this.assetInfo.name} dependencies is null");
                return;
            }

            progress = 0.5f * dependencies.progress;
            if (!dependencies.IsDone())
            {
                return;
            }

            var assetBundle = dependencies.GetAssetBundle();
            if (assetBundle == null)
            {
                Finish($"[BundleAsset] UpdateDependencies: {this.assetInfo.name} assetBundle is null");
                return;
            }

            request = assetBundle.LoadAssetAsync(pathOrURL, type);
            status = LoadableStatus.Loading;
        }

        public override bool IsVaild()
        {
            if (dependencies == null || !dependencies.IsDone())
                return false;

            if (dependencies.status != LoadableStatus.SuccessToLoad)
                return false;

            return true;
        }

        /// <summary>
        /// load sub asset
        /// </summary>
        /// <param name="path"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public override UnityEngine.Object LoadSubAsset(string path, Type t)
        {
            if (!IsDone())
                return null;

            var assetBundle = dependencies.GetAssetBundle();
            if (assetBundle == null)
                return null;
            return assetBundle.LoadAsset(path, t);
        }
    }
}