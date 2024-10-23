using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTriggter : MonoBehaviour
{
    public HeroBehaviour hb;
    public SphereCollider sc;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter: " + other.name);
        onTrigs(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("OnTriggerStay: " + other.name);
        onTrigs(other);
    }

    private void OnTriggerExit(Collider other)
    {
    }

    private void onTrigs(Collider other)
    {
        int layer = other.gameObject.layer;
        if (layer == 7) //block
        {
            onTrigBlock(other.gameObject.GetComponent<MapBlock>());
            return;
        }
        if (layer == 8) //enemy
        {
            return;
        }
        if (layer == 9) //item
        {
            onTrigItem(other.gameObject.GetComponent<MapItem>());
            return;
        }
    }

    private void onTrigBlock(MapBlock mb)
    {

    }

    private void onTrigItem(MapItem mi)
    {
        if (mi.State != 1)
        {
            return;
        }

        hb.cbInfo.hp += mi.TB.val;
        mi.destoryThis();
        SoundManager.instance.playSound("Chop");
    }
}
