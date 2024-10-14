using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    public UIEnum uiEnum;
    public int order;
    public Transform root;
    protected bool isInit = false;
    protected bool UITween = true;
    protected bool isFade = false;
    Tween scaleTween;
    Canvas canvas;
    CanvasGroup canvasGroup;

    public void initCanvas()
    {
        if (canvas != null)
        {
            return;
        }

        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;

        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
    }



    protected virtual void OnShow()
    {

    }
    protected virtual void OnHide()
    {

    }
    protected virtual void OnInit()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void RegisterBaseUI()
    {
        this.gameObject.SetActive(true);
        if (!isInit)
        {
            OnInit();
            isInit = true;
        }
        this.gameObject.SetActive(false);
    }
    public void SyncData(IBaseData data)
    {
        InitData(data);
    }
    public void SyncData(string msg, Action onYes, Action onNo)
    {
        InitData(msg, onYes, onNo);
    }
    protected virtual void InitData(IBaseData data)
    {

    }
    protected virtual void InitData(string msg, Action onYes, Action onNo)
    {

    }
    public virtual void SyncShow()
    {
        this.transform.SetAsLastSibling();
        gameObject.SetActive(true);
        if (scaleTween != null)
        {
            scaleTween.Kill();
            scaleTween = null;
        }
        if (UITween)
        {
            root.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            scaleTween = root.DOScale(new Vector3(1, 1, 1), 0.2f).OnComplete(TweenShow).SetEase(Ease.InOutQuad).Play();
        }
        else
        {
            TweenShow();
            if (isFade)
            {
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, 0.5f);
            }
        }
    }
   

    protected virtual void TweenShow()
    {
        if (!isInit)
        {
            OnInit();
            isInit = true;
        }
        OnShow();
    }
    public virtual void SyncHide()
    {
        if (scaleTween != null)
        {
            scaleTween.Kill();
            scaleTween = null;
        }
        if (UITween)
        {
            scaleTween = root.DOScale(new Vector3(0, 0, 0), 0.2f).OnComplete(TweenHide).SetEase(Ease.InOutQuad).Play();
        }
        else
        {
            if (isFade)
            {
                canvasGroup.alpha = 1;
                canvasGroup.DOFade(0, 0.5f);
                Invoke("TweenHide", 0.5f);
            }
            else
            {
                TweenHide();
            }
        }

    }

    protected virtual void TweenHide()
    {
        gameObject.SetActive(false);
        OnHide();
    }

    public void Show()
    {
        UIManager.instance.Show(uiEnum);
    }

    public void Hide()
    {
        UIManager.instance.Hide(uiEnum);
    }

    public virtual bool isActive
    {
        get
        {
            return gameObject.activeSelf;
        }
    }
}