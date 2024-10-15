using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILodingAdaptation : MonoBehaviour
{
    [Header("�ȱ�����")]
    public RectTransform[] adps;    // ����
    [Header("����")]
    public RectTransform[] rpx;     // X������ƫ��
    [Header("���¿�")]
    public RectTransform[] rpy;     // Y������ƫ��
    [Header("���ҿ�")]
    public RectTransform[] lpx;     // X���긺ƫ��
    [Header("���Ͽ�")]
    public RectTransform[] lpy;     // Y���긺ƫ��

    private RectTransform rtCanvas;

    void Awake()
    {
        rtCanvas = gameObject.transform.parent.gameObject.GetComponent<RectTransform>();
        float sy = (rtCanvas.rect.height / 2400f);
        float sx = (rtCanvas.rect.width / 1080f);

        float minus = sy - sx;
        //Debug.Log("sy = " + sy + ", sx = " + sx + ", minus = " + minus);
        if (minus <= -0.10f)
        {
            sy = sx;
        }
        else if (minus >= 0.10f)
        {
            sx = sy;
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

        //float d = 0;
        for (int i = 0; i < rpy.Length; ++i)
        {
            RectTransform rt = rpy[i];
            Vector3 pos = rt.localPosition;
            float val = Mathf.Abs(pos.y * (sy - 1f));
            if (sy < 1f) { val = -val; }
            pos.y += val;
            //d += val;
            rt.localPosition = pos;
            //Debug.Log(d);
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
