using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private List<MapItem> ballItems = new List<MapItem>();
    private string pathItem = "Item/mapItem";
    private string pathItemSpirte = "Maps/items/";
    private int maxBallCount = 220;

    public void createItems()
    {
        for (int i = 0; i < maxBallCount; ++i)
        {
            createItem(1, MapManager.instance.randomPoint());
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
        mi.gameObject.transform.localPosition = new Vector3(GameManager.offHide, 0f, 0f);
        ballItems.Remove(mi);
        GameManager.instance.DestroyPrefab(pathItem, mi.gameObject);
    }

    private void itemUpdate()
    {
    }

    void Update()
    {
        itemUpdate();
    }
}
