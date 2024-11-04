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

    public void setDirect(float x)
    {
        float r = (x >= 0f) ? 0f : 180f;
        Quaternion q = node.localRotation;
        q.z = r;
        node.localRotation = q;
    }

    private bool isRunUp()
    {
        return cbInfo.speedBuff > 0f;
    }

    private float getCurSpeed()
    {
        return (cbInfo.speed + cbInfo.speedBuff) * MapManager.instance.getScaleRate();
    }

    private void move(Vector3 v)
    {
        transform.position += v;
        MapManager.instance.checkCharaterRange(this);
    }

    public void moveTo(float x, float z)
    {
        float sp = getCurSpeed();
        move(new Vector3(sp * x * Time.deltaTime, 0f, sp * z * Time.deltaTime));
        setDirect(x);
        setAni(isRunUp() ? CharaterStates.run_up : CharaterStates.run);
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

    public virtual float getAniTime(CharaterStates cs)
    {
        return body.getAniTime(MapManager.instance.charManager.getAniState(cs));
    }

    public void addHp(int val, bool aniHp)
    {
        cbInfo.hp += val;
        if (cbInfo.hp <= 0)
        {
            cbInfo.hp = 0;
        }
        else if (cbInfo.hp > MapManager.instance.GD.maxHeroScoreHp)
        {
            cbInfo.hp = MapManager.instance.GD.maxHeroScoreHp;
        }

        if (aniHp)
        {
            MapManager.instance.addViewScaleList(val);
        }
    }

    public int addTrigDamage(bool aniHp)
    {
        int hp = cbInfo.hp;
        int damage = (hp >= MapManager.instance.CFGD.heroMinHpCheck) ? hp / 2 : hp;
        addHp(-damage, aniHp);
        return damage;
    }
}
