using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

#region TaskTb
public class TaskTb
{
    public int id;
    public string Describe;
    public int Type;
    public string Condition;
    public string Reward;
    public string Jump;
    public int New_Task;
}

public class TaskData
{
    public List<TaskTb> cfg;
    private Dictionary<int, TaskTb> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<TaskTb>>(str);
        dic = new Dictionary<int, TaskTb>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            dic.Add(cfg[i].id, cfg[i]);
        }
    }

    public TaskTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }
}

#endregion

#region CommonTb
public class CommonTb
{
    public int id;
    public string Name;
    public string Para1;
    public string Para2;
    public string Para3;
    public string Para4;
}

public class CommonData
{
    public List<CommonTb> cfg;
    private Dictionary<int, CommonTb> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<CommonTb>>(str);
        dic = new Dictionary<int, CommonTb>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            dic.Add(cfg[i].id, cfg[i]);
        }
    }

    public CommonTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

}

#endregion

#region WritingTb
public class WritingTb
{
    public int id;
    public string Describe;
}

public class WritingData
{
    public List<WritingTb> cfg;
    private Dictionary<int, WritingTb> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<WritingTb>>(str);
        dic = new Dictionary<int, WritingTb>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            dic.Add(cfg[i].id, cfg[i]);
        }
    }

    public WritingTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }
}

#endregion

#region MapBlockTb
public class MapBlockTb
{
    public int id;
    public int map;
    public string sprite;
    public string center;
    public string size;
}

public class MapBlockData
{
    public List<MapBlockTb> cfg;
    private Dictionary<int, List<MapBlockTb>> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<MapBlockTb>>(str);
        dic = new Dictionary<int, List<MapBlockTb>>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            if (!dic.ContainsKey(cfg[i].map))
            {
                dic.Add(cfg[i].id, new List<MapBlockTb>());
            }
            dic[cfg[i].map].Add(cfg[i]);
        }
    }

    public MapBlockTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

    public List<MapBlockTb> getBlocksOfMap(int map)
    {
        return dic.ContainsKey(map) ? dic[map] : null;
    }

}

#endregion

#region MapPartsTb
public class MapPartsTb
{
    public int id;
    public int map;
    public string sprite;
    public string center;
    public string size;
}

public class MapPartsData
{
    public List<MapPartsTb> cfg;
    private Dictionary<int, List<MapPartsTb>> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<MapPartsTb>>(str);
        dic = new Dictionary<int, List<MapPartsTb>>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            if (!dic.ContainsKey(cfg[i].map))
            {
                dic.Add(cfg[i].id, new List<MapPartsTb>());
            }
            dic[cfg[i].map].Add(cfg[i]);
        }
    }

    public MapPartsTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

    public List<MapPartsTb> getPartsOfMap(int map)
    {
        return dic.ContainsKey(map) ? dic[map] : null;
    }

}

#endregion

#region MapItemTb
public class MapItemTb
{
    public int id;
    public string sprite;
    public int type;
    public float scale;
    public int val;
}

public class MapItemData
{
    public List<MapItemTb> cfg;
    private Dictionary<int, List<MapItemTb>> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<MapItemTb>>(str);
        dic = new Dictionary<int, List<MapItemTb>>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            if (!dic.ContainsKey(cfg[i].type))
            {
                dic.Add(cfg[i].type, new List<MapItemTb>());
            }
            dic[cfg[i].type].Add(cfg[i]);
        }
    }

    public MapItemTb randItemOfType(int type)
    {
        if (!dic.ContainsKey(type))
        {
            return null;
        }

        List<MapItemTb> lists = dic[type];
        if (lists.Count <= 0)
        {
            return null;
        }

        return lists[Random.Range(0, lists.Count)];
    }

    public MapItemTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

}

#endregion

#region StageTb
public class StageTb
{
    public int id;
    public int map;
    public int stage;
    public int xgrid;
    public int zgrid;
    public string gridspirte;
    public int sidegrid;
    public string sidegridspirte;
    public int blockcount;
    public int partscount;
    public int hphero;
    public int maxscore;
    public string stepscore;
    public string itemtype1;
    public string itemtype2;
    public string enemytype1;
    public string enemytype2;
    public string enemytype3;

}

public class StageData
{
    public List<StageTb> cfg;
    private Dictionary<int, List<StageTb>> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<StageTb>>(str);
        dic = new Dictionary<int, List<StageTb>>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            if (!dic.ContainsKey(cfg[i].map))
            {
                dic.Add(cfg[i].map, new List<StageTb>());
            }
            dic[cfg[i].map].Add(cfg[i]);
        }
    }

    public StageTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

    public List<StageTb> getStagesOfMap(int map)
    {
        return dic.ContainsKey(map) ? dic[map] : null;
    }

    public StageTb getStageOfMapStage(int map, int stage)
    {
        List<StageTb> lists = getStagesOfMap(map);
        if (lists == null)
        {
            return null;
        }
        for (int i=0; i<lists.Count; ++i)
        {
            StageTb tb = lists[i];
            if (tb.stage == stage)
            {
                return tb;
            }
        }

        return null;
    }
}

#endregion

#region EnemyTb
public class EnemyTb
{
    public int id;
    public string name;
    public string spine;
    public string attack;
    public string skill;
    public int type;
    public int score;
    public float speed;
    public float radius;
    public float range;
    public string hp;
}

public class EnemyData
{
    public List<EnemyTb> cfg;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<EnemyTb>>(str);
    }

    public EnemyTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

}

#endregion

#region GuideTb

public class GuideTb
{
    public int id;
    public int Delay;
    public int Type;
    public string parameter;
    public string Text;
}

public class GuideData
{
    public List<GuideTb> cfg;
    private Dictionary<int, GuideTb> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<GuideTb>>(str);
        dic = new Dictionary<int, GuideTb>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            dic.Add(cfg[i].id, cfg[i]);
        }
    }

    public GuideTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

}

#endregion

#region GuideIDTb

public class GuideIDTb
{
    public int id;
    public string GuideTab;
    public int NewGuide;
    public string Text;
}

public class GuideIDData
{
    public List<GuideIDTb> cfg;
    private Dictionary<int, GuideIDTb> dic;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<GuideIDTb>>(str);
        dic = new Dictionary<int, GuideIDTb>();
        for (int i = 0; i < cfg.Count; ++i)
        {
            dic.Add(cfg[i].id, cfg[i]);
        }
    }

    public GuideIDTb getItem(int id)
    {
        return cfg.Find((item) => item.id == id);
    }

}

#endregion




