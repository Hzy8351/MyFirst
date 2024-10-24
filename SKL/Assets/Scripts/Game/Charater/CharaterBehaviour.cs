using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharaterBaseInfo
{
    public float speed = 8.0f;
    public float speedBuff = 0f;
    public CharaterStates cstate = CharaterStates.none;
    public Vector3 offvec;
    //public float radius;
    public int hp = 10;
    public int lv = 1;
}

public class CharaterBehaviour : MonoBehaviour
{
    public CharaterBaseInfo cbInfo;
    public Transform node;
    public AniBehaviour body;

    public void setDirect(Vector3 direct)
    {
        float r = (direct.x >= 0f) ? 0f : 180f;
        Quaternion q = node.localRotation;
        q.z = r;
        node.localRotation = q;
    }

    public bool isRunUp()
    {
        return cbInfo.speedBuff > 0f;
    }

    public float getCurSpeed()
    {
        return cbInfo.speed + cbInfo.speedBuff;
    }

    public void move(Vector3 v)
    {
        transform.position += v;
        MapManager.instance.checkCharaterRange(this);
    }

    public virtual void setAni(CharaterStates cs)
    {
        if (cs == cbInfo.cstate)
        {
            return;
        }

        string aniState = MapManager.instance.charManager.getAniState(cs);
        if (aniState == "")
        {
            return;
        }

        cbInfo.cstate = cs;
        body.playAni(aniState);
    }
}
