using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MapGrid
{
    public BoxCollider bc;

    public void inits(string spPath, MapBlockTb tb)
    {
        Sprite sprite = Resources.Load<Sprite>(spPath + tb.Sprite);
        setSprite(sprite);

        //string[] cets = GameManager.instance.CM.Split(tb.Center, "|");
        //bc.center = new Vector3(float.Parse(cets[0]), float.Parse(cets[1]), float.Parse(cets[2]));

        //string[] sizes = GameManager.instance.CM.Split(tb.Size, "|");
        //bc.size = new Vector3(float.Parse(sizes[0]), float.Parse(sizes[1]), float.Parse(sizes[2]));
    }

}
