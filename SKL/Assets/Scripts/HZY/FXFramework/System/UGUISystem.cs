using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace FXFramework
{
    public enum E_UILayer
    {
        Top,
        Mid,
        Down,
        System
    }
    public interface IUGUISystem : ISystem
    {
        RectTransform Canvas { get; }
        Vector2 CanvasResolution { get; set; }
        Transform GetFatherLayer(E_UILayer layer);

        void OpenPanel<T>(E_UILayer layer, Action<T> callback = null) where T : UIPanel;
        void HidePanel<T>(bool isDestroy = false) where T : UIPanel;
    }
    public class UGUISystem : AbstractSystem, IUGUISystem
    {
        private Dictionary<string, UIPanel> mOpenPanels;

        private RectTransform mCanvas;
        private CanvasScaler mScaler;

        private Transform mTop, mMid, mDown, mSystem;

        RectTransform IUGUISystem.Canvas => mCanvas;

        Vector2 IUGUISystem.CanvasResolution
        {
            get => mScaler.referenceResolution;
            set => mScaler.referenceResolution = value;
        }

        protected override void OnInit()
        {
            mOpenPanels = new Dictionary<string, UIPanel>();
            // ������������ 
            var o = new GameObject("Canvas", typeof(Canvas), typeof(GraphicRaycaster));
            // ���� UI ��
            o.layer = LayerMask.NameToLayer("UI");
            // ���滭���ı任���
            mCanvas = o.transform as RectTransform;
            // ���� Canvas RenderMode 
            o.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            // ���� CanvasScaler
            mScaler = o.AddComponent<CanvasScaler>();
            mScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            mScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            mScaler.referenceResolution = new Vector2(750, 1334);

            // ��������㼶 ����������������
            mSystem = new GameObject("System").transform;
            mSystem.SetParent(mCanvas);
            mSystem.localPosition = Vector2.zero;

            mDown = new GameObject("Down").transform;
            mDown.SetParent(mCanvas);
            mDown.localPosition = Vector2.zero;

            mMid = new GameObject("Mid").transform;
            mMid.SetParent(mCanvas);
            mMid.localPosition = Vector2.zero;

            mTop = new GameObject("Top").transform;
            mTop.SetParent(mCanvas);
            mTop.localPosition = Vector2.zero;

            // ����������������
            GameObject.DontDestroyOnLoad(o);

            // ����һ���¼�ϵͳ�Ķ��� ����һ���¼�ϵͳ���
            o = new GameObject("EventSystem", typeof(EventSystem));
            // ���ʹ�õ��Ǿ�����ϵͳ ����һ�� StandaloneInputModule
            o.AddComponent<StandaloneInputModule>();
            // ���ʹ�õ���������ϵͳ ����һ�� InputSystemUIInputModule
            //o.AddComponent<InputSystemUIInputModule>();
            // �����¼�ϵͳ
            GameObject.DontDestroyOnLoad(o);
        }

        public Transform GetFatherLayer(E_UILayer layer)
        {
            switch (layer)
            {
                case E_UILayer.Top: return mTop;
                case E_UILayer.Mid: return mMid;
                case E_UILayer.Down: return mDown;
                case E_UILayer.System: return mSystem;
                default: return null;
            }
        }

        void IUGUISystem.HidePanel<T>(bool isDestroy)
        {
            var name = typeof(T).Name;
            // ����]�дӿ�������л�ȡ����Ӧ����� ����رջ�����
            if (!mOpenPanels.TryGetValue(name, out UIPanel panel)) return;
            if (isDestroy)
            {
                GameObject.Destroy(panel.gameObject);
                mOpenPanels.Remove(name);
            }
            // �������Ҫ���ٵĻ� �϶������ض԰� �����ǰ������Ѿ������� ����Ҫ����
            else if(panel.IsOpen)
            {
                panel.gameObject.SetActive(false);
            }
        }
        // �� UI ���
        void IUGUISystem.OpenPanel<T>(E_UILayer layer, Action<T> callback)
        {
            var name = typeof(T).Name;
            // �ж�һ�� ��ǰ������ǲ����Ѿ����� ����������ֵ� ���ڼ���״̬ ��Ȼ���� ���������Ӱ��״̬
            if (mOpenPanels.TryGetValue(name, out UIPanel panel))
            {
                // ������ʱ����״̬ �ҾͲ���Ҫ�ظ������
                if (panel.IsOpen) return;
                panel.gameObject.SetActive(true);
                callback?.Invoke(panel as T);
                return;
            }
            // �����������û�������� ˵��û�б�����
            ResHelper.AsyncLoad<GameObject>("Panel/" + name, o =>
             {
                 o.transform.SetParent(GetFatherLayer(layer));
                 o.transform.localPosition = Vector3.zero;
                 o.transform.localScale = Vector3.one;

                 (o.transform as RectTransform).offsetMax = Vector2.zero;
                 (o.transform as RectTransform).offsetMin = Vector2.zero;

                 T newPanel = o.GetComponent<T>();

                 callback?.Invoke(newPanel);
                 // ��ӵ������б�
                 mOpenPanels.Add(name, newPanel);
             });
        }
    }
}