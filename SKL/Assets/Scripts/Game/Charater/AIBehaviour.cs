using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public DrawRange drLook;
    public DrawRange drRange;
    private AIManager aim = new AIManager();
    private EnemyBehaviour eb;

    public void inits(EnemyBehaviour e)
    {
        drLook.viewRender(false);
        drRange.viewRender(false);
        eb = e;
        aim.comms.Clear();
        aim.actions.Clear();

        if (eb.ETB.type == 1)   // 蚊子这种类型怪
        {
            setSpAttack();
        }
        else
        {
            setRestStateStandby();
        }
    }

    public void destorys()
    {
        eb = null;
        aim.comms.Clear();
        aim.actions.Clear();
        aim.curState = AIEnum.none;
    }

    private void addAction(AIAction item)
    {
        aim.actions.Enqueue(item);
    }

    private void breakComm()
    {
        aim.comms.Clear();
    }

    private void addComm(AICommand com)
    {
        aim.comms.Add(com);
    }

    private void setSpAttack()
    {
        aim.curState = AIEnum.spattack;
        addComSpAttack();
    }

    private void setRestStateStandby()
    {
        aim.curState = AIEnum.rest;
        addComStandby();
    }

    private void setRestStateRun()
    {
        aim.curState = AIEnum.rest;
        addComRun();
    }

    private void setEscape()
    {
        breakComm();

        AICommand acm = new AICommand();
        acm.comState = CommEnum.escape;
        acm.target = MapManager.instance.charManager.HB;
        acm.tarPos = eb.transform.localPosition;
        acm.tarTick = eb.ETB.range;
        addComm(acm);

    }

    private void setChase()
    {
        breakComm();

        AICommand acm = new AICommand();
        acm.comState = CommEnum.chase;
        acm.target = MapManager.instance.charManager.HB;
        acm.tarPos = eb.transform.localPosition;
        acm.tarTick = eb.ETB.range;
        addComm(acm);
    }

    private void addComSpAttack()
    {
        AICommand com = new AICommand();
        com.comState = CommEnum.spattack;
        com.target = MapManager.instance.charManager.HB;
        com.tarTick = 0f;
        addComm(com);
    }

    private void addComStandby()
    {
        AICommand com = new AICommand();
        com.comState = CommEnum.standby;
        com.tarTick = eb.randStandTime();
        addComm(com);
        eb.setAni(CharaterStates.standby);
    }

    private void addComRun()
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
        if (BattleHeroUI.isBattlePause)
        {
            return;
        }

        updateAI();
    }

    private void updateAI()
    {
        if (eb == null)
        {
            return;
        }

        drLook.drawCircleRender(transform.position, eb.ETB.radius, Color.red);
        //drRange.drawCircleRender(transform.position, eb.ETB.range, Color.blue);

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
    }

    private void updateRest()
    {
        if (Vector3.Distance(MapManager.instance.getHB().transform.localPosition, eb.transform.localPosition) > eb.ETB.radius)
        {
            return;
        }

        aim.curState = (MapManager.instance.getHB().cbInfo.hp >= eb.cbInfo.hp) ? AIEnum.escape : AIEnum.chase;
        if (aim.curState == AIEnum.escape)
        {
            setEscape();
            return;
        }

        if (aim.curState == AIEnum.chase)
        {
            setChase();
            return;
        }
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

        if (com.comState == CommEnum.chase)
        {
            updateCommChase(com);
            return;
        }

        if (com.comState == CommEnum.escape)
        {
            updateCommEscape(com);
            return;
        }

        if (com.comState == CommEnum.spattack)
        {
            updateCommSpAttack(com);
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

    private void updateCommChase(AICommand com)
    {
        if (eb.cbInfo.hp <= com.target.cbInfo.hp || Vector3.Distance(com.tarPos, eb.transform.localPosition) >= com.tarTick)
        {
            com.bComplete = true;
            setRestStateRun();
            return;
        }

        Vector3 tarPos = com.target.transform.localPosition;

        AIAction act = new AIAction();
        act.id = ((int)CommEnum.run).ToString();
        act.content = tarPos.x + "_" + tarPos.y + "_" + tarPos.z;
        addAction(act);
    }

    private void updateCommEscape(AICommand com)
    {
        if (eb.cbInfo.hp > com.target.cbInfo.hp || Vector3.Distance(com.tarPos, eb.transform.localPosition) >= com.tarTick)
        {
            com.bComplete = true;
            setRestStateRun();
            return;
        }

        Vector3 ebPos = eb.transform.localPosition;
        Vector3 tarPos = com.target.transform.localPosition;
        float dis2 = Vector3.Distance(tarPos, ebPos) * 2;

        tarPos.x += (tarPos.x >= ebPos.x) ? -dis2 : dis2;
        tarPos.z += (tarPos.z >= ebPos.z) ? -dis2 : dis2;

        AIAction act = new AIAction();
        act.id = ((int)CommEnum.run).ToString();
        act.content = (tarPos.x) + "_" + tarPos.y + "_" + (tarPos.z);
        addAction(act);
    }

    private void updateCommSpAttack(AICommand com)
    {
        if (com.state == 0)
        {
            updateComSpAttack0(com);
            return;
        }

        if (com.state == 1)
        {
            updateComSpAttack1(com);
            return;
        }
        
        if (com.state == 2)
        {
            updateComSpAttack2(com);
            return;
        }

        updateComSpAttack3(com);
    }
    private void updateComSpAttack0(AICommand com)
    {
        Vector3 tarPos = com.target.transform.localPosition;
        if (Vector3.Distance(tarPos, eb.transform.localPosition) <= eb.ETB.range)
        {
            com.tarPos = tarPos;
            com.state = 1;
            return;
        }

        AIAction act = new AIAction();
        act.id = ((int)CommEnum.run).ToString();
        act.content = (tarPos.x) + "_" + tarPos.y + "_" + (tarPos.z);
        addAction(act);
    }

    private void updateComSpAttack1(AICommand com)
    {
        Vector3 tarPos = com.tarPos;
        if (Vector3.Distance(tarPos, eb.transform.localPosition) <= eb.ETB.radius)
        {
            eb.setAni(CharaterStates.attack);
            com.tarTick = eb.getAniTime(CharaterStates.attack) * 0.7f;
            com.state = 2;
            return;
        }

        AIAction act = new AIAction();
        act.id = ((int)CommEnum.run).ToString();
        act.content = (tarPos.x) + "_" + tarPos.y + "_" + (tarPos.z);
        addAction(act);
    }

    private void updateComSpAttack2(AICommand com)
    {
        com.tarTick -= Time.deltaTime;
        if (com.tarTick > 0f)
        {
            return;
        }

        HeroBehaviour hb = (HeroBehaviour)com.target;
        if (Vector3.Distance(hb.transform.localPosition, com.tarPos) <= eb.ETB.radius)
        {
            int damage = int.Parse(eb.ETB.attack);
            hb.addHp(-damage, true);
            hb.createHpTips(-damage);
            SoundManager.instance.playSound("Hit");
        }

        com.state = 3;
        com.tarTick = 2f;
        com.tarPos = MapManager.instance.randSpEnemyPos();
    }

    private void updateComSpAttack3(AICommand com)
    {
        com.tarTick -= Time.deltaTime;
        if (com.tarTick <= 0f)
        {
            breakComm();
            MapManager.instance.stepManager.destoryEnemy(eb);
            eb.cbInfo.cstate = CharaterStates.none;
            return;
        }

        Vector3 tarPos = com.tarPos;
        AIAction act = new AIAction();
        act.id = ((int)CommEnum.run).ToString();
        act.content = (tarPos.x) + "_" + tarPos.y + "_" + (tarPos.z);
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
