using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitController : MonoBehaviour
{
    public static bool bInitSDK = false;
    [NonSerialized] public bool bRealName = true; // sdk的FetchDeviceRealName没有触发起回调(强制设置true了)，这个开关本来应该是false，等回调成功再成true的。

    public static InitController instance;

    void Awake()
    {
        instance = this;

#if UNITY_EDITOR
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        bInitSDK = true;
        bRealName = true;
#elif UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#elif UNITY_IOS
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

        Application.targetFrameRate = 60;
    }


}