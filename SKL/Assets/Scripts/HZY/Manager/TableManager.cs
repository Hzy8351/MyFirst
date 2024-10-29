using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class TableManager
{
    public TaskData dataTask = new TaskData();
    public CommonData dataCommon = new CommonData();
    public WritingData dataWriting = new WritingData();
    public MapBlockData dataMapBlock = new MapBlockData();
    public MapPartsData dataMapParts = new MapPartsData();
    public MapItemData dataMapItem = new MapItemData();
    public StageData dataStage = new StageData();
    public EnemyData dataEnemy = new EnemyData();

    public void Init()
    {
        dataTask.Init(TbTool.Read("TaskTb"));
        dataCommon.Init(TbTool.Read("CommonTb"));
        dataWriting.Init(TbTool.Read("WritingTb"));
        dataMapBlock.Init(TbTool.Read("MapBlockTb"));
        dataMapParts.Init(TbTool.Read("MapPartsTb"));
        dataMapItem.Init(TbTool.Read("MapItemTb"));
        dataStage.Init(TbTool.Read("StageTb"));
        dataEnemy.Init(TbTool.Read("EnemyTb"));
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

    public void Split(string src, string s1, ref int[] ret)
    {
        string [] arr = src.Split(s1);
        ret = new int[arr.Length];
        for (int i = 0; i < arr.Length; ++i)
        {
            ret[i] = int.Parse(arr[i]);
        }
    }

}
