using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharaterBehaviour
{
    public AIBehaviour aiBehaviour;
    public QuaryMark qmHp;
    protected EnemyTb etb; public EnemyTb ETB { get { return etb; } }
    protected StepManager sm; public StepManager SM { get { return sm; } }

    protected ClockBase clockDie = new ClockBase();

    protected float standTimeMin;
    protected float standTimeMax;

    public virtual void inits(EnemyTb tb)
    {
        etb = tb;
        setState(CharaterStates.standby);
        aiBehaviour.inits(this);

        string[] arrStand = GameManager.instance.CM.Split(etb.standtime, "_");
        standTimeMin = float.Parse(arrStand[0]);
        standTimeMax = float.Parse(arrStand[1]);

        string[] hpStr = GameManager.instance.CM.Split(etb.hp, "_");
        cbInfo.hp = Random.Range(int.Parse(hpStr[0]), int.Parse(hpStr[1]));
        cbInfo.speed = etb.speed;

        if (ETB.type == 1)   // 蚊子这种类型怪
        {
            qmHp.onHide();
        }
        else
        {
            qmHp.onShowHp(cbInfo.hp.ToString());
        }   
    }

    public float randStandTime() 
    {
        return Random.Range(standTimeMin, standTimeMax);
    }

    public virtual void setDie()
    {
        qmHp.onHide();
        setState(CharaterStates.die);
    }

    public void setState(CharaterStates cs)
    {
        float sec = getAniTime(cs);
        setAni(cs);
        aiBehaviour.destorys();
        clockDie.InitTick(sec);
    }

    void Update()
    {
        if (BattleHeroUI.isBattlePause)
        {
            return;
        }

        updateDie();
    }

    protected void updateDie()
    {
        if (cbInfo.cstate != CharaterStates.die)
        {
            return;
        }

        if (clockDie.updateTick())
        {
            MapManager.instance.stepManager.destoryEnemy(this);
            cbInfo.cstate = CharaterStates.none;
        }
    }
}
