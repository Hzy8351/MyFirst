using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy_Des : MonoBehaviour
{
    private ParticleSystem[] _particleSystem;
    void Start()
    {
        _particleSystem = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        bool istrue = true;
        foreach (ParticleSystem p in _particleSystem)
        {
            if (!p.isStopped)
            {
                istrue = false;
            }
        }
        if (istrue)
        {
            Destroy(gameObject);

        }
    }
}
