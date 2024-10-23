using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    public CameraBehaviour cameraBehaviour;
    public CharaterManager charManager;
    public ItemManager itemManager;
    public Transform parentMap;
    public Transform parentParts;
    public Transform parentBlocks;
    public Transform parentSides;
    public float gridOff = 5f;
    public int xcMax = 15;
    public int zcMax = 15;
    public float viewMax = 25f;

    private string pathMapGrid = "Map/mapGrid";
    private string pathMapPart = "Map/mapPart";
    private string pathMapBlock = "Map/mapBlock";
    private string pathMapSide = "Map/mapSide";
    private string pathSpriteGrid = "Maps/map";

    private string pathHero = "Charater/hero";

    private float rangeXMax;
    private float rangeXMin;
    private float rangeZMax;
    private float rangeZMin;
    private float charXMax;
    private float charXMin;
    private float charZMax;
    private float charZMin;

    //private Dictionary<string, MapGrid> dicMapGrids = new Dictionary<string, MapGrid>(); public Dictionary<string, MapGrid> DMGS() { return dicMapGrids; }

    private float mapViewTick = 0f;
    private List<GameObject> mapViewObjs = new List<GameObject>();
    private MapUsed usedBlocks = new MapUsed(); public MapUsed UBS { get { return usedBlocks; } }
    private MapUsed usedParts = new MapUsed(); public MapUsed UPS { get { return usedParts; } }


    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this.gameObject);

        GameManager.instance.CM.Init();
        SoundManager.instance.playMusic("BG");

        createMap(1, 15, 15, 3);
        createSides(1);
        //initsUsePoses();

        createBlocks(1, 70);
        createParts(1, 130, 5);

        createUI();

        charManager.inits();
        createHero();
        itemManager.createItems();
    }

    #region maps

    // mc哪个map, xc横向多少grid, zc竖向多少grid, 边缘多少个grid
    private void createMap(int mc, int xc, int zc, int sc)
    {
        rangeXMax = gridOff * (xc - sc - 0.5f);
        rangeXMin = -rangeXMax;
        rangeZMax = gridOff * (zc - sc - 0.5f);
        rangeZMin = -rangeZMax;

        charXMax = gridOff * (xc - sc);
        charXMin = -charXMax;
        charZMax = gridOff * (zc - sc);
        charZMin = -charZMax;

        string gridPath = pathSpriteGrid + mc + "/";

        if (xc > xcMax) { xc = xcMax; }
        if (zc > zcMax) { zc = zcMax; }
        int isc = sc;
        for (int i = -xc; i <= xc; ++i)
        {
            bool bSideI = (isc > 0) || (i > xc - sc);
            int ksc = sc;
            for (int k = -zc; k <= zc; ++k)
            {
                GameObject go = GameManager.instance.AddPrefab(pathMapGrid, parentMap);
                MapGrid mg = go.GetComponent<MapGrid>();

                // 暂时写死里面为1, 外面为2
                bool bSideK = (ksc > 0) || (k > zc - sc);
                string gridName = (bSideI || bSideK) ? "grid2" : "grid1";
                Sprite sprite = Resources.Load<Sprite>(gridPath + gridName);
                mg.setSprite(sprite);
                if (bSideI || bSideK) { mg.setOrder(mg.getOrder() - 10); }

                go.transform.localPosition = new Vector3(i * gridOff, 0f, k * gridOff);
                go.SetActive(true);
                --ksc;

                mapViewObjs.Add(go);
            }

            --isc;
        }


    }

    private void createSides(int mc)
    {
    }

    //private void initsUsePoses()
    //{
    //    for (int x = (int)rangeXMin; x <= (int)rangeXMax; ++x)
    //    {
    //        for (int z = (int)rangeZMin; z <= (int)rangeZMax; ++z)
    //        {
    //            if (Mathf.Abs(x) <= 5 && Mathf.Abs(z) <= 5)
    //            {
    //                continue;
    //            }

    //            UBS.addDic(x, z, null);
    //            UPS.addDic(x, z, null);
    //            itemManager.UIS.addDic(x, z, null);
    //        }
    //    }
    //}

    private bool isHeroInitPos(Vector3 pos)
    {
        return (pos.x >= -2f && pos.x <= 2f) && (pos.z >= -2f && pos.z <= 2f);
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

    private void createBlocks(int mc, int c)
    {
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
    private void createParts(int mc, int c, int tc)
    {
        string path = pathSpriteGrid + mc + "/";

        for (int i = 0; i < c; ++i)
        {
            GameObject go = GameManager.instance.AddPrefab(pathMapPart, parentParts);
            MapPart mp = go.GetComponent<MapPart>();
            int r = Random.Range(1, tc + 1);
            Sprite sprite = Resources.Load<Sprite>(path + "parts" + r);
            mp.setSprite(sprite);
            go.transform.localPosition = randPartPos();
            go.SetActive(true);

            mapViewObjs.Add(go);
        }
    }

    public Vector3 randomPoint()
    {
        return new Vector3(Random.Range(rangeXMin, rangeXMax), 0f, Random.Range(rangeZMin, rangeZMax));
    }

    #endregion

    #region charaters
    private void createHero()
    {
        GameObject go = GameManager.instance.AddPrefab(pathHero, charManager.transform);
        HeroBehaviour hb = go.GetComponent<HeroBehaviour>();
        hb.inits();
        cameraBehaviour.setCB(hb);
        ((JoyStickUI)UIManager.instance.GetUI(UIEnum.JoyStickUI)).setCB(hb);
        charManager.HB = hb;
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

    #endregion

    #region ui
    public void createUI()
    {
        createJoyUI();
    }

    private void createJoyUI()
    {
        UIManager.instance.Show(UIEnum.JoyStickUI);
    }
    #endregion

    void FixedUpdate()
    {
        updateMapViews();
    }

    private void updateMapViews()
    {
        if (charManager.HB == null)
        {
            return;
        }

        mapViewTick += Time.deltaTime;
        if (mapViewTick <= 0.5f)
        {
            return;
        }
        mapViewTick = 0f;

        Vector3 pos = charManager.HB.gameObject.transform.localPosition;
        for (int i = 0; i < mapViewObjs.Count; ++i)
        {
            GameObject obj = mapViewObjs[i];
            Vector3 v = obj.transform.localPosition;
            obj.SetActive(Vector3.Distance(pos, v) <= viewMax);
        }
    }
}
