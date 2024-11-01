using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBehaviour : CharaterBehaviour
{
    public AniBehaviour part1;
    public Transform tranFont;
    public int aniHp;

    public void inits(float speed)
    {
        cbInfo.speed = speed;
        setAni(CharaterStates.standby);
    }

    public override void setAni(CharaterStates cs)
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
        part1.playAni(aniState);
    }

    public void createHpTips(int hp)
    {
        if (hp >= 0)
        {
            MapManager.instance.CreateTextTips(CTUtils.World2Screen(tranFont.position), "+" + hp, 76, 80, 0.5f, "f3140b");
            return;
        }

        MapManager.instance.CreateTextTips(CTUtils.World2Screen(tranFont.position), hp.ToString(), 76, 80, 0.5f);
    }

}
