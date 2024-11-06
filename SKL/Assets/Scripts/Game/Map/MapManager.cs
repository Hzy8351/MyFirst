using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    public CameraBehaviour cameraBehaviour;
    public CharaterManager charManager;
    public ItemManager itemManager;
    public StepManager stepManager;
    public Transform parentMap;
    public Transform parentParts;
    public Transform parentBlocks;
    public Transform parentSides;

    private string pathMapGrid = "Map/mapGrid";
    private string pathMapPart = "Map/mapPart";
    private string pathMapBlock = "Map/mapBlock";
    private string pathMapSide = "Map/mapSide";
    private string pathSpriteGrid = "Maps/map";

    private string pathHero = "Charater/hero";
    private string pathEnemy = "Charater/";

    private float rangeXMax;
    private float rangeXMin;
    private float rangeZMax;
    private float rangeZMin;
    private float charXMax;
    private float charXMin;
    private float charZMax;
    private float charZMin;
    private float enemyXMax;
    private float enemyXMin;
    private float enemyZMax;
    private float enemyZMin;
    private float mapViewMax; public float VCMAX { get { return mapViewMax; } }

    private float mapViewTick;
    private List<viewScaleData> heroScaleList = new List<viewScaleData>();
    private List<GameObject> mapViewObjs = new List<GameObject>();
    private List<GameObject> mapSideObjs = new List<GameObject>();

    private MapUsed usedBlocks = new MapUsed(); public MapUsed UBS { get { return usedBlocks; } }
    private MapUsed usedParts = new MapUsed(); public MapUsed UPS { get { return usedParts; } }
    private CfgData cfgData = new CfgData(); public CfgData CFGD { get { return cfgData; } }
    private GameData gameData = new GameData(); public GameData GD { get { return gameData; } }

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this.gameObject);

        mapViewTick = 0f;
        heroScaleList.Clear();
        mapViewObjs.Clear();
        mapSideObjs.Clear();

        GameManager.instance.CM.Init();
        initCfgData();

        StageTb stb = GameManager.instance.CM.dataStage.getStageOfMapStage(1, 1);
        initGameData(stb);
        createMap(stb);
        createSides(stb);
        createBlocks(stb);
        createParts(stb);

        charManager.inits();
        charManager.HB = createHero(stb);

        createUI(charManager.HB, stb);
        cameraBehaviour.setCB(charManager.HB);
        stepManager.inits(charManager.HB, stb);
        itemManager.createItems(stb);

        charManager.HB.node.localScale = Vector3.one;
        mapViewMax = CFGD.minMapScale;
        cameraBehaviour.setCameraSize(CFGD.minCameraScale);

        SoundManager.instance.playMusic("BG");
    }

    private void initCfgData()
    {
        CommonTb tb1000 = GameManager.instance.CM.dataCommon.getItem(1000);
        CFGD.xcMax = int.Parse(tb1000.Para1);
        CFGD.zcMax = int.Parse(tb1000.Para2);
        CFGD.gridOff = float.Parse(tb1000.Para3);
        CFGD.mapViewTickMax = float.Parse(tb1000.Para4);

        CommonTb tb1001 = GameManager.instance.CM.dataCommon.getItem(1001);
        string[] arr1 = GameManager.instance.CM.Split(tb1001.Para1, "_");
        CFGD.minHeroScale = float.Parse(arr1[0]);
        CFGD.maxHeroScale = float.Parse(arr1[1]);

        string[] arr2 = GameManager.instance.CM.Split(tb1001.Para2, "_");
        CFGD.minCameraScale = float.Parse(arr2[0]);
        CFGD.maxCameraScale = float.Parse(arr2[1]);

        string[] arr3 = GameManager.instance.CM.Split(tb1001.Para3, "_");
        CFGD.minMapScale = float.Parse(arr3[0]);
        CFGD.maxMapScale = float.Parse(arr3[1]);
        CFGD.sideMapViewDisOff = float.Parse(arr3[2]);

        string[] arr4 = GameManager.instance.CM.Split(tb1001.Para4, "_");
        CFGD.heroHpScaleBegin = int.Parse(arr4[0]);
        CFGD.heroHpScaleEnd = int.Parse(arr4[1]);

        CommonTb tb1002 = GameManager.instance.CM.dataCommon.getItem(1002);
        CFGD.heroPosRangeInit = int.Parse(tb1002.Para1);
        CFGD.heroMinHpCheck = int.Parse(tb1002.Para2);
        CFGD.heroSpeed = float.Parse(tb1002.Para3);
        CFGD.heroScaleTime = float.Parse(tb1002.Para4);

    }

    private void initGameData(StageTb tb)
    {
        GD.maxHeroScoreHp = tb.maxscore;

        float per = 1f / (GD.maxHeroScoreHp - getHpScaleMinusVal());
        GD.perHeroScale = (CFGD.maxHeroScale - CFGD.minHeroScale) * per;
        GD.perCameraScale = (CFGD.maxCameraScale - CFGD.minCameraScale) * per;
        GD.perMapScale = (CFGD.maxMapScale - CFGD.minMapScale) * per;
    }

    #region maps

    // mc哪个map, xc横向多少grid, zc竖向多少grid, 边缘多少个grid
    private void createMap(StageTb stb)
    {
        int mc = stb.map;
        int xc = stb.xgrid;
        int zc = stb.zgrid;
        int sc = stb.sidegrid;

        rangeXMax = CFGD.gridOff * (xc - sc - 0.5f);
        rangeXMin = -rangeXMax;
        rangeZMax = CFGD.gridOff * (zc - sc - 0.5f);
        rangeZMin = -rangeZMax;

        charXMax = CFGD.gridOff * (xc - sc);
        charXMin = -charXMax;
        charZMax = CFGD.gridOff * (zc - sc);
        charZMin = -(CFGD.gridOff * (zc - sc - 0.3f));//-charZMax;

        enemyXMax = CFGD.gridOff * (xc - sc - 1.5f);
        enemyXMin = -enemyXMax;
        enemyZMax = CFGD.gridOff * (zc - sc - 1.5f);
        enemyZMin = -enemyZMax;

        string gridPath = pathSpriteGrid + mc + "/";

        if (xc > CFGD.xcMax) { xc = CFGD.xcMax; }
        if (zc > CFGD.zcMax) { zc = CFGD.zcMax; }
        int isc = sc;
        for (int i = -xc; i <= xc; ++i)
        {
            bool bSideI = (isc > 0) || (i > xc - sc);
            int ksc = sc;
            for (int k = -zc; k <= zc; ++k)
            {
                GameObject go = GameManager.instance.AddPrefab(pathMapGrid, parentMap);
                MapGrid mg = go.GetComponent<MapGrid>();

                bool bSideK = (ksc > 0) || (k > zc - sc);
                string gridName = (bSideI || bSideK) ? stb.sidegridspirte : stb.gridspirte;
                Sprite sprite = Resources.Load<Sprite>(gridPath + gridName);
                mg.setSprite(sprite);
                if (bSideI || bSideK) { mg.setOrder(mg.getOrder() - 10); }

                go.transform.localPosition = new Vector3(i * CFGD.gridOff, 0f, k * CFGD.gridOff);
                go.SetActive(true);
                --ksc;

                mapViewObjs.Add(go);
            }

            --isc;
        }


    }


    private void createSides(StageTb stb)
    {
        string path = pathSpriteGrid + stb.map + "/";

        List<MapSideTb> lists = GameManager.instance.CM.dataMapSide.getPartsOfMap(stb.map);
        for (int i = 0; i < lists.Count; ++i)
        {
            MapSideTb mst = lists[i];
            GameObject go = GameManager.instance.AddPrefab(pathMapSide, parentSides);
            MapSide ms = go.GetComponent<MapSide>();
            ms.inits(path, mst);
            go.SetActive(true);

            mapSideObjs.Add(go);
        }

    }

    private bool isHeroInitPos(Vector3 pos)
    {
        return (pos.x >= -CFGD.heroPosRangeInit && pos.x <= CFGD.heroPosRangeInit) && (pos.z >= -CFGD.heroPosRangeInit && pos.z <= CFGD.heroPosRangeInit);
    }

    private Vector3 randBlockPos(Vector3 size)
    {
        Vector3 pos;
        while (true)
        {
            pos = randomPoint();
            if (isHeroInitPos(pos))
            {
                continue;
            }

            if (UBS.isContains((int)pos.x, (int)pos.z))
            {
                continue;
            }

            break;
        }

        int maxX = (int)(pos.x + size.x + 1);
        int minX = (int)(pos.x - size.x - 1);
        int maxZ = (int)(pos.z + size.z + 1);
        int minZ = (int)(pos.z - size.z - 1);
        for (int x = minX; x <= maxX; ++x)
        {
            for (int z = minZ; z <= maxZ; ++z)
            {
                UBS.addDic(x, z);
            }
        }

        return pos;
    }

    private void createBlocks(StageTb stb)
    {
        int c = stb.blockcount;
        int mc = stb.map;
        string path = pathSpriteGrid + mc + "/";

        List<MapBlockTb> blocks = GameManager.instance.CM.dataMapBlock.getBlocksOfMap(mc);

        for (int i = 0; i < c; ++i)
        {
            MapBlockTb tb = blocks[Random.Range(0, blocks.Count)];
            GameObject go = GameManager.instance.AddPrefab(pathMapBlock, parentBlocks);
            MapBlock mb = go.GetComponent<MapBlock>();
            mb.inits(path, tb);
            go.transform.localPosition = randBlockPos(mb.bc.size);
            go.SetActive(true);

            mapViewObjs.Add(go);
        }
    }


    private Vector3 randPartPos()
    {
        Vector3 pos;
        while (true)
        {
            pos = randomPoint();
            if (UBS.isContains((int)pos.x, (int)pos.z))
            {
                continue;
            }

            if (UPS.isContains((int)pos.x, (int)pos.z))
            {
                continue;
            }

            break;
        }

        int maxX = (int)(pos.x + 1);
        int minX = (int)(pos.x - 1);
        int maxZ = (int)(pos.z + 1);
        int minZ = (int)(pos.z - 1);
        for (int x = minX; x <= maxX; ++x)
        {
            for (int z = minZ; z <= maxZ; ++z)
            {
                UPS.addDic(x, z);
            }
        }

        return pos;
    }
    
    // c是数量, tc类型数量
    private void createParts(StageTb stb)
    {
        int c = stb.partscount;
        int mc = stb.map;
        string path = pathSpriteGrid + mc + "/";

        List<MapPartsTb> parts = GameManager.instance.CM.dataMapParts.getPartsOfMap(mc);

        for (int i = 0; i < c; ++i)
        {
            MapPartsTb tb = parts[Random.Range(0, parts.Count)];
            GameObject go = GameManager.instance.AddPrefab(pathMapPart, parentParts);
            MapPart mp = go.GetComponent<MapPart>();
            mp.setSprite(Resources.Load<Sprite>(path + tb.sprite));
            go.transform.localPosition = randPartPos();
            go.SetActive(true);

            mapViewObjs.Add(go);
        }
    }

    public Vector3 randomPoint()
    {
        return new Vector3(Random.Range(rangeXMin, rangeXMax), 0f, Random.Range(rangeZMin, rangeZMax));
    }

    public Vector3 randomEnemyPoint()
    {
        return new Vector3(Random.Range(enemyXMin, enemyXMax), 0f, Random.Range(enemyZMin, enemyZMax));
    }

    public Vector3 randSpEnemyPos()
    {
        Vector3 pos;
        float dis = CFGD.heroPosRangeInit * 3;
        while (true)
        {
            pos = randomEnemyPoint();
            if (Vector3.Distance(pos, charManager.HB.transform.localPosition) <= dis)
            {
                continue;
            }
            break;
        }
        return pos;
    }

    #endregion

    #region charaters
    public HeroBehaviour getHB()
    {
        return charManager.HB;
    }

    public EnemyBehaviour createEnemy(EnemyTb tb)
    {
        GameObject go = GameManager.instance.AddPrefab(pathEnemy + tb.spine, charManager.gameObject.transform);
        EnemyBehaviour eb = go.GetComponent<EnemyBehaviour>();
        Vector3 pos;
        while (true)
        {
            pos = randomEnemyPoint();
            if (Vector3.Distance(pos, charManager.HB.transform.localPosition) <= CFGD.heroPosRangeInit)
            {
                continue;
            }
            if (UBS.isContains((int)pos.x, (int)pos.z))
            {
                continue;
            }
            break;
        }
        go.transform.localPosition = pos;
        eb.inits(tb);

        return eb;
    }

    public EnemyBehaviour createSpEnemy(EnemyTb tb)
    {
        GameObject go = GameManager.instance.AddPrefab(pathEnemy + tb.spine, charManager.gameObject.transform);
        EnemyBehaviour eb = go.GetComponent<EnemyBehaviour>();
        go.transform.localPosition = randSpEnemyPos();
        eb.inits(tb);

        return eb;
    }

    public void destoryEnemy(EnemyBehaviour eb)
    {
        eb.transform.localPosition = new Vector3(GameManager.offHide, 0f, 0f);
        GameManager.instance.DestroyPrefab(pathEnemy + eb.ETB.spine, eb.gameObject);
    }

    private HeroBehaviour createHero(StageTb stb)
    {
        GameObject go = GameManager.instance.AddPrefab(pathHero, charManager.transform);
        HeroBehaviour hb = go.GetComponent<HeroBehaviour>();
        hb.inits(CFGD.heroSpeed);
        return hb;
    }

    public void checkCharaterRange(CharaterBehaviour cb)
    {
        bool b = false;
        Vector3 pos = cb.transform.position;
        if (pos.x > charXMax)
        {
            b = true;
            pos.x = charXMax;
        }
        else if (pos.x < charXMin)
        {
            b = true;
            pos.x = charXMin;
        }

        if (pos.z > charZMax)
        {
            b = true;
            pos.z = charZMax;
        }
        else if (pos.z < charZMin)
        {
            b = true;
            pos.z = charZMin;
        }

        if (b)
        {
            cb.transform.position = pos;
        }
    }

    public Vector3 checkEnemyRange(Vector3 pos)
    {
        if (pos.x > enemyXMax)
        {
            pos.x = enemyXMax;
        }
        else if (pos.x < enemyXMin)
        {

            pos.x = enemyXMin;
        }

        if (pos.z > enemyZMax)
        {
            pos.z = enemyZMax;
        }
        else if (pos.z < enemyZMin)
        {
            pos.z = enemyZMin;
        }

        return pos;
    }

    #endregion

    #region ui
    public void createUI(HeroBehaviour hb, StageTb tb)
    {
        ((JoyStickUI)UIManager.instance.Show(UIEnum.JoyStickUI)).setCB(charManager.HB);
        ((BattleHeroUI)UIManager.instance.Show(UIEnum.BattleHeroUI)).inits(charManager.HB, tb);
    }


    #endregion

    #region CreateTextTips(string dec,string num)
    //public void CreateTextTips(string des, string num)
    //{
    //    ViewTextTips("<size=45><color=#38a1d7>" + des + ":</color>" + "<color=#63c42b>" + num + "</color></size>");
    //}

    public void CreateTextTips(Vector3 pos, string des, int size = 80, float dic = 120f, float sec = 0.8f, string color16 = "24d9f1")
    {
        ViewTextTips(pos, string.Format("<size={0}><color=#{1}>{2}</color></size>", size, color16, des), dic, sec);
    }

    private void ViewTextTips(Vector3 pos, string des, float dic = 120f, float sec = 0.5f)
    {
        string path = "Prefabs/UI/TextTips";
        GameObject obj = Instantiate(ResourcesLoad.Instance.Load<GameObject>(path), CanvasManager.instance.tranTips);
        obj.GetComponent<TextTipsCtrl>().Init(pos, des, dic, sec);
    }

    #endregion

    void Update()
    {
        if (BattleHeroUI.isBattlePause)
        {
            return;
        }

        updateMapViews();
        updateHeroScaleAni();
        updatePer();
    }

    public void addViewScaleList(int hp)
    {
        viewScaleData vsd = new viewScaleData();
        if (hp >= 0)
        {
            vsd.hpMax = hp;
            vsd.hpLeft = hp;
            vsd.mark = 1;
        }
        else
        {
            vsd.hpMax = -hp;
            vsd.hpLeft = -hp;
            vsd.mark = -1;
        }
        heroScaleList.Add(vsd);
    }

    public float getScaleRate()
    {
        if (charManager.HB.aniHp <= CFGD.heroHpScaleBegin)
        {
            return 1f;
        }

        float perHp = (charManager.HB.aniHp - CFGD.heroHpScaleBegin);
        float maxHp = (GD.maxHeroScoreHp - getHpScaleMinusVal());
        return 1f + perHp / maxHp;
    }

    public int getHpScaleMinusVal()
    {
        return CFGD.heroHpScaleBegin + CFGD.heroHpScaleEnd;
    }

    private void updateMapViews()
    {
        if (charManager.HB == null)
        {
            return;
        }

        mapViewTick += Time.deltaTime;
        if (mapViewTick <= CFGD.mapViewTickMax)
        {
            return;
        }
        mapViewTick = 0f;

        Vector3 pos = charManager.HB.gameObject.transform.localPosition;
        for (int i = 0; i < mapViewObjs.Count; ++i)
        {
            GameObject obj = mapViewObjs[i];
            Vector3 v = obj.transform.localPosition;
            obj.SetActive(Vector3.Distance(pos, v) <= mapViewMax);
        }
        for (int i = 0; i < mapSideObjs.Count; ++i)
        {
            GameObject obj = mapSideObjs[i];
            Vector3 v = obj.transform.localPosition;
            obj.SetActive(Vector3.Distance(pos, v) <= mapViewMax + CFGD.sideMapViewDisOff);
        }

    }

    private void updateHeroScaleAni()
    {
        for (int i = 0; i < heroScaleList.Count;)
        {
            viewScaleData vsd = heroScaleList[i];
            if (vsd.hpLeft <= 0f)
            {
                heroScaleList.RemoveAt(i);
                continue;
            }

            int val = (int)(vsd.hpMax * (Time.deltaTime * CFGD.heroScaleTime));
            if (val < 1)
            {
                val = 1;
            }
            vsd.hpLeft -= val;

            charManager.HB.aniHp += (vsd.mark * val);
            if (charManager.HB.aniHp < 0) { charManager.HB.aniHp = 0; }
            else if (charManager.HB.aniHp > GD.maxHeroScoreHp) { charManager.HB.aniHp = GD.maxHeroScoreHp; }
            ++i;
            //Debug.Log("aniHp = " + charManager.HB.aniHp);
        }
    }

    private void updatePer()
    {
        if (charManager.HB == null)
        {
            return;
        }

        if (heroScaleList.Count <= 0)
        {
            charManager.HB.aniHp = charManager.HB.cbInfo.hp;
        }

        int perHp = charManager.HB.aniHp - getHpScaleMinusVal();
        if (perHp < 0) { perHp = 0; }

        charManager.HB.node.localScale = Vector3.one * (CFGD.minHeroScale + perHp * GD.perHeroScale);
        mapViewMax = CFGD.minMapScale + perHp * GD.perMapScale;
        cameraBehaviour.setCameraSize(CFGD.minCameraScale + perHp * GD.perCameraScale);
    }

}
