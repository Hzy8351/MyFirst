using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectInfo
{
    public string aniName;
    public float durTick;
    public bool bLoop;
}

public class EffectUIBehaviour : MonoBehaviour
{
    public EffectInfo[] effects;

    private SkeletonGraphic sg;
    private RectTransform rt;
    private ClockBase clock;
    private string effName;
    private bool bTick;

    private int einx;
    private int estate;

    private void Awake()
    {
        sg = gameObject.GetComponent<SkeletonGraphic>();
        rt = gameObject.GetComponent<RectTransform>();
        clock = new ClockBase();
    }

    public string getName()
    {
        return effName;
    }

    public void setStartAni(string aniName, bool bLoop)
    {
        sg.AnimationState.SetAnimation(0, aniName, bLoop);
    }

    public void setSkin(string skName)
    {
        sg.initialSkinName = skName;
        sg.Initialize(true);
    }

    public void initViews(string EffectName, Vector3 pos)
    {
        initBase(EffectName, pos);

        einx = 0;
        estate = 1;
        initAni(effects[einx]);
        
    }

    private void initBase(string EffectName, Vector3 pos)
    {
        effName = EffectName;
        rt.localPosition = pos;
        rt.localScale = Vector3.one;
    }

    private void initAni(EffectInfo info)
    {
        sg.AnimationState.SetAnimation(0, info.aniName, info.bLoop);
        clock.InitTick(info.durTick);
        bTick = (info.durTick > 0);
    }

    private void FixedUpdate()
    {
        updateTick();
    }

    private void updateTick()
    {
        if (effects == null)
        {
            return;
        }

        if (estate == 0)
        {
            return;
        }

        if (!bTick)
        {
            return;
        }

        if (clock.updateTick())
        {
            ++einx;
            if (einx < effects.Length)
            {
                initAni(effects[einx]);
                return;
            }

            estate = 0;
            einx = 0;
            EffectManager.instance.destoryEffectUI(effName, gameObject);
        }

    }
}
