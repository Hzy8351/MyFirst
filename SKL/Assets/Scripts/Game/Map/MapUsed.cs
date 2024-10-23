using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUseInfo
{
    public int x;
    public int z;
}

public class MapUsed
{
    private Dictionary<string, MapUseInfo> dic = new Dictionary<string, MapUseInfo>();

    public static string getKeyStr(int x, int z)
    {
        return x + "|" + z;
    }

    public int getDicCount()
    {
        return dic.Count;
    }

    public void addDic(int x, int z)
    {
        string key = getKeyStr(x, z);
        if (!dic.ContainsKey(key))
        {
            dic.Add(key, new MapUseInfo());
        }
        dic[key].x = x;
        dic[key].z = z;
    }

    public void removeDic(int x, int z)
    {
        string key = getKeyStr(x, z);
        if (!dic.ContainsKey(key))
        {
            return;
        }
        dic.Remove(key);
    }

    public bool isContains(int x, int z)
    {
        return dic.ContainsKey(getKeyStr(x, z));
    }

    public MapUseInfo getFromDic(int x, int z)
    {
        string key = getKeyStr(x, z);
        return dic.ContainsKey(key) ? dic[key] : null;
    }

}
