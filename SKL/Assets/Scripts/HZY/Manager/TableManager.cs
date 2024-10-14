using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class TableManager
{
    public TaskData dataTask = new TaskData();
    public CommonData dataCommon = new CommonData();
    public WritingData dataWriting = new WritingData();

    public void Init()
    {
        dataTask.Init(TbTool.Read("TaskTb"));
        dataCommon.Init(TbTool.Read("CommonTb"));
        dataWriting.Init(TbTool.Read("WritingTb"));
    }

    public SplitData ParseString(string a0, string a1)
    {
        SplitData sd = new SplitData();
        int id = int.Parse(a0);
        if (id >= 20000 && id < 60000)
        {
            CommonTb cfg = dataCommon.getItem(id);
            string[] arr = Split(cfg.Configuration, "_");
            sd.time = long.Parse(arr[0]);
            sd.id = int.Parse(arr[1]);
            sd.val = long.Parse(a1);
        }
        else if (id >= 60000 && id < 100000)
        {
            CommonTb cfg = dataCommon.getItem(id);
            string[] arr = Split(cfg.Configuration, "_");
            sd.time = long.Parse(arr[0]);
            sd.id = int.Parse(arr[1]);
            sd.val = long.Parse(arr[2]);
        }
        else
        {
            sd.id = id;
            sd.val = long.Parse(a1);
        }

        return sd;
    }

    public void Split(string src, string s1, string s2, ref List<int[]> ret)
    {
        ret.Clear();
        string[] arr = src.Split(s1);
        for (int i = 0; i < arr.Length; ++i)
        {
            string[] arr2 = arr[i].Split(s2);
            int[] irr = new int[arr2.Length];
            for (int k = 0; k < irr.Length; ++k)
            {
                irr[k] = int.Parse(arr2[k]);
            }
            ret.Add(irr);
        }
    }

    public List<string[]> Split(string src, string s1, string s2)
    {
        List<string[]> ret = new List<string[]>();
        string[] arr = src.Split(s1);
        for (int i = 0; i < arr.Length; ++i)
        {
            ret.Add(arr[i].Split(s2));
        }
        return ret;
    }

    public void Split(string src, string s1, string s2, ref Dictionary<string, string[]> ret)
    {
        ret.Clear();
        string[] arr = src.Split(s1);
        for (int i = 0; i < arr.Length; ++i)
        {
            string[] a2 = arr[i].Split(s2);
            if (!ret.ContainsKey(a2[0]))
            {
                ret.Add(a2[0], a2);
            }
        }
    }

    public string[] Split(string src, string s1)
    {
        return src.Split(s1);
    }

}
