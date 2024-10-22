using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStickControl : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public JoyStickUI jsui;
    public RectTransform rtJoy;
    public RectTransform rtIcon;
    public RectTransform rtRound;

    private int dragState;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag" + eventData.position);
        if (dragState != 0)
        {
            return;
        }

        dragState = 1;

        rtJoy.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragState != 1)
        {
            return;
        }
        onCtrl(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragState = 0;
        rtJoy.transform.localPosition = Vector3.zero;
        rtIcon.transform.localPosition = Vector3.zero;
        rtRound.transform.localRotation = Quaternion.identity;


        jsui.joyEnd();
    }

    private void onCtrl(PointerEventData eventData)
    {
        rtIcon.transform.position = eventData.position;
        if (Vector3.Distance(rtIcon.transform.localPosition, Vector3.zero) <= 40f)
        {
            return;
        }
        Vector3 pos = rtIcon.transform.localPosition;
        rtRound.transform.localRotation = Quaternion.LookRotation(Vector3.forward, pos);
    }

    void updateMove()
    {
        if (dragState == 0)
        {
            return;
        }

        Vector3 pos = rtIcon.transform.localPosition;
        pos.Normalize();
        jsui.joyMove(pos);
    }
    
    void Update()
    {
        updateMove();
    }

}
