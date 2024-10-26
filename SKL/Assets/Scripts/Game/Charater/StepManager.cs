using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    private HeroBehaviour hb;
    private StageTb stb;
    private int step;

    private int[] stepScores;

    public void inits(HeroBehaviour h, StageTb tb)
    {
        hb = h;
        stb = tb;
        step = 1;
        GameManager.instance.CM.Split(stb.stepscore, "|", ref stepScores);
        
        
    }

    public int getStep()
    {
        return step;
    }

    void FixedUpdate()
    {
        updateStep();
    }

    private void updateStep()
    { 
        if (step == stepScores.Length)
        {
            return;
        }

        int tmp = 0;
        for (int i = 0; i < stepScores.Length; ++i)
        {
            if (hb.cbInfo.hp >= stepScores[i])
            {
                ++tmp;
            }
            else
            {
                break;
            }
        }
        step = tmp;
    }

}
