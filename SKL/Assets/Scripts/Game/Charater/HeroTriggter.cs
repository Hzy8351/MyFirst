using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTriggter : MonoBehaviour
{
    public HeroBehaviour hb;
    public SphereCollider sc;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.name);
        onTrigs(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay: " + other.name);
        onTrigs(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: " + other.name);
    }

    private void onTrigs(Collider other)
    {

    }
}
