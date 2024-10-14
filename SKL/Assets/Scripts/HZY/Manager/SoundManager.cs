using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private string pathName;

    private string musicName;
    private AudioBehaviour abMusic;

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);

        pathName = "Audio/Clip";
    }

    public void stopMusic()
    {
        if (abMusic != null)
        {
            destoryClip(abMusic.gameObject);
            abMusic = null;
        }
    }

    public void playCurMusic()
    {
        playMusic(musicName);
    }

    public void playMusic(string clipName)
    {
        musicName = clipName;
        stopMusic();
        if (!GameManager.instance.getMusicState())
        {
            return;
        }

        abMusic = play(musicName, true);
    }

    public AudioBehaviour playSound(string clipName)
    {
        if (!GameManager.instance.getSoundState())
        {
            return null;
        }

        return play(clipName, false);
    }

    private AudioBehaviour play(string clipName, bool bLoop)
    {
        GameObject go = GameManager.instance.AddPrefab(pathName, transform);
        if (go == null)
        {
            return null;
        }

        AudioBehaviour ab = go.GetComponent<AudioBehaviour>();
        if (ab == null || !ab.play(clipName, bLoop))
        {
            destoryClip(go);
            return null;
        }

        return ab;
    }

    public void destoryClip(GameObject obj)
    {
        GameManager.instance.DestroyPrefab(pathName, obj);
    }


}
