using System.Collections.Generic;
using UnityEngine;


// 退出游戏需要存储的数据(主数据)
public class MainData
{
    public static int lmax = 11;
    //vals[0] 未使用
    //vals[1] 金币值

    public long[] vals = new long[lmax];

    // 音乐音效
    public bool musicState = true;
    public bool soundState = true;
}

// 计时器基类，可以写子类继承，实现onTick() return true(1次)  return false 无限循环
public class ClockBase
{
    private float tick;          // 当前时间进度
    private float tickDesc;      // 多少秒之后
    private float tickBuff;
    private float tickBuff2;

    public void InitTick(float sec)
    {
        setTickDesc(sec);
        resetTick();
    }

    public void setTickDesc(float sec)
    {
        tickDesc = sec;
    }

    public void setTickBuf2(float t)
    {
        tickBuff2 = t;
    }

    public void setTickBuf()
    {
        tickBuff = getLeftTick() / 40f;
    }

    public bool isTickBuff()
    {
        return tickBuff > 0f;
    }

    public void resetTick()
    {
        tick = 0f;
        tickBuff = 0f;
        tickBuff2 = 1f;
    }

    public void setTickT(float t)
    {
        tick = t;
    }

    public void setTickMax()
    {
        tick = tickDesc;
    }

    public void setTick0()
    {
        tick = 0;
    }

    public float getLeftTick()
    {
        return tickDesc - tick;
    }

    public float getDescTick()
    {
        return tickDesc;
    }
    
    public float getTick()
    {
        return tick;
    }

    public bool updateTick()
    {
        tick += Time.deltaTime * tickBuff2 + tickBuff;
        if (tick >= tickDesc)
        {
            tick = 0f;
            return onTick();
        }

        return false;
    }

    protected virtual bool onTick(){ return true; }
}

public class SplitData
{
    public int id;
    public long val;
    public long time;
}

public class GameFormula
{
    public static float func1(float org, float buf, float x, float y)
    {
        return org - buf / (buf + buf * x + y) * org;
    }

    public static long func2(long org, float buf)
    {
        return (long)(org * (1 + buf));
    }
}





















