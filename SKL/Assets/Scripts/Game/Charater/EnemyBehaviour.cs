using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharaterBehaviour
{
    public QuaryMark qmHp;
    protected EnemyTb etb; public EnemyTb ETB { get { return etb; } }
    protected StepManager sm; public StepManager SM { get { return sm; } }

    protected ClockBase clockDie = new ClockBase();

    public virtual void inits(EnemyTb tb)
    {
        etb = tb;
        setState(CharaterStates.standby);

        string[] hpStr = GameManager.instance.CM.Split(etb.hp, "_");
        cbInfo.hp = Random.Range(int.Parse(hpStr[0]), int.Parse(hpStr[1]));
        cbInfo.speed = etb.speed;

        qmHp.onShowHp(cbInfo.hp.ToString());
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
        clockDie.InitTick(sec);
    }

    void Update()
    {
        updateDie();
    }

    protected void updateDie()
    {
        if (cbInfo.cstate == CharaterStates.die)
        {
            if (clockDie.updateTick())
            {
                MapManager.instance.stepManager.destoryEnemy(this);
            }
        }
    }
}
