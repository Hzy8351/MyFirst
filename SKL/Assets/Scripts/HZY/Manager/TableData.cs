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
    public string Configuration;
    public string Text;
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

    public string getConfiguration(int id)
    {
        return getItem(id).Configuration;
    }

    public string getText(int id)
    {
        return getItem(id).Text;
    }

    public string[] getSplitsConfig(int id, string sp)
    {
        return GameManager.instance.CM.Split(getItem(id).Configuration, sp);
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
    public int Map;
    public string Sprite;
    public string Center;
    public string Size;
    public int damage;
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
            if (!dic.ContainsKey(cfg[i].Map))
            {
                dic.Add(cfg[i].id, new List<MapBlockTb>());
            }
            dic[cfg[i].Map].Add(cfg[i]);
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

#region MapItemTb
public class MapItemTb
{
    public int id;
    public string Sprite;
    public int type;
}

public class MapItemData
{
    public List<MapItemTb> cfg;

    public void Init(string str)
    {
        cfg = JsonMapper.ToObject<List<MapItemTb>>(str);
    }

    public MapItemTb getItem(int id)
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




