using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDoTweenScale : MonoBehaviour
{
    private RectTransform rtTrans;
    private ClockBase aniScale;

    public void beginAniScale(float tick)
    {
        if (aniScale != null)
        {
            return;
        }

        if (rtTrans == null)
        {
            rtTrans = gameObject.GetComponent<RectTransform>();
        }

        if (tick < 1f)
        {
            tick = 1f;
        }

        aniScale = new ClockBase();
        aniScale.InitTick(tick);
    }

    public void endAniScale()
    {
        if (aniScale == null)
        {
            return;
        }
        aniScale = null;

        if (rtTrans != null)
        {
            rtTrans.localScale = Vector3.one;
        }
    }

    private void updateAniScale()
    {
        if (rtTrans == null)
        {
            return;
        }

        if (aniScale == null)
        {
            return;
        }

        if (!aniScale.updateTick())
        {
            return;
        }

        rtTrans.DOScale(Vector3.one * 0.8f, 0.1f).OnComplete(() =>
        {
            rtTrans.DOScale(Vector3.one * 1.25f, 0.08f).OnComplete(() =>
            {
                rtTrans.DOScale(Vector3.one * 0.85f, 0.06f).OnComplete(() =>
                {
                    rtTrans.DOScale(Vector3.one * 1.1f, 0.04f).OnComplete(() =>
                    {
                        rtTrans.DOScale(Vector3.one, 0.02f).OnComplete(() =>
                        {
                            rtTrans.DOKill();
                        });
                    });
                });
            });
        });
    }

    void Update()
    {
        updateAniScale();
    }

}
