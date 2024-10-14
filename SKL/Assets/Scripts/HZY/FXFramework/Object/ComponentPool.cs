using System;
using System.Collections.Generic;
using UnityEngine;

namespace FXFramework
{
    /// <summary>
    /// 针对同一个游戏对象 挂载相同组件的组件池
    /// </summary>
    public class ComponentPool<T> where T : Behaviour
    {
        /// <summary>
        /// 组件根对象
        /// </summary>
        private GameObject mRoot;
        /// <summary>
        /// 指定组件的名字
        /// </summary>
        private string mRootName;
        /// <summary>
        /// 存储所有已激活组件
        /// </summary>
        private List<T> mOpenList = new List<T>();
        /// <summary>
        /// 存储所有未使用组件
        /// </summary>
        private Queue<T> mCloseList = new Queue<T>();
        /// <summary>
        /// 初始化一个游戏对象用于
        /// </summary>
        public ComponentPool(string rootObjName)
        {
            mRootName = rootObjName;
        }
        public void Clear()
        {
            mOpenList.Clear();
            mCloseList.Clear();
            GameObject.Destroy(mRoot);
            mRoot = null;
        }
        /// <summary>
        /// 设置所有已激活组件的相同参数
        /// </summary>
        /// <param name="callBack">回调函数</param>
        public void SetAllEnabledComponent(Action<T> callBack)
        {
            foreach (T component in mOpenList) callBack(component);
        }
        /// <summary>
        /// 获取一个可使用组件
        /// </summary>
        public void Get(out T component)
        {
            //如果关闭列表有东西 
            if (mCloseList.Count > 0)
            {                
                component = mCloseList.Dequeue();//获取一个未使用组件
                component.enabled = true;//激活组件
            }
            else
            {
                //如果关闭列表没有东西的时候  可能是第一次使用
                if (mRoot == null)
                {
                    mRoot = new GameObject(mRootName);
                    GameObject.DontDestroyOnLoad(mRoot);
                }
                component = mRoot.AddComponent<T>();
            }
            //把激活的组件 放入开启列表
            mOpenList.Add(component);
        }
        /// <summary>
        /// 自动回收组件
        /// </summary>
        /// <param name="condition">回收条件</param>
        public void AutoPush(Func<T,bool> condition)
        {
            // 可能开启列表有东西可以回收 逆向遍历 看看那些满足条件的组件 就把他回收掉
            for (int i = mOpenList.Count - 1; i >= 0; i--)
            {
                //如果为true 就回收组件
                if (condition(mOpenList[i]))
                {
                    mOpenList[i].enabled = false;
                    mCloseList.Enqueue(mOpenList[i]);
                    mOpenList.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 回收单个组件
        /// </summary>
        public void Push(T component, Action callBack = null)
        {
            if (mOpenList.Contains(component))
            {
                callBack?.Invoke();                
                component.enabled = false;
                mOpenList.Remove(component);
                mCloseList.Enqueue(component);                
            }
        }
    }
}