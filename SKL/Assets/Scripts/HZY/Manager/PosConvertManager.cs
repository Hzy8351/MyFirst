using UnityEngine;

/// <summary>
/// ����ת��������
/// </summary>
public static class CTUtils
{
    /// <summary>
    /// ��������ת��Ļ����
    /// </summary>
    public static Vector2 World2Screen(Vector3 wp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.WorldToScreenPoint(wp);
    }

    /// <summary>
    /// ��Ļ����ת��������
    /// </summary>
    public static Vector3 Screen2World(Vector3 sp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ScreenToWorldPoint(sp);
    }

    /// <summary>
    /// ��������ת�ӿ�����
    /// </summary>
    public static Vector2 World2Viewport(Vector3 wp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.WorldToViewportPoint(wp);
    }

    /// <summary>
    /// �ӿ�����ת��������
    /// </summary>
    public static Vector3 Viewport2World(Vector3 vp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ViewportToWorldPoint(vp);
    }

    /// <summary>
    /// ��Ļ����ת�ӿ�����
    /// </summary>
    public static Vector2 Screen2Viewport(Vector2 sp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ScreenToViewportPoint(sp);
    }

    /// <summary>
    /// �ӿ�����ת��Ļ����
    /// </summary>
    public static Vector2 Viewport2Screen(Vector2 vp, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        return camera.ViewportToScreenPoint(vp);
    }

    /// <summary>
    /// ��Ļ����תUI����
    /// </summary>
    public static Vector2 Screen2UI(Vector2 sp, RectTransform rect, Camera camera = null)
    {
        Vector2 uiLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, sp, camera, out uiLocalPos);
        return uiLocalPos;
    }

    /// <summary>
    /// ��������תUI����
    /// </summary>
    /// <param name="isUIObj">�Ƿ�Ϊui����</param>
    public static Vector2 World2UI(bool isUIObj, Vector3 wp, RectTransform rect, Camera uiCamera, Camera worldCamera = null)
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }
        Vector2 screenPos = World2Screen(wp, isUIObj ? uiCamera : worldCamera);
        return Screen2UI(screenPos, rect, uiCamera);
    }
}