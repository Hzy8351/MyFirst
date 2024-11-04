using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : EnemyBehaviour
{
    public AniBehaviour part1;

    public override void inits(EnemyTb tb)
    {
        base.inits(tb);
        part1.gameObject.SetActive(false);
    }

    public override void setDie()
    {
        base.setDie();
        part1.gameObject.SetActive(false);
    }

    void Update()
    {
        updateDie();
    }
}

