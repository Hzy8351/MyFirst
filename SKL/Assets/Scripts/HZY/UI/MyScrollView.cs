using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MyScrollView : MonoBehaviour
{
    public Transform transItem;
    public GridLayoutGroup grContent;
    public Axis startAxis;
    //public float rowDistance;               //�о�
    //public float columnDistance;            //�о�


    /// <summary>
    /// ��������scroll view�ڵ�����Ԫ�ص���ʾ��λ�õĽ��뷽��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="setElement"></param>
    public ScrollViewSetter<T> SetParam<T>(List<T> list, ScrollViewSetter<T>.SetItemData setElement, int columnCount,float scale)
    {
        ScrollViewSetter<T> baseList = new ScrollViewSetter<T>();
        baseList.SetParam(list, setElement, transform, grContent, transItem, startAxis, 1, columnCount, scale);
        return baseList;
    }
}
