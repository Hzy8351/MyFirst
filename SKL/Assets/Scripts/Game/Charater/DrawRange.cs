using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRange : MonoBehaviour
{
    public LineRenderer lineRender;

    public void viewRender(bool b)
    {
        lineRender.gameObject.SetActive(b);
    }

    public void drawCircleDebug(Vector3 self, float dis)
    {
        int nCircle = 360;
        Vector3 beginPos = self;
        Vector3 endPos = Vector3.zero;
        float step = 2 * Mathf.PI / nCircle;
        bool bFirst = true;
        for (float s = 0; s < 2 * Mathf.PI; s += step)
        {
            float x = dis * Mathf.Cos(s);
            float z = dis * Mathf.Sin(s);
            endPos.x = self.x + x;
            endPos.z = self.z + z;

            if (bFirst)
            {
                bFirst = false;
            }
            else
            {
                Debug.DrawLine(beginPos, endPos, Color.red);
            }

            beginPos = endPos;
        }

    }

    public void drawCircleRender(Vector3 self, float dis, Color color)
    {
        viewRender(true);
        int nCircle = 360;
        lineRender.positionCount = nCircle;
        lineRender.endWidth = 0.1f;
        lineRender.startWidth = 0.1f;
        lineRender.startColor = color;
        lineRender.endColor = color;
        lineRender.loop = true;

        float angle = 360f / nCircle;
        for (int i = 0; i < nCircle; ++i)
        {
            lineRender.SetPosition(i, self + Quaternion.AngleAxis(angle * i, Vector3.up) * Vector3.forward * dis);
        }
    }
}
