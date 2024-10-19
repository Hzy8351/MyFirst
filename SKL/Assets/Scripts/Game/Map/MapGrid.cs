using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public SpriteRenderer sp;

    public void setSprite(Sprite s)
    {
        sp.sprite = s; 
    }

    public void setOrder(int order)
    {
        sp.sortingOrder = order;
    }

    public int getOrder()
    {
        return sp.sortingOrder; 
    }
}
