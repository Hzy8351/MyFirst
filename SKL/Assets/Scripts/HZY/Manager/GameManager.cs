using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;
using System.Net;
using System.Globalization;

public class GameManager : MonoSingleton<GameManager>
{
    #region 构造函数及其变量
    public GameManager()
    {
    }

    public static float offHide = -90000f;
    public bool useGuide = true;
    public bool useGuideData = false;
    public bool useGM = false;

    private UserDatas playerData = null;
    public UserDatas PlayerData { get { return playerData; } }
    private List<ClockBase> clocks = new List<ClockBase>();

    private TableManager configManager = new TableManager();
    public TableManager CM { get { return configManager; } }

    //private CfgData cfgData = new CfgData();
    //public CfgData CFGD { get { return cfgData; } }

    //private GameData gameData = new GameData();
    //public GameData GD { get { return gameData; } }

    #endregion

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this.gameObject);
        Application.targetFrameRate = 60;

    }

    private void Update()
    {
        updateTimeClocks();
    }

    #region clocks

    public long getTickCount()
    {
        return (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000L;
    }

    public void addClock(ClockBase btc, float sec)
    {
        btc.InitTick(sec);
        clocks.Add(btc);
    }

    private void updateTimeClocks()
    {
        if (playerData == null || clocks == null)
        {
            return;
        }

        for (int i = 0; i < clocks.Count;)
        {
            ClockBase btc = clocks[i];
            if (btc.updateTick())
            {
                clocks.Remove(btc);
                continue;
            }
            ++i;
        }
    }

    #endregion

    #region others

    public int ramdomQuanZhong(List<int[]> lists)
    {
        int count = 0;
        for (int i = 0; i < lists.Count; ++i)
        {
            count += lists[i][1];
        }
        int r = UnityEngine.Random.Range(0, count);
        for (int i = 0; i < lists.Count; ++i)
        {
            int[] arr = lists[i];
            if (r < arr[1])
            {
                return arr[0];
            }
            r -= arr[1];
        }

        return 0;
    }

    public int ramdomQuanZhong(List<int[]> lists, ref int outIndex)
    {
        int count = 0;
        for (int i = 0; i < lists.Count; ++i)
        {
            count += lists[i][1];
        }
        int r = UnityEngine.Random.Range(0, count);
        for (int i = 0; i < lists.Count; ++i)
        {
            int[] arr = lists[i];
            if (r < arr[1])
            {
                outIndex = i;
                return arr[0];
            }
            r -= arr[1];
        }

        return 0;
    }


    private int getLv(string key, Dictionary<string, int> dic)
    {
        return dic.ContainsKey(key) ? dic[key] : 0;
    }

    private bool setLv(string key, int lv, Dictionary<string, int> dic)
    {
        if (!dic.ContainsKey(key))
        {
            return false;
        }
        dic[key] = lv;
        return true;
    }

    #endregion

    #region vals
    private bool addLv(string key, int lv, Dictionary<string, int> dic)
    {
        if (!dic.ContainsKey(key))
        {
            return false;
        }
        dic[key] += lv;
        return true;
    }

    public void addValNobuf(int i, long v, string reason)
    {
        addValNobuf(i, v);
        reportVal(i, (int)v, 1, reason);
    }

    public void addValNobuf(int i, long v)
    {
        playerData.mainData.vals[i] += v;
        //if (i == 10)
        //{
        //    getTaskManager().onTask(16, 0, 0);
        //}
    }

    public long addVal(int i, long count, string reason)
    {
        //long v = GameFormula.func2(count + GD.valBuff[i], GD.rateBuff[i] / 100f);
        long v = GameFormula.func2(count + 0, 0);
        addValNobuf(i, v, reason);
        return v;
    }

    public void addVal(string reason, string a0, string a1, string a2 = "0", bool bTask = false)
    {
        SplitData sd = CM.ParseString(a0, a1);
        int id = int.Parse(a0);
        if (id < getValArrLen())
        {
            long val = addVal(sd.id, sd.val, reason);
            //if (bTask)
            //{
            //    getTaskManager().onTask(3, sd.id, val);
            //}
            return;
        }

        //Debug.Log("add buff: a0 = " + a0 + ", a1 = " + a1);
    }

    public bool dedVal(int i, long count, string reason)
    {
        if (playerData.mainData.vals[i] >= count)
        {
            playerData.mainData.vals[i] -= count;
            reportVal(i, (int)count, 0, reason);
            return true;
        }
        return false;
    }

    public void reportVal(int id, int val, int ar, string reason)
    {
        ohayoo_game_moneyflow gr = new ohayoo_game_moneyflow();
        gr.moneytype = id;
        gr.moneyid = id;
        gr.moneyname = CM.dataCommon.getItem(id).Name;
        gr.mchange = val;
        gr.addorreduce = ar;
        gr.moneyleft = (int)getVal(id);
        gr.reason = reason;
        gr.subreason = GameReportManager.getTimeYYMMSS();
        OhayooSDKManager.instance.GameReport("ohayoo_game_moneyflow", JsonUtility.ToJson(gr));
    }

    public int getValArrLen()
    {
        return playerData.mainData.vals.Length;
    }

    public long getVal(int i)
    {
        return playerData.mainData.vals[i];
    }

    public string getValStr(string val)
    {
        return getValStr(long.Parse(val));
    }

    public string getValStr(long val)
    {
        if (val < 10000)
        {
            return val.ToString();
        }

        if (val < 100000000)
        {
            return convertValStr(string.Format("{0:F}", val / 10000f)) + "万";
        }

        return convertValStr(string.Format("{0:F}", val / 100000000f)) + "亿";
    }

    private string convertValStr(string str)
    {
        if (str[str.Length - 1] == '0')
        {
            string str1 = str.Substring(0, str.Length - 1); // 去掉0
            if (str1[str1.Length - 1] == '0')
            {
                return str1.Substring(0, str1.Length - 2); // 去掉.0
            }

            return str1;
        }

        return str;
    }
    #endregion

    #region OnApplicationPause(bool pause)切屏感知
    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
            return;
        }
        SoundManager.instance.playCurMusic();
    }
    #endregion

    #region OnApplicationQuit() 退出游戏感知
    public void OnApplicationQuit()
    {
        SaveGame();
    }
    #endregion

    #region 初始化本地数据
    public bool InitLocalData()
    {
        CM.Init();

        bool bNew = false;
        playerData = UserDatas.GetLocalData(ref bNew);//读取本地持久化玩家数据(包括本土化设置)

        if (!useGuide && bNew)
        {
            if (useGuideData)
            {

            }
        }


        return bNew;
    }

    #endregion

    #region CreateGiveTips(string iconName,string num,Vector3 initPos)
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iconName">图标名称</param>
    /// <param name="num">数量</param>
    /// <param name="initPos">初始位置</param>
    public void CreateGiveTips(string iconName, string textName, string num, Vector3 initPos, float dis = 100f, float sec = 1f)
    {
        string path = "Prefabs/UI/GiveTips";
        GameObject obj = Instantiate(ResourcesLoad.Instance.Load<GameObject>(path), CanvasManager.instance.tranFront);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.position = initPos;
        obj.GetComponent<GiveTipsCtrl>().Iint(iconName, textName, num, dis, sec);
    }

    public void CreateGiveTipsInUITip(string iconName, string textName, string num, Vector3 initPos, Vector3 sc, float dis = 100f, float sec = 1f)
    {
        string path = "Prefabs/UI/GiveTips";
        GameObject obj = Instantiate(ResourcesLoad.Instance.Load<GameObject>(path), CanvasManager.instance.tranTips);
        obj.transform.localScale = sc;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.position = initPos;
        obj.GetComponent<GiveTipsCtrl>().Iint(iconName, textName, num, dis, sec);
    }
    #endregion

    #region CreateTextTips

    public void CreateTextTips(string des, int size = 45, float dic = 120f, float sec = 0.8f, string color16 = "24d9f1")
    {
        ViewTextTips(string.Format("<size={0}><color=#{1}>{2}</color></size>", size, color16, des), dic, sec);
    }

    private void ViewTextTips(string des, float dic = 120f, float sec = 0.5f)
    {
        string path = "Prefabs/UI/TextTips";
        GameObject obj = Instantiate(ResourcesLoad.Instance.Load<GameObject>(path), CanvasManager.instance.tranTips);
        obj.GetComponent<TextTipsCtrl>().Init(des, dic, sec);
    }

    #endregion

    #region audio
    public void setMusicState()
    {
        playerData.mainData.musicState = !playerData.mainData.musicState;
    }
    public bool getMusicState()
    {
        return playerData.mainData.musicState;
    }

    public void setSoundState()
    {
        playerData.mainData.soundState = !playerData.mainData.soundState;
    }
    public bool getSoundState()
    {
        return playerData.mainData.soundState;
    }
    #endregion

    #region SaveGame() 保存玩家数据
    public void SaveGame()
    {
        playerData.SaveGame();
    }
    #endregion

    #region OnDestroy()
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    #endregion

    #region ObjPrefab
    /// <summary>
    /// 添加预制体
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fatherTransform"></param>
    /// <returns></returns>
    public GameObject AddPrefab(string name, Transform fatherTransform)
    {
        string newpath = "Prefabs/" + name;
        GameObject obj = NABObjPool.Instance.GetObj(newpath, fatherTransform);

        return obj;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(string name, GameObject gameObject)
    {
        string[] list = name.Split(new char[] { '(' });
        if (list.Length != 2)
        {
            string newpath = "Prefabs/" + name;
            NABObjPool.Instance.Recycle(newpath, gameObject);
        }
        else
        {
            string newpath = "Prefabs/" + list[0];
            NABObjPool.Instance.Recycle(newpath, gameObject);
        }
        return;
    }
    
    public Sprite getBuildSpriteRender(string id)
    {
        return Resources.Load<Sprite>(ConstValue.path_sr_name + id);
    }
    public Sprite getBuildSpriteRender(int id)
    {
        return Resources.Load<Sprite>(ConstValue.path_sr_name + id);
    }

    ///// <summary>
    ///// 销毁预制体
    ///// </summary>
    ///// <returns></returns>
    //public void DestroyPrefab(GameObject prefabObj, GameObject gameObject, string _path = null)
    //{
    //    NABObjPool.Instance.Recycle(prefabObj, gameObject, "Prefabs/" + _path);
    //    return;
    //}
    /// <summary>
    /// 加载图片
    /// </summary>
    //public void SpritPropImage(string id, Image image)
    //{
    //    string path = "Icon/" + id;
    //    Sprite Tab3Img = ResourcesLoad.Instance.Load<Sprite>(path);
    //    image.sprite = Tab3Img;
    //}

    ///// <summary>
    ///// 加载图片
    ///// </summary>
    //public void SpritPropImageByPath(string path, Image image)
    //{
    //    Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
    //    image.sprite = Tab3Img;
    //}
    ///// <summary>
    ///// 加载图片
    ///// </summary>
    //public void SpritPropImageByPath(string path, SpriteRenderer image)
    //{
    //    Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
    //    image.sprite = Tab3Img;
    //}
    ///// <summary>
    ///// 播放动画并重置动画到第0帧
    ///// </summary>
    //public void PlaySpine(SkeletonGraphic _skeletonGraphic, bool isLoop, string _spineName, bool isRest)
    //{
    //    if (isRest)
    //    {
    //        _skeletonGraphic.AnimationState.ClearTracks();
    //        _skeletonGraphic.AnimationState.Update(0);
    //    }
    //    _skeletonGraphic.AnimationState.SetAnimation(0, _spineName, isLoop);

    //    return;
    //}
    ///// <summary>
    ///// 播放动画并重置动画到第0帧
    ///// </summary>
    //public void PlaySpine(Animator _animator, string _spineName, bool isRest)
    //{
    //    //_animator.Play(_spineName, 0 ,0f);
    //    if (isRest)
    //    {
    //        //_animator.Update(0);
    //        _animator.Play(_spineName, 0, 0f);
    //    }
    //    else
    //    {
    //        _animator.Play(_spineName);
    //    }
    //    return;
    //}
    ///// <summary>
    ///// 获取对象池内对象数据
    ///// </summary>
    ///// <returns></returns>
    //public NABObjPool.PoolItem GetPoolItem(string name)
    //{
    //    string newpath = "Prefabs/" + name;
    //    return NABObjPool.Instance.GetPoolItem(newpath); ;
    //}
    ///// <summary>
    ///// 网络拉取图片
    ///// </summary>
    ///// <param name="_url"></param>
    ///// <param name="_image"></param>
    ///// <returns></returns>
    //public IEnumerator GetHead(string _url, Image _image)
    //{
    //    if (_url == string.Empty || _url == "")
    //    {
    //        _url = "https://p11.douyinpic.com/aweme/100x100/aweme-avatar/mosaic-legacy_3797_2889309425.jpeg?from=3067671334";
    //    }

    //    using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url))
    //    {
    //        yield return www.SendWebRequest();

    //        if (www.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log(www.error);
    //        }
    //        else
    //        {
    //            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    //            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1f, 1f));
    //            _image.sprite = sprite;
    //            //Renderer renderer = plane.GetComponent<Renderer>();
    //            //renderer.material.mainTexture = texture;
    //        }
    //    }
    //}

    /// <summary>
    /// 清空数据
    /// </summary>
    public void CleraPlayerData()
    {
        UserDatas.ClearLocalData();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Editor/Tools/Clear")]
    static void CleraPlayerData1()
    {
        UserDatas.ClearLocalData();
    }
#endif
    private GameObject[] GetDontDestroyOnLoadGameObjects()
    {
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        //移除所有场景包含的对象
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //移除父级不为null的对象
        int k = allGameObjects.Count;
        while (--k >= 0)
        {
            if (allGameObjects[k].transform.parent != null)
            {
                allGameObjects.RemoveAt(k);
            }
        }
        return allGameObjects.ToArray();
    }

    #endregion
}


