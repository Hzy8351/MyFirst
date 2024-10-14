using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockTextTip : ClockBase
{
    private string content;
    private int size;
    private float dic;
    private float sec;

    public void setText(string str, int z, float d, float s)
    {
        content = str;
        size = z;
        dic = d;
        sec = s;
    }

    protected override bool onTick()
    {
        GameManager.instance.CreateTextTips(content, size, dic, sec, "f3c42b");
        return true;
    }
}

public class ClockGiveTip : ClockBase
{
    private string iname;
    private string tname;
    private string nums;
    private Vector3 pos;
    private float d;
    private float s;

    public void setData(string iconName, string textName, string num, Vector3 initPos, float dis, float sec)
    {
        iname = iconName;
        tname = textName;
        nums = num;
        pos = initPos;
        d = dis;
        s = sec;
    }

    protected override bool onTick()
    {
        GameManager.instance.CreateGiveTips(iname, tname, nums, pos, d, s);
        return true;
    }
}












