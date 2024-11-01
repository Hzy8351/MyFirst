using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSide : MapGrid
{
    public void inits(string spPath, MapSideTb tb)
    {
        Sprite sprite = Resources.Load<Sprite>(spPath + tb.sprite);
        setSprite(sprite);
        sp.sortingOrder = tb.order;

        string[] pos = GameManager.instance.CM.Split(tb.pos, "|");
        gameObject.transform.localPosition = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));

        string[] rot = GameManager.instance.CM.Split(tb.rot, "|");
        Quaternion q = gameObject.transform.rotation;
        q.eulerAngles = new Vector3(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]));
        gameObject.transform.rotation = q;
    }
}
