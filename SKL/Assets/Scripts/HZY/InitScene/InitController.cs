using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitController : MonoBehaviour
{
    public static bool bInitSDK = false;
    [NonSerialized] public bool bRealName = true; // sdk��FetchDeviceRealNameû�д�����ص�(ǿ������true��)��������ر���Ӧ����false���Ȼص��ɹ��ٳ�true�ġ�

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