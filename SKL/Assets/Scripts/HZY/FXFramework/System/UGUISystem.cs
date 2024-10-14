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
            // 创建画布对象 
            var o = new GameObject("Canvas", typeof(Canvas), typeof(GraphicRaycaster));
            // 设置 UI 层
            o.layer = LayerMask.NameToLayer("UI");
            // 缓存画布的变换组件
            mCanvas = o.transform as RectTransform;
            // 设置 Canvas RenderMode 
            o.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            // 设置 CanvasScaler
            mScaler = o.AddComponent<CanvasScaler>();
            mScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            mScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            mScaler.referenceResolution = new Vector2(750, 1334);

            // 创建多个层级 从下至上依次排序
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

            // 保护画布不被销毁
            GameObject.DontDestroyOnLoad(o);

            // 创建一个事件系统的对象 挂载一个事件系统组件
            o = new GameObject("EventSystem", typeof(EventSystem));
            // 如果使用的是旧输入系统 挂载一个 StandaloneInputModule
            o.AddComponent<StandaloneInputModule>();
            // 如果使用的是新输入系统 挂载一个 InputSystemUIInputModule
            //o.AddComponent<InputSystemUIInputModule>();
            // 保护事件系统
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
            // 如果沒有从开启面板中获取到对应的面板 无需关闭或销毁
            if (!mOpenPanels.TryGetValue(name, out UIPanel panel)) return;
            if (isDestroy)
            {
                GameObject.Destroy(panel.gameObject);
                mOpenPanels.Remove(name);
            }
            // 如果不需要销毁的话 肯定是隐藏对吧 如果当前的面板已经隐藏了 不需要隐藏
            else if(panel.IsOpen)
            {
                panel.gameObject.SetActive(false);
            }
        }
        // 打开 UI 面板
        void IUGUISystem.OpenPanel<T>(E_UILayer layer, Action<T> callback)
        {
            var name = typeof(T).Name;
            // 判断一下 当前的面板是不是已经打开了 面板存在这个字典 处于激活状态 虽然存在 但是面板是影藏状态
            if (mOpenPanels.TryGetValue(name, out UIPanel panel))
            {
                // 如果面板时开启状态 我就不需要重复打开面板
                if (panel.IsOpen) return;
                panel.gameObject.SetActive(true);
                callback?.Invoke(panel as T);
                return;
            }
            // 如果池子里面没有这个面板 说明没有被加载
            ResHelper.AsyncLoad<GameObject>("Panel/" + name, o =>
             {
                 o.transform.SetParent(GetFatherLayer(layer));
                 o.transform.localPosition = Vector3.zero;
                 o.transform.localScale = Vector3.one;

                 (o.transform as RectTransform).offsetMax = Vector2.zero;
                 (o.transform as RectTransform).offsetMin = Vector2.zero;

                 T newPanel = o.GetComponent<T>();

                 callback?.Invoke(newPanel);
                 // 添加到开启列表
                 mOpenPanels.Add(name, newPanel);
             });
        }
    }
}