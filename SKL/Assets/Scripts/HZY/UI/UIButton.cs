using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : UnityEngine.UI.Button
{
    public bool scaleEnable = true;
    Vector3 m_startScale;
    Tween tween;
    private string m_soundName = "";

    protected override void Start()
    {
        base.Start();
        m_startScale = transform.localScale;
        transition = Transition.None;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }

    private void OnClickTweenDown()
    {
        if (!scaleEnable)
        {
            return;
        }
        if (tween != null)
        {
            tween.Kill();
            tween = null;
        }
        Vector3 target = m_startScale * 0.8f;
        tween = transform.DOScale(target, 0.1f).Play();
    }

    private void OnClickTweenUp()
    {
        if (!scaleEnable)
        {
            return;
        }
        if (tween != null)
        {
            tween.Kill();
            tween = null;
        }
        // Vector3 target = m_startScale * 0.8f;
        // transform.localScale = target;
        tween = transform.DOScale(m_startScale, 0.1f).Play();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        // MLog.Log("OnPointerDown");
        OnClickTweenDown();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        // MLog.Log("OnPointerExit");
        OnClickTweenUp();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        // MLog.Log("OnPointerUp");
        OnClickTweenUp();

        // ≤•∑≈“Ù–ß
        if (m_soundName != null || m_soundName.Length != 0)
        {
            SoundManager.instance.playSound(m_soundName);
        }
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        onClick.Invoke();
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        Press();
        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }

    public void LockBtn()
    {
        scaleEnable = false;
        interactable = false;
    }
    public void UnlockBtn(bool activeScale = true)
    {
        scaleEnable = activeScale;
        interactable = true;
    }

    public void AddUniqueClickListerer(UnityEngine.Events.UnityAction call, string soundName = "Click")
    {
        onClick.RemoveAllListeners();
        onClick.AddListener(call);
        m_soundName = soundName;
    }
}