using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private CharaterBehaviour cb;

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
