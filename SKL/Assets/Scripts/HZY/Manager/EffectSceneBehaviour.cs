using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSceneBehaviour : MonoBehaviour
{
    public EffectInfo[] effects;

    private SkeletonAnimation sa;
    private MeshRenderer mr;
    private ClockBase clock;
    private string effName;
    private bool bTick;

    private int einx;
    private int estate;

    private void Awake()
    {
        sa = gameObject.GetComponent<SkeletonAnimation>();
        mr = gameObject.GetComponent<MeshRenderer>();
        clock = new ClockBase();
    }

    public void initViews(string EffectName, Vector3 pos, int order, float delayTick)
    {
        mr.enabled = false;
        clock.InitTick(delayTick);
        estate = 1;
        einx = 0;

        initBase(EffectName, pos, order);
    }

    private void initBase(string EffectName, Vector3 pos, int order)
    {
        mr.sortingOrder = order;
        effName = EffectName;
        transform.localPosition = pos;
    }

    private void initAni(EffectInfo info)
    {
        sa.AnimationState.SetAnimation(0, info.aniName, info.bLoop);
        clock.InitTick(info.durTick);
        bTick = (info.durTick > 0);
    }

    private void FixedUpdate()
    {
        updateDelay();
        updateTick();
    }

    private void updateDelay()
    {
        if (effects == null)
        {
            return;
        }

        if (estate != 1)
        {
            return;
        }

        if (clock.updateTick())
        {
            mr.enabled = true;
            estate = 2;
            initAni(effects[einx]);
        }
    }

    private void updateTick()
    {
        if (effects == null)
        {
            return;
        }

        if (estate != 2)
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
            EffectManager.instance.destoryEffectScene(effName, gameObject);
        }

    }
}
