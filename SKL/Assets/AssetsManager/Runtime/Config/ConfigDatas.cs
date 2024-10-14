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
        /// hash name �����ļ���չ��
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
        /// hash ������֤��Դ�Ƿ�����
        /// </summary>
        public string md5;

        /// <summary>
        /// bundle �����Ǹ�����
        /// </summary>
        public string group;

        /// <summary>
        /// bundle ����
        /// </summary>
        public int bundleID;

        /// <summary>
        /// �Ƿ�Ϊbuildin 
        /// </summary>
        [System.NonSerialized]
        public bool buildin = false;

        /// <summary>
        /// ������AB
        /// </summary>
        public int[] deps;

        /// <summary>
        /// �жϱ����Ƿ��и��ļ� & ���û����Ҫ����
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
        /// ��Դ��(��Դ·��)
        /// </summary>
        public string name;

        /// <summary>
        /// ��Դ���ڵ�bundle
        /// </summary>
        public int bundle;

        /// <summary>
        /// ��Դ������bundle ĿǰΪ���޸�����ж�ص�bug��ʱ���� asset ���ü��ع�ϵ
        /// ���� prefabA prefabB ��ͬһ����AB1 ���档����prefabB ������AB2 
        /// ����� prefabB �����ͷš���ô���ͷŶ�Ӧ��AB2 ��Ȼ���ٴμ���prefabB�������Instantiate �����Ķ������õ�AB2 �е���Դ�ᶪʧ
        /// </summary>
        //public int[] deps;
    }


    /// <summary>
    /// ��ԴĿ¼ 
    /// ��¼���е� AssetBundle �� Asset
    /// </summary>
    [System.Serializable]
    public class Catlog
    {
        /// <summary>
        /// ��Ӧ�İ汾
        /// </summary>
        public string version;
        
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public long buildDate;

        /// <summary>
        /// ��ǰƽ̨
        /// </summary>
        public string platform;

        /// <summary>
        /// ���е�AssetBundle
        /// </summary>
        public BundleInfo[] bundles;

        /// <summary>
        /// ���� Asset ��Դ
        /// </summary>
        public AssetInfo[] assets;

        /// <summary>
        /// ���ڿ��ٲ���
        /// </summary>
        Dictionary<string, AssetInfo> hashAssets;

        /// <summary>
        /// ��ʼ����Դ
        /// </summary>
        public virtual void InitializeAssets()
        {
            if (hashAssets != null)
                return;
            /// ��ʼ������bundle 
            foreach( var bundle in bundles)
            {
                bundle.buildin = Manager.BuildinExists(bundle.hashName);
            }
            ///��ʼ������asset ���չ�ϵ
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
        /// ������Դ
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
        /// ��ȡbundle info
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
        /// �ж��ļ��Ƿ�����ڵ�ǰ����ʹ�õ�bundle
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
        /// ��ǰ���µ���Դ�汾Key
        /// </summary>
        public const string UPDATE_CATLOG = "UPDATE_CATLOG";
        /// <summary>
        /// ͨ���ֽڼ���
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
        /// ͨ���ı�����
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
        /// תΪjson
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
        /// ͨ������ʱ���ȡ�洢�ļ���
        /// </summary>
        /// <param name="buildDate"></param>
        /// <returns></returns>
        static public string GetCatlogFile(long buildDate)
        {
            return $"catlog_{buildDate}.txt";
        }

        /// <summary>
        /// ��ȡ���õ�catlog
        /// </summary>
        /// <param name="buildDate"></param>
        /// <returns></returns>
        static public Catlog GetBuildinCatlog(long buildDate)
        {
            var bytes = Manager.ReadAllBytes(GetCatlogFile(buildDate));
            return LoadCatlogFromBytes(bytes);
        }

        /// <summary>
        /// ��ȡ����Ŀ¼��catlog
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
        /// ���õ�ǰ��Դ�汾
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