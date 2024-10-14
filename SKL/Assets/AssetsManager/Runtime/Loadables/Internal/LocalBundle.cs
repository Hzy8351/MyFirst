using System;
using UnityEngine;

namespace Assets
{

    internal class LocalBundle : Bundle, IDisposable
    {
        /// <summary>
        /// 资源流
        /// </summary>
        private System.IO.Stream abstream = null;
        
        /// <summary>
        /// ab 加载
        /// </summary>
        private AssetBundleCreateRequest request = null;

        /// <summary>
        /// 释放标志
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="info"></param>
        public LocalBundle(BundleInfo info) : base(info)
        {
        }

        ~LocalBundle()
        {
            Dispose(false);
        }

        protected override void OnLoad()
        {
            try
            {
#if ENCRYPE_RES
                abstream = ABStream.Create(bundleInfo);
                request = AssetBundle.LoadFromStreamAsync(abstream);
#else
                var url = bundleInfo.buildin ? Manager.GetPlayerDataURL(bundleInfo.hashName) : Manager.GetDownloadDataPath(bundleInfo.hashName);
                request = AssetBundle.LoadFromFileAsync(url);
#endif
            }
            catch( Exception e)
            {
                Finish($"BundleRequest load bundle: {bundleInfo.name} failed! => {e.ToString()}");
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            // 释放托管资源
            Dispose();
        }

        public override void LoadImmediate()
        {
            if (IsDone()) 
                return;
            OnLoaded(request.assetBundle);
            request = null;
        }

        protected override void OnUpdate()
        {
            if (status != LoadableStatus.Loading)
            {
                return;
            }
            progress = request.progress;
            /// call onloaded
            if (request.isDone)
            {
                OnLoaded(request.assetBundle);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // 通知垃圾回收器不再调用终结器
            GC.SuppressFinalize(this);
        }

        virtual protected void Dispose(bool disposing)
        {
            if (disposed)
                return;
            /// 释放资源
            abstream?.Dispose();
            abstream = null;
            /// 标记已经释放过
            disposed = true;
        }
    }
}