using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AniInfo
{
    public string aniName;
    public float aniTick;
}

public class AniBehaviour : MonoBehaviour
{
    public MeshRenderer mr;
    public SkeletonAnimation sa;
    public AniInfo[] aniInfos;


    private string aniCurName;

    public void setSkin(string skName)
    {
        sa.initialSkinName = skName;
        sa.Initialize(true);
    }

    public int playAni(string ani)
    {
        for (int i = 0; i < aniInfos.Length; ++i)
        {
            AniInfo ai = aniInfos[i];
            if (ai.aniName == ani)
            {
                return playAni(ai.aniName, ai.aniTick <= 0f);
            }
        }
        return 0;
    }

    public int playAni(string ani, bool loop)
    {
        if (sa == null || sa.AnimationName == ani)
        {
            return 0;
        }

        sa.AnimationName = aniCurName = ani;
        sa.AnimationState.SetAnimation(0, ani, loop);        
        return 1;
    }

    public void setOrderLayer(int order)
    {
        if (mr == null)
        {
            return;
        }

        mr.sortingOrder = order;
    }

    public string getCurAniName()
    {
        return aniCurName;
    }
}
