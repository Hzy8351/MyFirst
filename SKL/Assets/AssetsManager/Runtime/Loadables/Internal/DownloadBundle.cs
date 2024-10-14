using System;
using UnityEngine;

namespace Assets
{

    internal class DownloadBundle : Bundle, IDisposable
    {
        /// <summary>
        /// 文件下载器
        /// </summary>
        private Download download;

        /// <summary>
        /// 资源流
        /// </summary>
        private System.IO.Stream abstream;

        /// <summary>
        /// ab 加载请求
        /// </summary>
        private AssetBundleCreateRequest request;

        /// <summary>
        /// 释放标志
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="info"></param>
        public DownloadBundle(BundleInfo info) : base(info) 
        { 
        }

        ~DownloadBundle()
        {
            Dispose(false);
        }

        /// <summary>
        /// 这种下载的bundle 不允许立即加载
        /// </summary>
        public override void LoadImmediate()
        {
            if (IsDone()) 
                return;

            if (!download.isDone)
            {
                request = null;
                return;
            }

            //  判断request 是否为null
            if (request != null)
            {
                OnLoaded(request.assetBundle);
                request = null;
                return;
            }
        }

        protected override void OnLoad()
        {
            download = Download.DownloadAsync(bundleInfo.hashName, Manager.GetDownloadDataPath(bundleInfo.hashName), null, bundleInfo.size, bundleInfo.md5);
            download.completed += OnDownloaded;
        }

        private void OnDownloaded(Download obj)
        {
            if (download.status == DownloadStatus.Failed)
            {
                Finish(download.error);
                return;
            }

            if (UnityBundle != null)
                return;
            try
            {
#if ENCRYPE_RES
                abstream = ABStream.Create(bundleInfo);
                request = AssetBundle.LoadFromStreamAsync(abstream);
#else
            request = AssetBundle.LoadFromFileAsync(obj.info.savePath);
#endif
            }
            catch( System.Exception e)
            {
                Finish($"DownloadBundle load bundle: {bundleInfo.name} failed! => {e.ToString()}");
            }
        }

        protected override void OnUpdate()
        {
            if (status != LoadableStatus.Loading) return;

            if (!download.isDone)
            {
                progress = download.downloadedBytes * 1f / download.info.size * 0.5f;
                return;
            }

            if (request == null) return;

            progress = 0.5f + request.progress;
            if (!request.isDone) return;

            OnLoaded(request.assetBundle);
            request = null;
        }
        protected override void OnUnload()
        {
            base.OnUnload();
            /// call dispose
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            // 通知垃圾回收器不再调用终结器
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
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