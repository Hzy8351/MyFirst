using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharaterBehaviour
{
    protected EnemyTb etb; public EnemyTb ETB { get { return etb; } }

    protected StepManager sm; public StepManager SM { get { return sm; } }

    public QuaryMark qmHp;

    public void inits(EnemyTb tb)
    {
        etb = tb;
        setAni(CharaterStates.standby);

        string[] hpStr = GameManager.instance.CM.Split(etb.hp, "_");
        cbInfo.hp = Random.Range(int.Parse(hpStr[0]), int.Parse(hpStr[1]));
        cbInfo.speed = etb.speed;

        qmHp.onShowHp(cbInfo.hp.ToString());
    }

    public void setDie()
    {
        qmHp.onHide();
        MapManager.instance.stepManager.destoryEnemy(this);
    }


}
