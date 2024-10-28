using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharaterBehaviour
{
    protected EnemyTb etb; public EnemyTb ETB { get { return etb; } }

    protected StepManager sm; public StepManager SM { get { return sm; } }

    

    public void inits(EnemyTb tb)
    {
        etb = tb;
        setAni(CharaterStates.run);

        string[] hpStr = GameManager.instance.CM.Split(etb.hp, "_");
        cbInfo.hp = Random.Range(int.Parse(hpStr[0]), int.Parse(hpStr[1]));
        cbInfo.speed = etb.speed;
    }

    public void setDie()
    {
        MapManager.instance.stepManager.destoryEnemy(this);
    }


}
