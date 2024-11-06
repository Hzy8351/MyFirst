using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    private HeroBehaviour hb;
    private StageTb stb;
    private int step;

    private BattleDatas bd;
    private int[] stepScores;

    public void inits(HeroBehaviour h, StageTb tb)
    {
        hb = h;
        stb = tb;

        step = 1;
        GameManager.instance.CM.Split(stb.stepscore, "|", ref stepScores);
        bd = new BattleDatas();
        bd.inits(stb, stepScores.Length);
    }

    public int getStep()
    {
        return step;
    }

    public void destoryEnemy(EnemyBehaviour eb)
    {
        List<EnemyBehaviour> listEnemys = bd.dicEnemys[eb.ETB.spine].enemys;
        if (listEnemys.Contains(eb))
        {
            listEnemys.Remove(eb);
        }
        MapManager.instance.destoryEnemy(eb);
    }

    void FixedUpdate()
    {
        updateStep();
        updateStepEnemys();
    }

    private void updateStep()
    { 
        if (step == stepScores.Length)
        {
            return;
        }

        int tmp = 0;
        for (int i = 0; i < stepScores.Length; ++i)
        {
            if (hb.cbInfo.hp >= stepScores[i])
            {
                ++tmp;
            }
            else
            {
                break;
            }
        }

        step = tmp;
    }

    private void updateStepEnemys()
    {
        StepTableData std = bd.dicTables[step];
        updateEnemyType1(std);
        updateEnemyType2(std);
        updateEnemyType3(std);
        updateItemType2(std);
    }

    private void updateEnemyType1(StepTableData std)
    {
        for (int i = 0; i < std.enemyType1.Count; ++i)
        {
            FreshData fd = std.enemyType1[i];
            if (fd.id == 0)
            {
                continue;
            }

            fd.tick += Time.deltaTime;
            if (fd.tick >= fd.para1)
            {
                fd.tick = 0f;
                EnemyTb tb = GameManager.instance.CM.dataEnemy.getItem(fd.id);
                EnemyBehaviour eb = MapManager.instance.createSpEnemy(tb);
                bd.getDicEnemy(tb.spine).enemys.Add(eb);
            }
        }
    }

    private void updateEnemyType2(StepTableData std)
    {
        for (int i = 0; i < std.enemyType2.Count; ++i)
        {
            FreshData fd = std.enemyType2[i];
            if (fd.id == 0)
            {
                continue;
            }

            EnemyTb tb = GameManager.instance.CM.dataEnemy.getItem(fd.id);
            StepEnemyData sed = bd.getDicEnemy(tb.spine);
            if (sed.enemys.Count < fd.para1)
            {
                EnemyBehaviour eb = MapManager.instance.createEnemy(tb);
                sed.enemys.Add(eb);
            }
        }
    }

    private void updateEnemyType3(StepTableData std)
    {
        for (int i = 0; i < std.enemyType3.Count; ++i)
        {
            FreshData fd = std.enemyType3[i];
            if (fd.id == 0)
            {
                continue;
            }

            EnemyTb tb = GameManager.instance.CM.dataEnemy.getItem(fd.id);
            StepEnemyData sed = bd.getDicEnemy(tb.spine);
            if (sed.enemys.Count < fd.para1)
            {
                EnemyBehaviour eb = MapManager.instance.createEnemy(tb);
                sed.enemys.Add(eb);
            }
        }
    }

    private void updateItemType2(StepTableData std)
    {

    }

}
