using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    private AIManager aim = new AIManager();
    private EnemyBehaviour eb;

    public void inits(EnemyBehaviour e)
    {
        eb = e;
        aim.comms.Clear();
        aim.actions.Clear();
        setRestState();
    }

    public void destorys()
    {
        eb = null;
        aim.comms.Clear();
        aim.actions.Clear();
        aim.curState = AIEnum.none;
    }

    public void breakComm()
    {
        aim.comms.Clear();
    }

    public void addAction(AIAction item)
    {
        aim.actions.Enqueue(item);
    }

    public void addComm(AICommand com)
    {
        aim.comms.Add(com);
    }

    public void setRestState()
    {
        aim.curState = AIEnum.rest;
        addComStandby();
    }

    public void addComStandby()
    {
        AICommand com = new AICommand();
        com.comState = CommEnum.standby;
        com.tarTick = eb.randStandTime();
        addComm(com);
        eb.setAni(CharaterStates.standby);
    }

    public void addComRun()
    {
        AICommand com = new AICommand();
        com.comState = CommEnum.run;
        Vector3 pos = eb.transform.localPosition;
        Vector3 rpos = new Vector3(Random.Range(pos.x - eb.ETB.range, pos.x + eb.ETB.range), 0f, Random.Range(pos.z - eb.ETB.range, pos.z + eb.ETB.range));
        com.tarPos = MapManager.instance.checkEnemyRange(rpos);
        addComm(com);
    }

    void Update()
    {
        updateAI();
    }

    private void updateAI()
    {
        if (eb == null)
        {
            return;
        }

        updateState();
        updateComms();
        updateActions();
    }

    #region states

    private void updateState()
    {
        if (aim.curState == AIEnum.rest)
        {
            updateRest();
            return;
        }

        if (aim.curState == AIEnum.escape)
        {
            updateEscape();
            return;
        }

        if (aim.curState == AIEnum.chase)
        {
            updateChase();
            return;
        }
    }

    private void updateRest()
    {
        if (Vector3.Distance(MapManager.instance.getHB().transform.localPosition, eb.transform.localPosition) <= eb.ETB.radius)
        {
            aim.curState = (MapManager.instance.getHB().cbInfo.hp >= eb.cbInfo.hp) ? AIEnum.escape : AIEnum.chase;
            return;
        }


    }

    private void updateEscape()
    {
    }

    private void updateChase()
    {
    }


    #endregion

    #region comms
    private void updateComms()
    {
        for (int i = 0; i < aim.comms.Count;)
        {
            AICommand com = aim.comms[i];
            if (com.bComplete)
            {
                aim.comms.RemoveAt(i);
                continue;
            }

            updateComm(com);
            ++i;
        }
    }

    private void updateComm(AICommand com)
    {
        if (com.comState == CommEnum.standby)
        {
            updateCommStandby(com);
            return;
        }

        if (com.comState == CommEnum.run)
        {
            updateCommRun(com);
            return;
        }
    }

    private void updateCommStandby(AICommand com)
    {
        com.tarTick -= Time.deltaTime;
        if (com.tarTick <= 0f)
        {
            com.bComplete = true;
            addComRun();
            return;
        }
        
    }

    private void updateCommRun(AICommand com)
    {
        if (Vector3.Distance(com.tarPos, eb.transform.localPosition) <= 1f)
        {
            com.bComplete = true;
            if (Random.Range(0, 2) == 0)
            {
                addComStandby();
            }
            else
            {
                addComRun();
            }
            return;
        }

        AIAction act = new AIAction();
        act.id = ((int)com.comState).ToString();
        act.content = com.tarPos.x + "_" + com.tarPos.y + "_" + com.tarPos.z;
        addAction(act);
    }

    #endregion

    #region actions 
    private void updateActions()
    {
        if (aim.actions.Count <= 0)
        {
            return;
        }

        AIAction act = aim.actions.Dequeue();
        updateAct(act);
    }

    private void updateAct(AIAction act)
    {
        if (int.Parse(act.id) == (int)CommEnum.run)
        {
            updateActRun(act);
            return;
        }


    }

    private void updateActRun(AIAction act)
    {
        string[] pos = GameManager.instance.CM.Split(act.content, "_");
        Vector3 tarPos = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        Vector3 direct = tarPos - eb.transform.localPosition;
        direct.Normalize();
        eb.moveTo(direct.x, direct.z);
    }


    #endregion
}
