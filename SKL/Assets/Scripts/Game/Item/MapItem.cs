using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MapGrid
{
    public BoxCollider bc;

    private MapItemTb tbItem;
    public MapItemTb TB { get { return tbItem; } }

    private int state;
    public int State { get { return state; } set { state = value; } }

    public void inits(string spPath, MapItemTb tb)
    {
        Sprite sprite = Resources.Load<Sprite>(spPath + tb.Sprite);
        setSprite(sprite);

        tbItem = tb;
        state = 0;
    }

    public void destoryThis()
    {
        state = 0;
        tbItem = null;
        MapManager.instance.itemManager.destoryItem(this);
    }
}
