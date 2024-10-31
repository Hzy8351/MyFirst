using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private CharaterBehaviour cb;

    private Camera cam;
    public Camera CAM { get { return cam; } }

    void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    public void setCameraSize(float s)
    {
        cam.orthographicSize = s;
    }

    public float getCameraSize()
    {
        return cam.orthographicSize;
    }

    public void setCB(CharaterBehaviour c)
    {
        cb = c;
    }

    private void updatePos()
    {
        if (cb == null)
        {
            return;
        }

        Vector3 pos = cb.transform.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }

    void Update()
    {
        updatePos();
    }
}
