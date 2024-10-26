using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MapGrid
{
    public BoxCollider bc;

    public void inits(string spPath, MapBlockTb tb)
    {
        Sprite sprite = Resources.Load<Sprite>(spPath + tb.sprite);
        setSprite(sprite);

        string[] cets = GameManager.instance.CM.Split(tb.center, "|");
        bc.center = new Vector3(float.Parse(cets[0]), float.Parse(cets[1]), float.Parse(cets[2]));

        string[] sizes = GameManager.instance.CM.Split(tb.size, "|");
        bc.size = new Vector3(float.Parse(sizes[0]), float.Parse(sizes[1]), float.Parse(sizes[2]));
    }

}
