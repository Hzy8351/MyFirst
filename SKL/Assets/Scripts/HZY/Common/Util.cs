using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Util
{
    // 求指定范围内的一个随机数
    public static int randomInt(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    public static float randomFloat(float min,float max)
    {
        return Random.Range(min, max);
    }

    //根据概率获得结果，例：1,100,60（60是爆率）
    public static bool getResultRandom(float probability)
    {
        int random = Util.randomInt(1, 100);
        bool result = probability >= random ? true : false;
        return result;
    }
    /**
    * describe: 根据权重来随机
    * 从一个数组中进行随机选择元素, 需要其元素为一个obj类型, 包含名为weight的key
    * 返回下标
    * @param array 
    */
    public static int randByWeight(List<(int id, int weight)> list)
    {
        float totalWeight = 0;
        int randIndex = -1;
        foreach (var element in list)
        {
            totalWeight += element.weight;
        }

        if (totalWeight <= 0) {
            return randIndex;
        }
        else
        {
            float randVal = Util.randomFloat(1, totalWeight);
            for (int index = 0; index < list.Count; index++)
            {
                var element = list[index];
                if (randVal <= element.weight) 
                {
                    randIndex = index;
                    break;
                }
                else 
                {
                    randVal -= element.weight;
                }
            }
        }
        return randIndex;
    }
    /**
    * describe: 根据权重来随机
    * 从一个数组中进行随机选择元素, 需要其元素为一个obj类型, 包含名为weight的key
    * 返回下标
    * @param array 
    */
    public static int randByWeight(List<(int id, float time, int weight)> list)
    {
        float totalWeight = 0;
        int randIndex = -1;
        foreach (var element in list)
        {
            totalWeight += element.weight;
        }

        if (totalWeight <= 0) {
            return randIndex;
        }
        else
        {
            float randVal = Util.randomFloat(1, totalWeight);
            for (int index = 0; index < list.Count; index++)
            {
                var element = list[index];
                if (randVal <= element.weight) 
                {
                    randIndex = index;
                    break;
                }
                else 
                {
                    randVal -= element.weight;
                }
            }
        }
        return randIndex;
    }


    /**打乱数组排序 */
    public static void shuffle<T>(List<T> arr)
    {
        for (int i = 0; i < arr.Count; i++)
        { 
            var index = Random.Range(i, arr.Count);
            var tmp = arr[i];
            var ran = arr[index];
            arr[i] = ran;
            arr[index] = tmp;
        }
    }
    /**计算2个时间相差天数*/
    public static int getTimeSubDay(DateTime d1, DateTime d2)
    {
        DateTime d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d1.Year, d1.Month, d1.Day));
        DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));
        int days = (d4 - d3).Days;
        return days;
    }

    public static string GetStringWithNewLine(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        return input.Replace('@'.ToString(), Environment.NewLine);
    }

    public static Rect InitRect(Vector2 center, Vector2 size)
    {
        Rect rect = new Rect(center - size / 2, size);
        return rect;
    }
    public static void DrawRect(Rect rect, Color color)
    {
        #if UNITY_EDITOR
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y ),color);
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x , rect.y + rect.height), color);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), color);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), color);
        #endif
    }

    #region 大额数值转换Numdispose(float tempNum_, int digits = 2)
    /// <summary>
    /// 大额数值转换Numdispose(float tempNum_, int digits = 2)
    /// </summary>
    /// <param name="tempNum_">被转换的数据对象</param>
    /// <param name="digits">保留位数</param>
    /// <returns></returns>
    public static string Numdispose(float tempNum_, int digits = 2)
    {
        string num = "";
        string[] symbol = { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad" };
        float tempNum = tempNum_;
        long v = 1000;
        int unitIndex = 0;
        while (tempNum >= v)
        {
            unitIndex++;
            tempNum /= v;
        }
        if (unitIndex >= symbol.Length)
        {
            num = tempNum_.ToString();
        }
        else
        {
            tempNum = Round(tempNum, digits);
            num = $"{tempNum}{symbol[unitIndex]}";
        }
        return num;
    }
    public static float Round(float value, int digits)
    {
        float multiple = Mathf.Pow(10, digits);
        float tempValue = value * multiple + 0.5f;
        tempValue = Mathf.FloorToInt(tempValue);
        return tempValue / multiple;
    }
    #endregion

    #region GetChinessName 随机获取三个字的名字
    /// <summary>
    /// 随机获取三个字的名字
    /// </summary>
    /// <returns></returns>
    public static string GetChinessName()
    {
        string name = "";
        string[] _crabofirstName = new string[]{
            "白","毕","卞","蔡","曹","岑","常","车","陈","成" ,"程","池","邓","丁","范","方","樊","闫","倪","周",
            "费","冯","符","元","袁","岳","云","曾","詹","张","章","赵","郑" ,"钟","周","邹","朱","褚","庄","卓"
           ,"傅","甘","高","葛","龚","古","关","郭","韩","何" ,"贺","洪","侯","胡","华","黄","霍","姬","简","江"
           ,"姜","蒋","金","康","柯","孔","赖","郎","乐","雷" ,"黎","李","连","廉","梁","廖","林","凌","刘","柳"
           ,"龙","卢","鲁","陆","路","吕","罗","骆","马","梅" ,"孟","莫","母","穆","倪","宁","欧","区","潘","彭"
           ,"蒲","皮","齐","戚","钱","强","秦","丘","邱","饶" ,"任","沈","盛","施","石","时","史","司徒","苏","孙"
           ,"谭","汤","唐","陶","田","童","涂","王","危","韦" ,"卫","魏","温","文","翁","巫","邬","吴","伍","武"
           ,"席","夏","萧","谢","辛","邢","徐","许","薛","严" ,"颜","杨","叶","易","殷","尤","于","余","俞","虞"
           };

        string _lastName = "震南洛栩嘉光琛潇闻鹏宇斌威汉火科技梦琪忆柳之召腾飞慕青问兰尔岚元香初夏沛菡傲珊曼文乐菱痴珊恨玉惜香寒新柔语蓉海安夜蓉涵柏水桃醉蓝春语琴从彤" +
            "傲晴语菱碧彤元霜怜梦紫寒妙彤曼易南莲紫翠雨寒易烟如萱若南寻真晓亦向珊慕灵以蕊寻雁映易雪柳孤岚笑霜海云凝天沛珊寒云冰旋宛儿" +
            "绿真盼晓霜碧凡夏菡曼香若烟半梦雅绿冰蓝灵槐平安书翠翠风香巧代云梦曼幼翠友巧听寒梦柏醉易访旋亦玉凌萱访卉怀亦笑蓝春翠靖柏夜蕾" +
            "冰夏梦松书雪乐枫念薇靖雁寻春恨山从寒忆香觅波静曼凡旋以亦念露芷蕾千帅新波代真新蕾雁玉冷卉紫千琴恨天傲芙盼山怀蝶冰山柏翠萱恨松问旋" +
            "南白易问筠如霜半芹丹珍冰彤亦寒寒雁怜云寻文乐丹翠柔谷山之瑶冰露尔珍谷雪乐萱涵菡海莲傲蕾青槐洛冬易梦惜雪宛海之柔夏青妙菡春竹痴梦紫蓝晓巧幻柏" +
            "元风冰枫访蕊南春芷蕊凡蕾凡柔安蕾天荷含玉书雅琴书瑶春雁从安夏槐念芹怀萍代曼幻珊谷丝秋翠白晴海露代荷含玉书蕾听访琴灵雁秋春雪青乐瑶含烟涵双" +
            "平蝶雅蕊傲之灵薇绿春含蕾梦蓉初丹听听蓉语芙夏彤凌瑶忆翠幻灵怜菡紫南依珊妙竹访烟怜蕾映寒友绿冰萍惜霜凌香芷蕾雁卉迎梦元柏代萱紫真千青凌寒" +
            "紫安寒安怀蕊秋荷涵雁以山凡梅盼曼翠彤谷新巧冷安千萍冰烟雅友绿南松诗云飞风寄灵书芹幼蓉以蓝笑寒忆寒秋烟芷巧水香映之醉波幻莲夜山芷卉向彤小玉幼";

        name = _crabofirstName[Random.Range(0, _crabofirstName.Length - 1)] + _lastName[Random.Range(0, _lastName.Length - 1)] + _lastName[Random.Range(0, _lastName.Length - 1)];
        return name;
    }
    #endregion

    public static Vector3 GetInspectorRotationValueMethod(Transform transform)
    {
        // 获取原生值
        System.Type transformType = transform.GetType();
        PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
        object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(transform, null);
        MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
        object value = m_methodInfo_GetLocalEulerAngles.Invoke(transform, new object[] { m_OldRotationOrder });
        //Debug.Log("反射调用GetLocalEulerAngles方法获得的值：" + value.ToString());
        string temp = value.ToString();
        //将字符串第一个和最后一个去掉
        temp = temp.Remove(0, 1);
        temp = temp.Remove(temp.Length - 1, 1);
        //用‘，’号分割
        string[] tempVector3;
        tempVector3 = temp.Split(',');
        //将分割好的数据传给Vector3
        Vector3 vector3 = new Vector3(float.Parse(tempVector3[0]), float.Parse(tempVector3[1]), float.Parse(tempVector3[2]));
        return vector3;
    }
}