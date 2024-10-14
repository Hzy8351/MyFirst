using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

public delegate void onBundleCallback();

public enum DownloadState
{
    Default = 0,
    Downloading = 1,
    Complete = 2,
}

// ��ʼ����Ϸ
public class BundleManager : MonoBehaviour
{
    public static BundleManager Ins = null;
    private onBundleCallback onFailedCallback;

    [NonSerialized]
    public Dictionary<int, WebBundleInfo> dicBundles;

    [NonSerialized]
    public Dictionary<int, WebBundleInfo> playingDownBundles;

    [NonSerialized]
    public int allDownloadCount = 0;

    [NonSerialized]
    public DownloadState downloadState = DownloadState.Default;

    [Header("����������AB����Դ����")]
    public string urlPath = "127.0.0.1";
    public string buildDate = "1709188036";
    [Header("��������ʱAB����Դ����")]
    public string urlPathUnity = "127.0.0.1";
    public string buildDateUnity = "1709188036";
    public bool isLog = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
        dicBundles = new Dictionary<int, WebBundleInfo>();
        playingDownBundles = new Dictionary<int, WebBundleInfo>();
        Assets.Manager.Initialize(new Assets.OtherPlatformAssetsImpl(Application.streamingAssetsPath));
        Ins = this;
    }

    public bool isUseBundle()
    {
        return downloadState != DownloadState.Default;
    }

    public void beginDownload(onBundleCallback cbFailed)
    {
        onFailedCallback = cbFailed;
        StartCoroutine("DownloadAB");
    }

    IEnumerator DownloadAB()
    {
        yield return null;
        LoadVersionWebGL();
    }

    private void Update()
    {
        if (downloadState == DownloadState.Downloading && dicBundles.Count > 0 && allDownloadCount >= dicBundles.Count)
        {
            onCompleteDownload();
            downloadState = DownloadState.Complete;
        }
    }

    public void Log(string str)
    {
        if (isLog)
        {
            Debug.Log(str);
        }
    }

    public WebBundleInfo FindWebBundleInfo(string name)
    {
        foreach (var one in dicBundles)
        {
            WebBundleInfo wbi = one.Value;
            Debug.Log(wbi.bi.name);
            if (wbi.bi.name == name.ToLower())
            {
                return wbi;
            }
        }

        return null;
    }

    public WebBundleInfo FindWebPlayingBundleInfo(string name)
    {
        foreach (var one in playingDownBundles)
        {
            WebBundleInfo wbi = one.Value;
            if (wbi.bi.name == name)
            {
                return wbi;
            }
        }

        return null;
    }

    public bool IsDownComplete()
    {
        return downloadState == DownloadState.Complete;
    }

    public bool IsDownPlayingBundles(string name)
    {
        WebBundleInfo wbi = FindWebPlayingBundleInfo(name);
        if (wbi == null)
        {
            return false;
        }

        for (int i = 0; i < wbi.bi.deps.Length; ++i)
        {
            if (!playingDownBundles.ContainsKey(wbi.bi.deps[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsPlayingBundlesComplete(string name)
    {
        WebBundleInfo wbi = FindWebPlayingBundleInfo(name);
        if (wbi == null || wbi.ab == null)
        {
            return false;
        }

        for (int i = 0; i < wbi.bi.deps.Length; ++i)
        {
            if (!playingDownBundles.ContainsKey(wbi.bi.deps[i]))
            {
                return false;
            }

            WebBundleInfo wbiDep = playingDownBundles[wbi.bi.deps[i]];
            if (wbiDep == null || wbiDep.ab == null)
            {
                return false;
            }

        }

        return true;
    }

    public GameObject LoadPlayingBundleAsset(string name)
    {
        WebBundleInfo wbi = FindWebPlayingBundleInfo(name);
        if (wbi == null || wbi.ab == null)
        {
            return null;
        }

        GameObject obj = wbi.ab.LoadAsset<GameObject>(wbi.bi.name);
        return obj;
    }

    public void DownPlayingBundles(string name)
    {
        WebBundleInfo wbi = FindWebPlayingBundleInfo(name);
        if (wbi == null)
        {
            return;
        }

        if (wbi.ab == null)
        {
            StartCoroutine(DownloadPlayingAB(wbi));
        }

        for (int i = 0; i < wbi.bi.deps.Length; ++i)
        {
            if (playingDownBundles.ContainsKey(wbi.bi.deps[i]))
            {
                WebBundleInfo wbiDep = playingDownBundles[wbi.bi.deps[i]];
                if (wbiDep.ab == null)
                {
                    StartCoroutine(DownloadPlayingAB(wbiDep));
                }
            }
        }

    }

    void LoadVersionWebGL()
    {
        if (downloadState != DownloadState.Default)
        {
            return;
        }

        downloadState = DownloadState.Downloading;
        Log($"Զ�����õ�ַ:{Assets.Setting.Instance().RemoteURL}");
        StartCoroutine(GetRemoteConfigWebGL());
    }

    IEnumerator GetRemoteConfigWebGL()
    {
        Log("GetRemoteConfigWebGL:��ʼ��ȡԶ�������ļ���");
        var setting = Assets.Setting.Instance();

        Log("streamingAssetsPath = " + Application.streamingAssetsPath);
        //string durl = "http://" + ip + ":" + port + "/" + path;
        string ip = "127.0.0.1";

#if !UNITY_EDITOR
        string text = "{\"main\": {\"build\": \"*\",\"version\": \"0.0.1\",\"buildDate\": " + buildDate + ",\"resUrl\": \"" + urlPath + "\",\"authUrl\": \"http://" + ip + "\",\"offline\": false,\"package\": \"http://" + ip + "\",\"serverListUrl\":\"http://" + ip + "\"},\"match\": [{\"build\": \"0.1.*\",\"version\": \"0.0.0\",\"buildDate\": 1675912549,\"resUrl\": \"" + urlPath + "\",\"authUrl\": \"http://" + ip + "\",\"offline\": false,\"package\": \"http://" + ip + "\",\"serverListUrl\":\"http://" + ip + "\" }]}";
#else
        string text = "{\"main\": {\"build\": \"*\",\"version\": \"0.0.1\",\"buildDate\": " + buildDateUnity + ",\"resUrl\": \"" + urlPathUnity + "\",\"authUrl\": \"http://" + ip + "\",\"offline\": false,\"package\": \"http://" + ip + "\",\"serverListUrl\":\"http://" + ip + "\"},\"match\": [{\"build\": \"0.1.*\",\"version\": \"0.0.0\",\"buildDate\": 1675912549,\"resUrl\": \"" + urlPathUnity + "\",\"authUrl\": \"http://" + ip + "\",\"offline\": false,\"package\": \"http://" + ip + "\",\"serverListUrl\":\"http://" + ip + "\" }]}";
#endif

        yield return null;
        try
        {
            var config = JsonUtility.FromJson<Assets.RemoteSetting>(text);
            if (config == null)
                throw new Exception("Decode Failed!");

            Assets.Setting.RemoteSetting = config.MatchVersion(Application.version);
            if (Assets.Setting.RemoteSetting == null)
                throw new Exception("Decode Failed2!");

            Assets.Manager.DownloadURL = Assets.Setting.RemoteSetting.resUrl;
            Assets.Manager.OfflineMode = Assets.Setting.RemoteSetting.offline;

            CheckVersion(Assets.Setting.RemoteSetting);
        }
        catch
        {
            Log("GetRemoteConfig:" + text);
            if (onFailedCallback != null) { onFailedCallback(); }
        }
    }

    void CheckVersion(Assets.RemoteVersion config)
    {
        if (Assets.Manager.BuildDate == Assets.Setting.RemoteSetting.buildDate)
        {
            StartCoroutine(VerifyResources());
        }
        else
        {
            StartCoroutine(LoadCatlog());
        }
    }

    protected IEnumerator LoadCatlog()
    {
        var catlog = new Assets.RemoteCatlog(Assets.Setting.RemoteSetting.buildDate);
        yield return ShowProgress(catlog, "ASSET_LOAD_CATLOG");

        Log("catlog.status = " + catlog.status);
        if (catlog.status == Assets.OperationStatus.Success)
        {
            StartCoroutine(VerifyResources());
        }
        else
        {
            Debug.Log($"Catlog DownLoad Failed");
            if (onFailedCallback != null) { onFailedCallback(); }
        }
    }

    protected IEnumerator VerifyResources()
    {
        //SetLocalizationTip("ASSET_VERIFY_BUNDLES");
        var verify = new Assets.VerifyBundles();
        yield return ShowProgress(verify, "ASSET_VERIFY_BUNDLES");

        if (verify.status == Assets.OperationStatus.Success)
        {
            // У����� ׼������ & ���ؽ�ѹ�� ������Զ������
            // ���ﲻ��ʾ��Ҫ���� & ��Ϊ��ѹ����Ҳ������
            if (verify.FailedBundles.Count > 0)
            {
                TipDownloadSize(verify.FailedBundles);
            }
            //else
            //{
            //    // ��Դ��ʼ���ɹ� & ֱ�ӿ�ʼ������Ϸ
            //    onCompleteDownload();
            //}
        }
        else
        {
            Log("verify.status == Assets.OperationStatus.Success error");
        }
    }

    public float getProgress()
    {
        if (dicBundles.Count <= 0)
        {
            return 0f;
        }
        if (allDownloadCount >= dicBundles.Count)
        {
            return 1f;
        }
        return allDownloadCount / (float)dicBundles.Count;
    }

    IEnumerator DownloadAB(WebBundleInfo wbi)
    {
        ++wbi.downloadCount;
        Log("DownloadAB count = " + wbi.downloadCount + " name = " + wbi.bi.name + " url = " + wbi.url);
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(wbi.url))
        {
            yield return request.SendWebRequest();
            if (request.error == null && request.isDone && request.result == UnityWebRequest.Result.Success)
            {
                if (wbi.ab == null)
                {
                    wbi.ab = DownloadHandlerAssetBundle.GetContent(request);
                }
                //GameObject item = wbi.ab.LoadAsset<GameObject>(wbi.bi.name);
                //GameObject go = GameObject.Instantiate<GameObject>(item);
                //go.transform.position = Vector3.zero;
            }
            request.Dispose();

            if (wbi.ab != null)
            {
                ++allDownloadCount;
                Log("download success count = " + allDownloadCount + " allcount = " + dicBundles.Count);
            }
            else
            {
                Log("downloadAB Failed, name = " + wbi.bi.name + " url = " + wbi.url);
                StartCoroutine(DownloadAB(wbi));
            }
        }
    }

    IEnumerator DownloadPlayingAB(WebBundleInfo wbi)
    {
        ++wbi.downloadCount;
        Log("DownloadPlayingAB count = " + wbi.downloadCount + " name = " + wbi.bi.name + " url = " + wbi.url);
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(wbi.url))
        {
            yield return request.SendWebRequest();
            if (request.error == null && request.isDone && request.result == UnityWebRequest.Result.Success)
            {
                if (wbi.ab == null)
                {
                    wbi.ab = DownloadHandlerAssetBundle.GetContent(request);
                }
            }
            request.Dispose();

            if (wbi.ab == null)
            {
                StartCoroutine(DownloadPlayingAB(wbi));
            }

        }
    }

    void addToDicBundle(Assets.BundleInfo bundle, List<Assets.BundleInfo> bundles, Dictionary<int, WebBundleInfo> dic)
    {
        if (dic.ContainsKey(bundle.bundleID))
        {
            return;
        }

        WebBundleInfo wbi = new WebBundleInfo();
        wbi.bi = bundle;
        wbi.url = Manager.GetDownloadURL(bundle.hashName);
        wbi.save = Manager.GetDownloadDataPath(bundle.hashName);
        dic.Add(wbi.bi.bundleID, wbi);
    }

    void addNoDownload(HashSet<int> sets, int key)
    {
        if (sets.Contains(key))
        {
            return;
        }

        sets.Add(key);
    }

    void removeNoDownload(HashSet<int> sets, int key)
    {
        if (!sets.Contains(key))
        {
            return;
        }

        sets.Remove(key);
    }

    HashSet<int> getNoDownloadBundlesOfName(List<Assets.BundleInfo> bundles, HashSet<int> noDownload, string strNoDown)
    {
        foreach (var bundle in bundles)
        {
            int inx = bundle.name.IndexOf(strNoDown);
            if (inx != 0)
            {
                continue;
            }

            addNoDownload(noDownload, bundle.bundleID);
            for (int k = 0; k < bundle.deps.Length; ++k)
            {
                addNoDownload(noDownload, bundle.deps[k]);
            }
        }

        return noDownload;
    }

    void TipDownloadSize(List<Assets.BundleInfo> bundles)
    {
        dicBundles.Clear();
        /// Զ�����ص�Bundles
        if (bundles != null && bundles.Count > 0)
        {
            HashSet<int> noDownNames = new HashSet<int>();
            foreach (var bundle in bundles)
            {
                if (noDownNames.Contains(bundle.bundleID))
                {
                    addToDicBundle(bundle, bundles, playingDownBundles);
                    continue;
                }
                addToDicBundle(bundle, bundles, dicBundles);
            }

            //long size = 0;
            foreach (var one in dicBundles)
            {
                //size += bundle.size;
                StartCoroutine(DownloadAB(one.Value));
            }

            /// ��ʾ������Դ��С
            //var content = $"{(size / 1048576.0).ToString("0.00") } MB";
            //UI.MessageBox.ShowWith("ALERT", content, "CONFIRM", "CANCEL", (bConfirm) =>
            //{
            //    if (bConfirm)
            //    {
            //       StartCoroutine(DownloadBundles(bundles));
            //    }
            //    else
            //    {
            //        Application.Quit();
            //    }
            //});
            //StartCoroutine(DownloadBundles(bundles));
        }
        else
        {
            Debug.Log($"Catlog Data Error");
            //StartCoroutine(LoadCatlog());
        }
    }

    /// ��ʾÿ�������Ľ���
    protected IEnumerator ShowProgress(Assets.Operation opr, string LocalizationKey)
    {
        opr.Start();
        var visible = opr is Assets.UpdateBundles;
        //var value = Localization.GetLocalizationDefault(LocalizationKey);
        var value = LocalizationKey;
        while (!opr.isDone)
        {
            yield return new WaitForUpdate();
        }
    }

    void onCompleteDownload()
    {
        Log("onCompleteDownload");

        //WebBundleInfo wbiPlayer = FindWebBundleInfo("player");
        //if (wbiPlayer != null)
        //{
        //    PlayerManager.Ins.LoadPrefabsFromAB(wbiPlayer);
        //}

        //WebBundleInfo wbiUI = FindWebBundleInfo("ui");
        //if (wbiUI != null)
        //{
        //    UIManager.GetInstance().AddBundleUI(wbiUI);
        //}

        ///// д�������汾
        Assets.Manager.SaveCatlog();
    }

}

