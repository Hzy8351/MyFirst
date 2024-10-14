using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public Transform transParent;
    private Dictionary<UIEnum, BaseUI> uiDictionary = new Dictionary<UIEnum, BaseUI>();
    private List<BaseUI> uiList2000 = new List<BaseUI>();
    private int count2000;


    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this.gameObject);

    }

    public int getLen2000()
    {
        return uiList2000.Count;
    }

    public void HideAll2000()
    {
        for (int i = 0; i < uiList2000.Count; ++i)
        {
            BaseUI ui = uiList2000[i];
            if (ui != null && ui.gameObject.activeSelf)
            {
                ui.Hide();
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < uiList2000.Count; )
        {
            BaseUI ui = uiList2000[i];
            if (ui == null || !ui.gameObject.activeSelf)
            {
                uiList2000.RemoveAt(i);
                continue;
            }

            ++i;
        }
        
        //if (count2000 != uiList2000.Count && uiList2000.Count == 0)
        //{
        //    ((ValsManager)GetUI(UIEnum.MoneyUI)).setMainView();
        //}
        count2000 = uiList2000.Count;
    }

    public static Sprite getIcon(string id)
    {
        return Resources.Load<Sprite>(ConstValue.path_icon_name + id);
    }
    public static Sprite getIcon(int id)
    {
        return Resources.Load<Sprite>(ConstValue.path_icon_name + id);
    }

    public bool IsActive(UIEnum uIEnum)
    {
        uiDictionary.TryGetValue(uIEnum, out BaseUI ui);
        if (!ui)
        {
            return false;
        }
        return ui.isActive;
    }

    public bool isHaveUI(UIEnum uIEnum)
    {
        uiDictionary.TryGetValue(uIEnum, out BaseUI ui);
        return ui != null;
    }

    public BaseUI GetUI(UIEnum uIEnum)
    {
        uiDictionary.TryGetValue(uIEnum, out BaseUI ui);
        if (ui == null)
        {
            Show(uIEnum);
            Hide(uIEnum);
            uiDictionary.TryGetValue(uIEnum, out BaseUI ui2);
            return ui2;
        }
        return ui;
    }

    public T GetUI<T>(UIEnum uIEnum) where T : BaseUI
    {
        uiDictionary.TryGetValue(uIEnum, out BaseUI result);
        if (result == null)
        {
            Show(uIEnum);
            Hide(uIEnum);
            uiDictionary.TryGetValue(uIEnum, out BaseUI result2);
            return result2 as T;
        }
        return result as T;
    }


    public BaseUI Show(UIEnum uiEnum)
    {
        BaseUI ui = _Show(uiEnum);
        if (ui == null)
        {
            return null;
        }

        ui.initCanvas();
        if (uiEnum >= UIEnum.SettingUI)
        {
            uiList2000.Add(ui);
        }
        return ui;
    }

    BaseUI _Show(UIEnum uiEnum)
    {
        uiDictionary.TryGetValue(uiEnum, out BaseUI ui);
        if (ui == null)
        {
            //Debug.Log("未找到指定展示UI，准备动态加载");
            string path = ConstValue.path_ui_root + uiEnum.ToString();
            GameObject pre = Resources.Load<GameObject>(path);
            if (pre == null)
            {
                Debug.LogError(string.Format($"UI 加载失败未找到UI：{path}"));
            }
            GameObject child = Instantiate(pre, transParent);
            child.TryGetComponent<BaseUI>(out ui);
            if (ui != null)
            {
                //Debug.Log("初始化UI" + ui.uiEnum.ToString());
                ui.RegisterBaseUI();
                uiDictionary.Add(ui.uiEnum, ui);
            }
            else
            {
                Debug.LogError(string.Format($"UI:{path}中未找到类型继承自BaseUI的组件"));
                return null;
            }
        }
        if (!ui.isActive)
        {
            ui.SyncShow();
        }
        return ui;
    }

    public void Hide(UIEnum uiEnum)
    {
        uiDictionary.TryGetValue(uiEnum, out BaseUI ui);
        if (ui == null)
        {
            //Debug.Log("隐藏ui失败，此UI不存在");
            return;
        }
        BaseUI baseUI = ui.GetComponent<BaseUI>();
        if (baseUI.isActive)
        {
            baseUI.SyncHide();
        }
    }

}