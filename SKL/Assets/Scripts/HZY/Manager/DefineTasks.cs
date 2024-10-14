using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TaskInfo
{
    public int id;
    public int type;
    public int nid;
    public long proceed;

    public string describe;
    public string[] condition;
    public string[] jump;
    public List<string[]> reward;

    public void Init(TaskTb cfg, TableManager CM)
    {
        proceed = 0;
        id = cfg.id;
        type = cfg.Type;

        nid = cfg.New_Task;
        describe = cfg.Describe;
        condition = CM.Split(cfg.Condition, "_");
        jump = CM.Split(cfg.Jump, "_");
        reward = CM.Split(cfg.Reward, ";", "_");
    }

    public bool checkComplete()
    {
        return proceed >= long.Parse(condition[1]);
    }

    public float getTaskProgress()
    {
        float val = proceed / float.Parse(condition[1]);
        return (val >= 1f) ? 1f : val;
    }
}

public class TaskManager
{
    // 当前正在进行的任务列表(该游戏只有一个任务在进行，所以tasks里面count = 1)
    public List<TaskInfo> tasks = new List<TaskInfo>();

    // 完成的任务
    public List<int> taskCompletes = new List<int>();

    public bool onComplete(TaskInfo ti)
    {
        if (!ti.checkComplete())
        {
            return false;
        }

        //List<string> cfgs = new List<string>();
        for (int i = 0; i < ti.reward.Count; ++i)
        {
            string[] arr = ti.reward[i];
            GameManager.instance.addVal("完成任务获得", arr[0], arr[1]);
            if (arr[0] == "1")
            {
                GameManager.instance.addVal("完成任务获得流水", "10", arr[1]);
            }

            //if (int.Parse(arr[0]) < GameManager.instance.getValArrLen())
            //{
            //    cfgs.Add(arr[0] + "_" + arr[1] + "_0");
            //}
        }
        //((RewardUI)UIManager.instance.Show(UIEnum.RewardUI)).initViews(cfgs);

        endCurrentTask(ti);
        nextTask(ti.nid);
        return true;
    }

    public void endCurrentTask(TaskInfo ti)
    {
        if (tasks.Contains(ti)) { tasks.Remove(ti); }
        if (!taskCompletes.Contains(ti.id)) { taskCompletes.Add(ti.id); }

        //GameManager.instance.taskReport(ti, 1);
       //MapManager.instance.HM.endGuideOn();
    }

    public bool nextTask(int nid)
    {
        bool bRet = true;
        TaskTb cfg = GameManager.instance.CM.dataTask.getItem(nid);
        if (cfg == null || cfg == GameManager.instance.CM.dataTask.cfg[GameManager.instance.CM.dataTask.cfg.Count - 1])
        {
            cfg = GameManager.instance.CM.dataTask.cfg[GameManager.instance.CM.dataTask.cfg.Count - 1];
            bRet = false;
        }

        TaskInfo nti = new TaskInfo();
        nti.Init(cfg, GameManager.instance.CM);
        tasks.Add(nti);

        //GameManager.instance.taskReport(nti, 0);
        onTask(nti.type, int.Parse(nti.condition[0]), 0);
        //GameManager.instance.taskGuide(nti);
        return bRet;
    }

    // type是任务表的类型, 参数id 要么是某表的id， 要么是某表的type
    public void onTask(int type, int id, long val)
    {
        for (int i = 0; i < tasks.Count; ++i)
        {
            TaskInfo ti = tasks[i];
            if (ti.type == type)
            {
                onTask(ti, id, val);
            }
        }
    }

    private void onTask(TaskInfo ti, int id, long val)
    {
        if (ti.type == 1) { onT1(ti, id, val); return; }
        if (ti.type == 2) { onT2(ti, id, val); return; }
        if (ti.type == 3) { onT3(ti, id, val); return; }
        if (ti.type == 4) { onT4(ti, id, val); return; }
        if (ti.type == 5) { onT5(ti, id, val); return; }
        if (ti.type == 6) { onT6(ti, id, val); return; }
        if (ti.type == 7) { onT7(ti, id, val); return; }
        if (ti.type == 8) { onT8(ti, id, val); return; }
        if (ti.type == 9) { onT9(ti, id, val); return; }
        if (ti.type == 10) { onT10(ti, id, val); return; }
        if (ti.type == 11) { onT11(ti, id, val); return; }
        if (ti.type == 12) { onT12(ti, id, val); return; }
        if (ti.type == 13) { onT13(ti, id, val); return; }
        if (ti.type == 14) { onT14(ti, id, val); return; }
        if (ti.type == 15) { onT15(ti, id, val); return; }
        if (ti.type == 16) { onT16(ti, id, val); return; }
    }

    // 某设施达到多少级
    private void onTypeShopSceneLv(TaskInfo ti, int id, long val)
    {
        if (id != int.Parse(ti.condition[0]))
        {
            return;
        }

        long max = long.Parse(ti.condition[1]);

        int lv = 0;//GameManager.instance.getShopSceneLv(id);
        if (lv > 0 && lv >= max)
        {
            ti.proceed = max;
        }
    }

    // 进度累加(id相等)
    private void onTypeProceedAddWithId(TaskInfo ti, int id, long val)
    {
        int c0 = int.Parse(ti.condition[0]);
        if (id != c0)
        {
            return;
        }

        ti.proceed += val;
    }

    // 进度累加(id相等，或者id为0)
    private void onTypeProceedAddWithIdOr0(TaskInfo ti, int id, long val)
    {
        int c0 = int.Parse(ti.condition[0]);
        if (c0 != 0 && id != c0)
        {
            return;
        }

        ti.proceed += val;
    }

    // 进度赋值(id相等, 或者id为0)
    private void onTypeProceedSetWithIdOr0(TaskInfo ti, int id, long val)
    {
        int c0 = int.Parse(ti.condition[0]);
        if (c0 != 0 && id != c0)
        {
            return;
        }

        ti.proceed = val;
    }

    private void onT1(TaskInfo ti, int id, long val)
    {
        onTypeShopSceneLv(ti, id, val);
    }

    private void onT2(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithIdOr0(ti, id, val);
    }

    private void onT3(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithId(ti, id, val);
    }

    private void onT4(TaskInfo ti, int id, long val)
    {
        onTypeShopSceneLv(ti, id, val);
    }

    private void onT5(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithIdOr0(ti, id, val);
    }

    private void onT6(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithId(ti, id, val);
    }

    private void onT7(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithIdOr0(ti, id, val);
    }

    private void onT8(TaskInfo ti, int id, long val)
    {
        if (id != int.Parse(ti.condition[0]))
        {
            return;
        }

        long max = long.Parse(ti.condition[1]);

        int lv = 0;//GameManager.instance.getFishLv(id.ToString());
        if (lv > 0 && lv >= max)
        {
            ti.proceed = max;
            return;
        }

        ti.proceed = lv;
    }


    private void onT9(TaskInfo ti, int id, long val)
    {
        if (id != int.Parse(ti.condition[0]))
        {
            return;
        }

        long max = long.Parse(ti.condition[1]);

        int lv = 0;// GameManager.instance.getStaffLv(id);
        if (lv > 0 && lv >= max)
        {
            ti.proceed = max;
            return;
        }

        ti.proceed = lv;
    }

    private void onT10(TaskInfo ti, int id, long val)
    {
        onTypeProceedSetWithIdOr0(ti, id, val);
    }

    private void onT11(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithIdOr0(ti, id, val);
    }

    private void onT12(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithIdOr0(ti, id, val);
    }

    private void onT13(TaskInfo ti, int id, long val)
    {
        onTypeProceedAddWithIdOr0(ti, id, val);
    }

    private void onT14(TaskInfo ti, int id, long val)
    {
        if (id != int.Parse(ti.condition[0]))
        {
            return;
        }

        int max = int.Parse(ti.condition[1]);

        int count = 0;// GameManager.instance.getFishUnlockCount();
        if (count > 0 && count >= max)
        {
            ti.proceed = max;
            return;
        }

        ti.proceed = count;
    }

    private void onT15(TaskInfo ti, int id, long val)
    {

    }

    private void onT16(TaskInfo ti, int id, long val)
    {
        if (id != int.Parse(ti.condition[0]))
        {
            return;
        }

        long max = long.Parse(ti.condition[1]);

        long count = GameManager.instance.getVal(10);
        if (count > 0 && count >= max)
        {
            ti.proceed = max;
            return;
        }

        ti.proceed = count;
    }
}




