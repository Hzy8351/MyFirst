using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoSingleton<OhayooSDKManager>
{
    public Transform parentMap;
    public Transform parentParts;
    public Transform parentBlocks;
    public Transform parentSides;
    public Transform parentNpcs;
    public float gridOff = 5f;
    public int xcMax = 15;
    public int zcMax = 15;

    private string pathMapGrid = "Map/mapGrid";
    private string pathMapPart = "Map/mapPart";
    private string pathMapBlock = "Map/mapBlock";
    private string pathMapSide = "Map/mapSide";
    private string pathSpriteGrid = "Maps/map";
    private float rangeXMax;
    private float rangeXMin;
    private float rangeZMax;
    private float rangeZMin;


    private Dictionary<string, MapGrid> dicMapGrids = new Dictionary<string, MapGrid>();
    public Dictionary<string, MapGrid> DMGS() { return dicMapGrids; }


    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this.gameObject);

        GameManager.instance.CM.Init();

        createMap(1, 15, 15, 2);
        createParts(1, 130, 5);
        createBlocks(1, 70, 4);
        createSides(1);
    }

    // mc哪个map, xc横向多少grid, zc竖向多少grid, 边缘多少个grid
    public void createMap(int mc, int xc, int zc, int sc)
    {
        rangeXMax = gridOff * (xc - sc - 1);
        rangeXMin = -rangeXMax;
        rangeZMax = gridOff * (zc - sc - 1);
        rangeZMin = -rangeZMax;

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
            }

            --isc;
        }


    }

    // c是数量, tc类型数量
    public void createParts(int mc, int c, int tc)
    {
        string path = pathSpriteGrid + mc + "/";

        for (int i = 0; i < c; ++i)
        {
            GameObject go = GameManager.instance.AddPrefab(pathMapPart, parentParts);
            MapPart mp = go.GetComponent<MapPart>();
            int r = Random.Range(0, tc);
            Sprite sprite = Resources.Load<Sprite>(path + "parts" + r);
            mp.setSprite(sprite);

            go.transform.localPosition = randomPoint();
            go.SetActive(true);
        }
    }

    public void createBlocks(int mc, int c, int tc)
    {
        string path = pathSpriteGrid + mc + "/";

        for (int i = 0; i < c; ++i)
        {
            GameObject go = GameManager.instance.AddPrefab(pathMapBlock, parentBlocks);
            MapBlock mb = go.GetComponent<MapBlock>();
            int r = Random.Range(0, tc);
            Sprite sprite = Resources.Load<Sprite>(path + "block" + r);
            mb.setSprite(sprite);

            go.transform.localPosition = randomPoint();
            go.SetActive(true);
        }
    }

    public void createSides(int mc)
    {

    }

    public Vector3 randomPoint()
    {
        return new Vector3(Random.Range(rangeXMin, rangeXMax), 0f, Random.Range(rangeZMin, rangeZMax));
    }

}
