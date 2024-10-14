using System;
using System.Collections.Generic;
using UnityEngine;

namespace FXFramework
{
    /// <summary>
    /// ���ͬһ����Ϸ���� ������ͬ����������
    /// </summary>
    public class ComponentPool<T> where T : Behaviour
    {
        /// <summary>
        /// ���������
        /// </summary>
        private GameObject mRoot;
        /// <summary>
        /// ָ�����������
        /// </summary>
        private string mRootName;
        /// <summary>
        /// �洢�����Ѽ������
        /// </summary>
        private List<T> mOpenList = new List<T>();
        /// <summary>
        /// �洢����δʹ�����
        /// </summary>
        private Queue<T> mCloseList = new Queue<T>();
        /// <summary>
        /// ��ʼ��һ����Ϸ��������
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
        /// ���������Ѽ����������ͬ����
        /// </summary>
        /// <param name="callBack">�ص�����</param>
        public void SetAllEnabledComponent(Action<T> callBack)
        {
            foreach (T component in mOpenList) callBack(component);
        }
        /// <summary>
        /// ��ȡһ����ʹ�����
        /// </summary>
        public void Get(out T component)
        {
            //����ر��б��ж��� 
            if (mCloseList.Count > 0)
            {                
                component = mCloseList.Dequeue();//��ȡһ��δʹ�����
                component.enabled = true;//�������
            }
            else
            {
                //����ر��б�û�ж�����ʱ��  �����ǵ�һ��ʹ��
                if (mRoot == null)
                {
                    mRoot = new GameObject(mRootName);
                    GameObject.DontDestroyOnLoad(mRoot);
                }
                component = mRoot.AddComponent<T>();
            }
            //�Ѽ������� ���뿪���б�
            mOpenList.Add(component);
        }
        /// <summary>
        /// �Զ��������
        /// </summary>
        /// <param name="condition">��������</param>
        public void AutoPush(Func<T,bool> condition)
        {
            // ���ܿ����б��ж������Ի��� ������� ������Щ������������� �Ͱ������յ�
            for (int i = mOpenList.Count - 1; i >= 0; i--)
            {
                //���Ϊtrue �ͻ������
                if (condition(mOpenList[i]))
                {
                    mOpenList[i].enabled = false;
                    mCloseList.Enqueue(mOpenList[i]);
                    mOpenList.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// ���յ������
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