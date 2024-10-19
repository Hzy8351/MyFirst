using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickUI : BaseUI 
{
    private CharaterBehaviour cb;

    protected override void OnInit()
    {
        base.OnInit();
        UITween = false;


    }

    public void setCB(CharaterBehaviour c)
    {
        cb = c;
    }

    public void joyMove(Vector3 direct)
    {
        if (cb == null)
        {
            return;
        }

        float sp = cb.getCurSpeed();
        cb.move(new Vector3(sp * direct.x * Time.deltaTime, 0f, sp * direct.y * Time.deltaTime));
        cb.setDirect(direct);
        cb.setAni(cb.isRunUp() ? CharaterStates.run_up : CharaterStates.run);
    }

    public void joyEnd()
    {
        cb.setAni(CharaterStates.standby);
    }

    //private void testKeyBoard()
    //{
    //    if (cb == null)
    //    {
    //        return;
    //    }

    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        cb.move(new Vector3(-cb.getCurSpeed() * Time.deltaTime, 0f, 0f));
    //    }
    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        cb.move(new Vector3(cb.getCurSpeed() * Time.deltaTime, 0f, 0f));
    //    }
    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        cb.move(new Vector3(0f, 0f, -cb.getCurSpeed() * Time.deltaTime));
    //    }
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        cb.move(new Vector3(0f, 0f, cb.getCurSpeed() * Time.deltaTime));
    //    }

    //}

    void Update()
    {
        //testKeyBoard();
    }
}
