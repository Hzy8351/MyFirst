
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

namespace Assets
{
    /// 初始化资源目录 分两种情况 
    /// 一种加载本地StreamAsset 内的目录
    /// 一种记载远程服务器的目录

    public class RemoteCatlog : Operation
    {
        /// <summary>
        /// catlog file
        /// </summary>
        public string catlogFile;

        /// <summary>
        /// web 请求
        /// </summary>
        UnityWebRequest request;
        
        /// <summary>
        /// 下载的catlog
        /// </summary>
        public Catlog catlog { protected set; get; }

        public RemoteCatlog(long buildDate)
        {
            catlogFile = Catlog.GetCatlogFile(buildDate);
        }

        public override void Start()
        {
            base.Start();
            /// get url
            var url = Manager.GetDownloadURL(catlogFile);
            Debug.Log($"Catlog {url}");
            request = UnityWebRequest.Get(url);
            request.SendWebRequest();
        }

        protected override void Update()
        {
            if (status != OperationStatus.Processing)
                return;

            // check is done
            progress = request != null ? request.downloadProgress : 1.0f;
            if (request == null || !request.isDone)
                return;

            // successed
            if (request.result == UnityWebRequest.Result.Success)
            {
                catlog = Catlog.LoadCatlogFromBytes(request.downloadHandler.data);
                // save catlog
                if (catlog != null)
                {
                    /// 设置catlog
                    Manager.SetCatlog(catlog, true);
                    /// save catlog file
                    var localPath = Manager.GetDownloadDataPath(catlogFile);
                    //try { File.WriteAllBytes(localPath, request.downloadHandler.data); }
                    //catch { Debug.LogError($"[RemoteCatlog]  WriteFailed:{localPath}"); }
                    // complete get catlog
                    Finish();
                }
                else
                {
                    //File.WriteAllText("C:/CatlogFromFailed.txt", request.url);
                    Error($"[RemoteCatlog] Decode Catlog From =>{request.url} Failed!");
                }
            }
            else
            {
                //File.WriteAllText("C:/LoadFromFailed.txt", request.url);
                Error($"[RemoteCatlog] Load From =>{request.url} {request.result} Failed");
            }
            request.Dispose();
        }
    }


    public class VerifyBundles : Operation
    {
        Catlog catlog = null;

        /// <summary>
        /// 待验证的资源
        /// </summary>
        Queue<BundleInfo> verifyBundles = new Queue<BundleInfo>();

        /// <summary>
        /// 校验失败的资源
        /// </summary>
        public List<BundleInfo> FailedBundles { protected set; get; } = new List<BundleInfo>();

        public VerifyBundles()
        {
            catlog = Manager.Catlog;
            var count = (catlog != null && catlog.bundles != null) ? catlog.bundles.Length : -1;
            //Debug.Log($"[VerifyBundles] VerifyResources Count=>{count}!");
        }

        public override void Start()
        {
            base.Start();
            // wait for checks
            if( catlog.bundles != null)
            {
                foreach (var bundle in catlog.bundles)
                {
                    verifyBundles.Enqueue(bundle);
                }
            }
            else
            {
                Error("VerifyAssets Bundles is Nil!");
            }
        }

        protected override void Update()
        {
            while(!Updater.Busy())
            {
                BundleInfo item = verifyBundles.Count > 0? verifyBundles.Dequeue(): null;
                if(  item == null)
                {
                    Finish();
                    return;
                }
                /// 判断PlayerData 文件是否存在
                if( VerifyBundle( item))
                {
                    continue;
                }
                /// 添加到失败的bundle
                if (!FailedBundles.Contains(item))
                {
                    FailedBundles.Add(item);
                }
                // remove file
                var filePath = !item.buildin? Manager.GetDownloadDataPath(item.hashName): null;
                if (filePath != null && File.Exists(filePath))
                {
                    try { File.Delete(filePath); }
                    catch { Debug.LogError($"Verify Bundle Failed & Delete Failed { filePath}"); }
                }
            }
        }

        // 只校验文件长度
        protected bool VerifyBundle( BundleInfo bundle)
        {
            using (var stream = Manager.OpenRead(bundle))
            {
                if (stream != null && bundle.size == stream.Length)
                    return true;
            }
            return false;
        }
    }

    // 更新资源
    public class UpdateBundles: Operation
    {
        //private float tick;
        /// <summary>
        /// 需要更新的bundles
        /// </summary>
        List<BundleInfo> _bundles;

        /// <summary>
        /// 当前下载项目
        /// </summary>
        List<Download> downloaders;

        /// <summary>
        /// 所有失败的bundles
        /// </summary>
        public List<BundleInfo> Faileds { protected set; get; }

        /// <summary>
        /// 创建更新操作
        /// </summary>
        /// <param name="bundles"></param>
        public UpdateBundles( List<BundleInfo> bundles)
        {
            _bundles = bundles;
            downloaders = new List<Download>(bundles.Count);
            Faileds = new List<BundleInfo>(bundles.Count);
        }

        public override void Start()
        {
            base.Start();
            // clear all
            Faileds.Clear();
            if ( _bundles != null )
            {
                foreach (var item in _bundles)
                {
                    var url = Manager.GetDownloadURL(item.hashName);
                    var save = Manager.GetDownloadDataPath(item.hashName);
                    var downloader = Download.DownloadAsync(url, save, null, size: item.size, hash: item.md5);
                    Debug.Log("bundle item url = " + url);
                    downloaders.Add(downloader);
                }
            }
            else
            {
                Error("UpdateBundles Bundle is Nil!");
            }
        }

        //void logTest(bool b, string str)
        //{
        //    if (b)
        //    {
        //        Debug.Log(str);
        //    }
        //}

        protected override void Update()
        {
            if (status != OperationStatus.Processing)
                return;

            //bool bLog = false;
            //tick += Time.deltaTime;
            //if (tick >= 1.0f)
            //{
            //    bLog = true;
            //    tick = 0.0f;
            //}

            progress = (float)Download.TotalDownloadedBytes / (float)Download.TotalSize;
            description = $"{Utility.FormatBytes(Download.TotalBandwidth)}/S ({ Utility.FormatBytes(Download.TotalDownloadedBytes)}/{Utility.FormatBytes(Download.TotalSize)})";
            bool allDone = true;
            string error = null;
            for (int i = 0; i < downloaders.Count; i++)
            {
                var item = downloaders[i];
                allDone = !item.isDone ? false : allDone;

                //logTest(bLog, "name = " + _bundles[i].name + " isDone = " + item.isDone + " status = " +  item.status);

                if (!item.isDone)
                    continue;

                if (item.status == DownloadStatus.Success)
                    continue;

                Faileds.Add(_bundles[i]);
                error = error != null ? error : item.error;
            }

            // check all done
            if (!allDone)
                return;

            ///判断更新成功或失败
            if( error != null)
            {
                Error(error);
            }
            else
            {
                Finish();
            }
        }
    }
}