using System;
using UnityEngine;

[Serializable]
public struct Number
{
    public enum CustomFloatType
    {
        C = 1,
        K,
        M,
        B,
    }
    public float BaseNum;
    public CustomFloatType customFloatType;
    static readonly int minCustomFloatType = 1;
    static readonly int maxCustomFloatType = 4;
    public Number(float num)
    {
        BaseNum = num;
        customFloatType = CustomFloatType.C;
    }
    public Number(float baseNum, CustomFloatType customfloatType)
    {
        BaseNum = baseNum;
        customFloatType = customfloatType;
    }
    public Number(string info)
    {
        string[] _info = info.Split('_');
        BaseNum = Convert.ToSingle(_info[0]);
        customFloatType = (CustomFloatType)int.Parse(_info[1]);
    }
    public string ToPlayerDataStr()
    {
        Normalized();
        return BaseNum + "_" + (int)customFloatType;
    }
    public string ToShowStr()
    {
        Normalized();
        while ((int)customFloatType > minCustomFloatType)
        {
            BaseNum *= 1000;
            customFloatType--;
        }
        if (BaseNum < 100000)
        {
            return BaseNum + "";
        }
        else if (BaseNum < 10000000)
        {
            BaseNum /= 1000;
            customFloatType++;

            return BaseNum.ToString("F2") + customFloatType.ToString();
        }
        else if (BaseNum < 10000000000)
        {
            BaseNum /= 1000000;
            customFloatType += 2;
            return BaseNum.ToString("F2") + customFloatType.ToString();
        }
        else
        {
            BaseNum /= 1000000000;
            customFloatType += 3;
            return BaseNum.ToString("F2") + customFloatType.ToString();
        }
        //BaseNum = (float)Math.Round(BaseNum, 2);
        //if (customFloatType == CustomFloatType.C)
        //{
        //    return BaseNum + "";
        //}
        //return BaseNum + customFloatType.ToString();
    }
    public Number Normalized()
    {
        while (BaseNum >= 1000 && (int)customFloatType < maxCustomFloatType)
        {
            BaseNum /= 1000;
            customFloatType++;
        }
        while (BaseNum < 1f && BaseNum > 0f && (int)customFloatType > minCustomFloatType)
        {
            BaseNum *= 1000;
            customFloatType--;
        }
        return this;
    }
    public static Number operator +(Number left, Number right)
    {
        while (left.customFloatType > right.customFloatType)
        {
            right.BaseNum /= 1000;
            right.customFloatType++;
        }
        while (left.customFloatType < right.customFloatType)
        {
            left.BaseNum /= 1000;
            left.customFloatType++;
        }
        left.BaseNum += right.BaseNum;
        return left.Normalized();
    }
    public static Number operator -(Number left, Number right)
    {
        while (left.customFloatType > right.customFloatType)
        {
            right.BaseNum /= 1000;
            right.customFloatType++;
        }
        while (left.customFloatType < right.customFloatType)
        {
            left.BaseNum /= 1000;
            left.customFloatType++;
        }
        left.BaseNum -= right.BaseNum;
        return left.Normalized();
    }
    public static Number operator *(Number musicEnergy, float basenum)
    {
        musicEnergy.BaseNum *= basenum;
        return musicEnergy.Normalized();
    }
    public static Number operator *(float basenum, Number musicEnergy)
    {
        musicEnergy.BaseNum *= basenum;
        return musicEnergy.Normalized();
    }
    public static bool operator <=(Number left, Number right)
    {
        while (left.customFloatType > right.customFloatType)
        {
            right.BaseNum /= 1000;
            right.customFloatType++;
        }
        while (left.customFloatType < right.customFloatType)
        {
            left.BaseNum /= 1000;
            left.customFloatType++;
        }
        return left.BaseNum <= right.BaseNum;
    }
    public static bool operator >=(Number left, Number right)
    {
        while (left.customFloatType > right.customFloatType)
        {
            right.BaseNum /= 1000;
            right.customFloatType++;
        }
        while (left.customFloatType < right.customFloatType)
        {
            left.BaseNum /= 1000;
            left.customFloatType++;
        }
        return left.BaseNum >= right.BaseNum;
    }
    public static bool operator <(Number left, Number right)
    {
        while (left.customFloatType > right.customFloatType)
        {
            right.BaseNum /= 1000;
            right.customFloatType++;
        }
        while (left.customFloatType < right.customFloatType)
        {
            left.BaseNum /= 1000;
            left.customFloatType++;
        }
        return left.BaseNum < right.BaseNum;
    }
    public static bool operator >(Number left, Number right)
    {
        while (left.customFloatType > right.customFloatType)
        {
            right.BaseNum /= 1000;
            right.customFloatType++;
        }
        while (left.customFloatType < right.customFloatType)
        {
            left.BaseNum /= 1000;
            left.customFloatType++;

        }
        return left.BaseNum > right.BaseNum;
    }
}