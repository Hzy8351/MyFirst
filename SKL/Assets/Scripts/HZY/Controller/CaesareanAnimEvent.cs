using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaesareanAnimEvent : MonoBehaviour
{
    public void OnCaesareanStart()
    {
        EventManager.Send(ListenerType.CaesareanStart);
    }
}