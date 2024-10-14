using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    public RectTransform tranUI;
    public RectTransform tranTips;
    public RectTransform tranClicks;
    public RectTransform tranFront;

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);
    }
}
