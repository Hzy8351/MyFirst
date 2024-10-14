using System.Collections.Generic;
using UnityEngine;


// �˳���Ϸ��Ҫ�洢������(������)
public class MainData
{
    public static int lmax = 11;
    //vals[0] δʹ��
    //vals[1] ���ֵ

    public long[] vals = new long[lmax];

    // ������Ч
    public bool musicState = true;
    public bool soundState = true;
}

// ��ʱ�����࣬����д����̳У�ʵ��onTick() return true(1��)  return false ����ѭ��
public class ClockBase
{
    private float tick;          // ��ǰʱ�����
    private float tickDesc;      // ������֮��
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





















