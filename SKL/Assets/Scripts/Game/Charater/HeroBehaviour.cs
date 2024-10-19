using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBehaviour : CharaterBehaviour
{
    public AniBehaviour part1;

    public void inits()
    {
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

    void Update()
    {

    }

}
