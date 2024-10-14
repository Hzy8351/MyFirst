using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdaptation : MonoBehaviour
{
    [Header("等比缩放")]
    public RectTransform[] adps;    // 缩放
    [Header("向左靠")]
    public RectTransform[] rpx;     // X座标正偏移
    [Header("向下靠")]
    public RectTransform[] rpy;     // Y座标正偏移
    [Header("向右靠")]
    public RectTransform[] lpx;     // X座标负偏移
    [Header("向上靠")]
    public RectTransform[] lpy;     // Y座标负偏移

    void Awake()
    {
        RectTransform rtCanvas = CanvasManager.instance.tranUI;
        float sy = (rtCanvas.rect.height / 2400);// 1920f);
        float sx = (rtCanvas.rect.width / 1080);

        float minus = sy - sx;
        //Debug.Log("sy = " + sy + ", sx = " + sx + ", minus = " + minus);
        if (minus <= -0.12f)
        {
            sx = sy;
        }
        else if (minus >= 0.12f)
        {
            sy = sx;
        }
        else
        {
            float per = (sx + sy) * 0.5f;
            sx = per;
            sy = per;
        }

        for (int i = 0; i < adps.Length; ++i)
        {
            RectTransform rt = adps[i];
            rt.localScale = new Vector3(sx, sy, 1);
        }

        for (int i = 0; i < rpx.Length; ++i)
        {
            RectTransform rt = rpx[i];
            Vector3 pos = rt.localPosition;
            float val = Mathf.Abs(pos.x * (sx - 1f));
            if (sx < 1f) { val = -val; }
            pos.x += val;
            rt.localPosition = pos;
        }
        for (int i = 0; i < lpx.Length; ++i)
        {
            RectTransform rt = lpx[i];
            Vector3 pos = rt.localPosition;
            float val = Mathf.Abs(pos.x * (sx - 1f));
            if (sx < 1f) { val = -val; }
            pos.x -= val;
            rt.localPosition = pos;
        }
        for (int i = 0; i < rpy.Length; ++i)
        {
            RectTransform rt = rpy[i];
            Vector3 pos = rt.localPosition;
            float val = Mathf.Abs(pos.y * (sy - 1f));
            if (sy < 1f) { val = -val; }
            pos.y += val;
            rt.localPosition = pos;
        }
        for (int i = 0; i < lpy.Length; ++i)
        {
            RectTransform rt = lpy[i];
            Vector3 pos = rt.localPosition;
            float val = Mathf.Abs(pos.y * (sy - 1f));
            if (sy < 1f) { val = -val; }
            pos.y -= val;
            rt.localPosition = pos;
        }
    }

}
