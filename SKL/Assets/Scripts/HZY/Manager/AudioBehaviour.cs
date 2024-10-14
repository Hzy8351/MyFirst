using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBehaviour : MonoBehaviour
{
    private AudioSource aus;
    private int state;

    private void Awake()
    {
        aus = gameObject.GetComponent<AudioSource>();
    }

    public bool play(string clipName, bool bLoop)
    {
        state = 0;
        AudioClip ac = Resources.Load<AudioClip>("AudioClip/" + clipName);
        if (ac == null)
        {
            return false;
        }

        state = 1;
        aus.clip = ac;
        aus.loop = bLoop;
        aus.Play();
        return true;
    }

    private void destroySelf()
    {
        state = 0;
        SoundManager.instance.destoryClip(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == 1 && !aus.isPlaying)
        {
            if (aus.loop)
            {
                aus.Play();
            }
            else
            {
                destroySelf();
            }
        }
    }
}
