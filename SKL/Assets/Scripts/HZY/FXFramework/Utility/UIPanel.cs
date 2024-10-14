using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace FXFramework
{
    public class UIPanel : MonoBehaviour
    {
        private Dictionary<string, List<UIBehaviour>> mControls;

        protected bool isOpen;

        public bool IsOpen => isOpen;

        protected virtual void Awake()
        {
            mControls = new Dictionary<string, List<UIBehaviour>>();

            FindChildrenControl<Button>((name, control) => control.onClick.AddListener(() => OnClick(name)));
            FindChildrenControl<Toggle>((name, control) => control.onValueChanged.AddListener(isSelect => OnValueChanged(name, isSelect)));
            FindChildrenControl<Slider>((name, control) => control.onValueChanged.AddListener(arg => OnValueChanged(name, arg)));

            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<RectMask2D>();
            /// .......
        }

        private void OnEnable()
        {
            isOpen = true;
        }

        private void OnDisable()
        {
            isOpen = false;
        }
        protected virtual void OnClick(string name)
        {

        }

        protected virtual void OnValueChanged(string name, bool value)
        {

        }

        protected virtual void OnValueChanged(string name,float value)
        {

        }

        protected T GetControl<T>(string name) where T : UIBehaviour
        {
            if (mControls.TryGetValue(name, out var controls))
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i] is T) return controls[i] as T;
                }
            }
            return null;
        }

        protected void FindChildrenControl<T>(Action<string, T> callback = null) where T : UIBehaviour
        {
            // 注意这里是带s的
            T[] controls = GetComponentsInChildren<T>();
            for (int i = 0; i < controls.Length; i++)
            {
                T control = controls[i];
                string name = control.gameObject.name;

                callback?.Invoke(name, control);

                if (mControls.ContainsKey(name))
                {
                    mControls[name].Add(control);
                }
                else
                {
                    mControls.Add(name, new List<UIBehaviour>() { control });
                }
            }
        }
    }
}