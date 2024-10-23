using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private List<MapItem> ballItems = new List<MapItem>();
    private string pathItem = "Item/mapItem";
    private string pathItemSpirte = "Maps/items/";
    private int maxBallCount = 220;
    private float viewTick = 0f;

    private MapUsed usedItems = new MapUsed(); public MapUsed UIS { get { return usedItems; } }

    private Vector3 randItemPos()
    {
        Vector3 pos;
        while (true)
        {
            pos = MapManager.instance.randomPoint();
            if (MapManager.instance.UBS.isContains((int)pos.x, (int)pos.z))
            {
                continue;
            }

            if (UIS.isContains((int)pos.x, (int)pos.z))
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
                UIS.addDic(x, z);
            }
        }

        return pos;
    }

    public void createItems()
    {
        for (int i = 0; i < maxBallCount; ++i)
        {
            createItem(1, randItemPos());
        }
    }

    public void createItem(int id, Vector3 pos)
    {
        MapItemTb tb = GameManager.instance.CM.dataMapItem.getItem(id);
        if (tb == null)
        {
            return;
        }

        GameObject go = GameManager.instance.AddPrefab(pathItem, gameObject.transform);
        go.transform.localPosition = pos;
        MapItem mi = go.GetComponent<MapItem>();
        mi.inits(pathItemSpirte, tb);
        ballItems.Add(mi);
        go.SetActive(true);
    }

    public void destoryItem(MapItem mi)
    {
        Vector3 pos = mi.gameObject.transform.localPosition;
        int maxX = (int)(pos.x + 1);
        int minX = (int)(pos.x - 1);
        int maxZ = (int)(pos.z + 1);
        int minZ = (int)(pos.z - 1);
        for (int x = minX; x <= maxX; ++x)
        {
            for (int z = minZ; z <= maxZ; ++z)
            {
                UIS.removeDic(x, z);
            }
        }
        //Debug.Log(UIS.getDicCount());

        mi.gameObject.transform.localPosition = new Vector3(GameManager.offHide, 0f, 0f);
        ballItems.Remove(mi);
        GameManager.instance.DestroyPrefab(pathItem, mi.gameObject);

    }

    void FixedUpdate()
    {
        itemUpdate();
    }

    private void itemUpdate()
    {
        viewTick += Time.deltaTime;
        if (viewTick <= 0.5f)
        {
            return;
        }
        viewTick = 0f;

        Vector3 pos = MapManager.instance.charManager.HB.gameObject.transform.localPosition;
        for (int i = 0; i < ballItems.Count; ++i)
        {
            MapItem mi = ballItems[i];
            if (mi.State != 1)
            {
                continue;
            }

            Vector3 v = mi.gameObject.transform.localPosition;
            mi.gameObject.SetActive(Vector3.Distance(pos, v) <= MapManager.instance.viewMax);
        }
    }
}
