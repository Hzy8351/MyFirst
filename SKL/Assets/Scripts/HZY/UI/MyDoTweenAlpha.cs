using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDoTweenAlpha : MonoBehaviour
{
    public float apTime = 0.5f;
    public float deTime = 0.5f;
    public float apVal = 1f;
    public float deVal = 0f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            beginAlphaAni();
        }
    }

    public void beginAlphaAni()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.DOKill();
        onAP();
    }

    private void onAP()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = apVal;
        canvasGroup.DOFade(deVal, deTime);
        Invoke("onDE", deTime);
    }

    private void onDE()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = deVal;
        canvasGroup.DOFade(apVal, apTime);
        Invoke("onAP", apTime);
    }
}
