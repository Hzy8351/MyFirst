using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoSingleton<OhayooSDKManager>
{
    public Transform parentStage;
    public Transform parentBlocks;
    public Transform parentNpcs;
    public float gridOff = 5f;

    private Dictionary<string, MapGrid> dicMapGrids = new Dictionary<string, MapGrid>();
    public Dictionary<string, MapGrid> DMGS() { return dicMapGrids; }


    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this.gameObject);



    }

    // mc哪个map, xc横向多少grid, zc竖向多少grid
    public void createMap(int mc, int xc, int zc)
    {

    }

}
