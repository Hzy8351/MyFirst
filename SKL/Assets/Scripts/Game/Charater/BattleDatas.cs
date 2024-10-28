using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepEnemyData
{
    public List<EnemyBehaviour> enemys = new List<EnemyBehaviour>();
}

public class StepItemData
{
    public List<GameObject> items = new List<GameObject>();
}

public class StepTableData
{
    public List<FreshData> enemyType1 = new List<FreshData>();
    public List<FreshData> enemyType2 = new List<FreshData>();
    public List<FreshData> enemyType3 = new List<FreshData>();
    public List<FreshData> itemType2 = new List<FreshData>();
}

public class FreshData
{
    public int id;
    public int para1;
    public float tick;
}

public class BattleDatas
{
    public Dictionary<int, StepTableData> dicTables = new Dictionary<int, StepTableData>();
    public Dictionary<string, StepEnemyData> dicEnemys = new  Dictionary<string, StepEnemyData>();
    public Dictionary<string, StepItemData> dicItems = new Dictionary<string, StepItemData>();

    public void inits(StageTb tb, int len)
    {
        initTableDatas(tb, len);
        initEnemyDatas();
    }

    public StepEnemyData getDicEnemy(string key)
    {
        if (!dicEnemys.ContainsKey(key))
        {
            dicEnemys.Add(key, new StepEnemyData());
        }
        return dicEnemys[key];
    }

    public StepItemData getDicItem(string key)
    {
        if (!dicItems.ContainsKey(key))
        {
            dicItems.Add(key, new StepItemData());
        }
        return dicItems[key];
    }

    private void initTableDatas(StageTb tb, int len)
    {
        dicTables.Clear();
        string[] enemytype1 = GameManager.instance.CM.Split(tb.enemytype1, "|");
        string[] enemytype2 = GameManager.instance.CM.Split(tb.enemytype2, "|");
        string[] enemytype3 = GameManager.instance.CM.Split(tb.enemytype3, "|");
        string[] itemtype2 = GameManager.instance.CM.Split(tb.itemtype2, "|");
        for (int i = 0; i < len; ++i)
        {
            StepTableData std = new StepTableData();
            setData(std.enemyType1, enemytype1[i]);
            setData(std.enemyType2, enemytype2[i]);
            setData(std.enemyType3, enemytype3[i]);
            setData(std.itemType2, itemtype2[i]);
            dicTables.Add(i + 1, std);
        }
    }

    private void initEnemyDatas()
    {
        dicEnemys.Clear();
    }

    private void setData(List<FreshData> lists, string str)
    {
        List<string[]> arr = GameManager.instance.CM.Split(str, ";", "_");
        for (int i = 0; i < arr.Count; ++i)
        {
            FreshData fd = new FreshData();
            fd.id = int.Parse(arr[i][0]);
            fd.para1 = int.Parse(arr[i][1]);
            lists.Add(fd);
        }

    }
}


