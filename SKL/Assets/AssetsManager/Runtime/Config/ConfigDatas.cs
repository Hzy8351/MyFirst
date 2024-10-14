using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets
{
    [System.Serializable]
    public class WebBundleInfo
    {
        public int downloadCount = 0;
        public string url = "";
        public string save = "";
        public AssetBundle ab;
        public BundleInfo bi;

    }

    [System.Serializable]
    public class BundleInfo
    {
        
        /// <summary>
        /// hash name 包含文件扩展名
        /// </summary>
        public string hashName;

        /// <summary>
        /// bundle name
        /// </summary>
        public string name;

        /// <summary>
        /// bundle size
        /// </summary>
        public long size;

        /// <summary>
        /// hash 用于验证资源是否完整
        /// </summary>
        public string md5;

        /// <summary>
        /// bundle 属于那个分组
        /// </summary>
        public string group;

        /// <summary>
        /// bundle 索引
        /// </summary>
        public int bundleID;

        /// <summary>
        /// 是否为buildin 
        /// </summary>
        [System.NonSerialized]
        public bool buildin = false;

        /// <summary>
        /// 依赖的AB
        /// </summary>
        public int[] deps;

        /// <summary>
        /// 判断本地是否有该文件 & 如果没有需要下载
        /// </summary>
        /// <returns></returns>
        /// 
        public bool NeedDownload()
        {
            if (buildin)
                return false;
            var file = Manager.GetDownloadDataPath(hashName);
            return !System.IO.File.Exists(file);
        }
    }

    [System.Serializable]
    public class AssetInfo
    {
        /// <summary>
        /// 资源名(资源路径)
        /// </summary>
        public string name;

        /// <summary>
        /// 资源所在的bundle
        /// </summary>
        public int bundle;

        /// <summary>
        /// 资源依赖的bundle 目前为了修复依赖卸载的bug暂时不做 asset 引用加载关系
        /// 例如 prefabA prefabB 在同一个包AB1 里面。其中prefabB 引用了AB2 
        /// 如果对 prefabB 进行释放、那么会释放对应的AB2 ，然后再次加载prefabB，会造成Instantiate 出来的对向引用的AB2 中的资源会丢失
        /// </summary>
        //public int[] deps;
    }


    /// <summary>
    /// 资源目录 
    /// 记录所有的 AssetBundle 和 Asset
    /// </summary>
    [System.Serializable]
    public class Catlog
    {
        /// <summary>
        /// 对应的版本
        /// </summary>
        public string version;
        
        /// <summary>
        /// 构建时间
        /// </summary>
        public long buildDate;

        /// <summary>
        /// 当前平台
        /// </summary>
        public string platform;

        /// <summary>
        /// 所有的AssetBundle
        /// </summary>
        public BundleInfo[] bundles;

        /// <summary>
        /// 所有 Asset 资源
        /// </summary>
        public AssetInfo[] assets;

        /// <summary>
        /// 用于快速查找
        /// </summary>
        Dictionary<string, AssetInfo> hashAssets;

        /// <summary>
        /// 初始化资源
        /// </summary>
        public virtual void InitializeAssets()
        {
            if (hashAssets != null)
                return;
            /// 初始化所有bundle 
            foreach( var bundle in bundles)
            {
                bundle.buildin = Manager.BuildinExists(bundle.hashName);
            }
            ///初始化所有asset 对照关系
            hashAssets = new Dictionary<string, AssetInfo>();
            foreach( var item in assets)
            {
                if( !hashAssets.ContainsKey( item.name))
                {
                    hashAssets.Add(item.name, item);
                }
            }
        }

        /// <summary>
        /// 查找资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual AssetInfo GetAssetInfo( string path)
        {
            if (hashAssets == null)
                InitializeAssets();
            hashAssets.TryGetValue(path, out var ret);
            return ret;
        }

        /// <summary>
        /// 获取bundle info
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public virtual BundleInfo GetBundleInfo( int bundle)
        {
            if (bundle < 0 || bundles == null || bundle >= bundles.Length)
                return null;
            return bundles[bundle];
        }

        /// <summary>
        /// 判断文件是否存在于当前正在使用的bundle
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual bool IncludeBundle( string file)
        {
            var fname = System.IO.Path.GetFileName(file);
            foreach( var bundle in bundles)
            {
                if (bundle.hashName == fname)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 当前更新的资源版本Key
        /// </summary>
        public const string UPDATE_CATLOG = "UPDATE_CATLOG";
        /// <summary>
        /// 通过字节加载
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static public Catlog LoadCatlogFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
                return null;

            Catlog catlog = null;
            try
            {
                if (bytes[0] > 0)
                {
                    bytes = GZip.Decompress(bytes, 1);
                    catlog = JsonUtility.FromJson<Catlog>(UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                }
                else
                {
                    catlog = JsonUtility.FromJson<Catlog>(UTF8Encoding.UTF8.GetString(bytes, 1, bytes.Length - 1));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Load Catlog from byte failed!" + e.ToString());
            }
            return catlog;
        }

        /// <summary>
        /// 通过文本加载
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        static public Catlog LoadFromText(string txt)
        {
            if (txt.Length > 0 && txt[0] == '{')
                return JsonUtility.FromJson<Catlog>(txt);
            return LoadCatlogFromBytes(UTF8Encoding.UTF8.GetBytes(txt));
        }

        /// <summary>
        /// 转为json
        /// </summary>
        /// <param name="bCompress"></param>
        /// <returns></returns>
        public byte[] ToJson(bool bCompress)
        {
            string txt = JsonUtility.ToJson(this);
            var bytes = UTF8Encoding.UTF8.GetBytes(txt);
            if (bCompress)
                bytes = GZip.Compress(bytes);
            var ret = new byte[bytes.Length + 1];
            ret[0] = bCompress ? (byte)1 : (byte)0;
            System.Array.Copy(bytes, 0, ret, 1, bytes.Length);
            return ret;
        }

        /// <summary>
        /// 通过构建时间获取存储文件名
        /// </summary>
        /// <param name="buildDate"></param>
        /// <returns></returns>
        static public string GetCatlogFile(long buildDate)
        {
            return $"catlog_{buildDate}.txt";
        }

        /// <summary>
        /// 获取内置的catlog
        /// </summary>
        /// <param name="buildDate"></param>
        /// <returns></returns>
        static public Catlog GetBuildinCatlog(long buildDate)
        {
            var bytes = Manager.ReadAllBytes(GetCatlogFile(buildDate));
            return LoadCatlogFromBytes(bytes);
        }

        /// <summary>
        /// 获取更新目录的catlog
        /// </summary>
        /// <returns></returns>
        static public Catlog GetUpdateCatlog()
        {
            var date = PlayerPrefs.GetString(UPDATE_CATLOG, "");
            if (string.IsNullOrEmpty(date))
                return null;
            if (!long.TryParse(date, out var buildDate))
                return null;
            var file = Manager.GetDownloadDataPath(GetCatlogFile(buildDate));
            if (!System.IO.File.Exists(file))
                return null;
            var bytes = System.IO.File.ReadAllBytes(file);
            return LoadCatlogFromBytes(bytes);
        }

        /// <summary>
        /// 设置当前资源版本
        /// </summary>
        /// <param name="buildDate"></param>
        static public void SetUpdateCatlog(long buildDate = 0)
        {
            if (buildDate <= 0)
            {
                PlayerPrefs.DeleteKey(UPDATE_CATLOG);
            }
            else
            {
                PlayerPrefs.SetString(UPDATE_CATLOG, buildDate.ToString());
            }
        }

    }
}