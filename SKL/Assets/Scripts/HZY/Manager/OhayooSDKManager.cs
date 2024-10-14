using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OhayooSDKManager : MonoSingleton<OhayooSDKManager>
{
    #if USE_OHAYOO_SDK
        private LGInitSuccessInfo lGInitSuccessInfo;

        #if USE_NEW
            private LGAbsRealName _realName;
            private LightGameSDK.Ad.LightGameRewardAd grad;
            private OhayooAdvCallback ohaAdv;
        #else
            private LGRewardAd_ADV_JH_H m_LGRewardAd_ADV_JH_H;
        #endif

    #endif

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this.gameObject);
        //debug("Init begin");

        initSDK();
    }

    private void setConfigLongitude()
    {
    #if USE_OHAYOO_SDK
        //Debug.Log("setConfigLongitude Begin");
        bool isOn = LGSDKAd.MediationAdService.FetchCurrentCAIDSwitchState();
        if (!isOn)
        {
            //Debug.Log("setConfigLongitude isOn = false ");
            //启用CAID
            LGSDKAd.MediationAdService.ConfigCAIDSwitchIsOn(true);
        }

        bool inOn2 = LGSDKAd.MediationAdService.FetchCurrentCAIDSwitchState();
        //Debug.Log("setConfigLongitude isOn2 = " + inOn2);

        //不启用CAID
        //LGSDKAd.MediationAdService.ConfigCAIDSwitchIsOn(false);

        ////设置当前经度
        //LGSDKAd.MediationAdService.ConfigLongitude(0f);
        ////查看当前经度
        //var currentLongitude = LGSDKAd.MediationAdService.CurrentLongitude();

        ////设置当前纬度
        //LGSDKAd.MediationAdService.ConfigLatitude(0f);
        ////查看当前纬度
        //var currentLatitude = LGSDKAd.MediationAdService.CurrentLatitude();
        //Debug.Log("setConfigLongitude End currentLatitude = " + currentLatitude + ", currentLongitude = " + currentLongitude);

    #endif
    }

#if USE_OHAYOO_SDK
    /// <summary>
    /// 初始化成功回调.
    /// 所有SDK功能都需要在初始化成功之后调⽤
    /// </summary>
    /// <param name="initSuccessInfo">初始化成功后返回的信息，详⻅LGInitSuccessInfo内部字段说明</param>
    private void InitSuccess(LGInitSuccessInfo initSuccessInfo)
    {
        debug("InitSuccess");
        lGInitSuccessInfo = initSuccessInfo;

        Invoke("DelayNewData", 0.2f);
    }

    private void DelayNewData()
    {
        // TODO 请处理SDK初始化成功后的游戏逻辑
        debug(string.Format(
            $"初始化sdk成功,DeviceID:{lGInitSuccessInfo.DeviceID}," +
            $"InstallID:{lGInitSuccessInfo.InstallID}," +
            $"SsID:{lGInitSuccessInfo.SsID}," +
            $"UserUniqueID:{lGInitSuccessInfo.UserUniqueID}"
            ));

        debug("InitSuccess DelayNewData");

#if USE_NEW
        grad = gameObject.AddComponent<LightGameSDK.Ad.LightGameRewardAd>();
        ohaAdv = gameObject.AddComponent<OhayooAdvCallback>();
        ohaAdv.initListeners(grad);

        ByteDance.Union.AgeLevel.LGAgeLevelService.Instance.OnShow += OnShow;
        ByteDance.Union.AgeLevel.LGAgeLevelService.Instance.OnClose += OnClose;
#else
        m_LGRewardAd_ADV_JH_H = gameObject.AddComponent<LGRewardAd_ADV_JH_H>();
        LGSDKCore.ComplianceService.GetAgeLevlService().OnShow += OnShow;
        LGSDKCore.ComplianceService.GetAgeLevlService().OnClose += OnClose;
#endif

        //FetchDeviceRealName();
        SetRealNameCallback();

        RequireIDFA();
        setConfigLongitude();

        ohayoo_game_init gr = new ohayoo_game_init(1, "加载资源成功");
        GameReport("ohayoo_game_init", JsonUtility.ToJson(gr));

        ohayoo_game_init gr2 = new ohayoo_game_init(2, "SDK初始化成功");
        GameReport("ohayoo_game_init", JsonUtility.ToJson(gr2));

#if USE_NEW
        if (grad != null) { grad.userID = lGInitSuccessInfo.UserUniqueID; }
#endif

        InitController.bInitSDK = true;
    }

#endif

    public void initSDK()
    {

#if USE_OHAYOO_SDK
    #if USE_NEW
        //Debug.Log("initSDK  11111");
        LGSDKCore.Init(InitSuccess, InitFail);
        //Debug.Log("initSDK  22222");
    #endif
#else
        InitController.bInitSDK = true;
#endif
    }


#if USE_OHAYOO_SDK
    /// <summary>
    /// 初始化失败回调
    /// </summary>
    /// <param name="code">错误码.</param>
    /// <param name="message">错误信息.</param>
    private void InitFail(int code, string message)
    {
        // TODO 此处需要打印出code及message，可根据code及message进⾏问题排查
        debug(string.Format($"初始化sdk错误,错误代码code:{code},错误信息message:{message}"));
        //Debug.Log("InitFail InitController.instance = " + InitController.instance);
        InitController.instance.showFailMsgBox();
    }
    public LGInitSuccessInfo GetLGInitSuccessInfo()
    {
        return lGInitSuccessInfo;
    }
#endif

    // 设置实名制及防沉迷回调，注意：需要在SDK初始化之前设置回调
    public void SetRealNameCallback()
    {
#if USE_OHAYOO_SDK
        //绑定防沉迷回调,点击防沉迷弹框确认按钮时触发,在该回调中终止玩家游戏
        LGAbsRealName.AntiAddictionCallback = () =>
        {
            //TODO 添加终止玩家游戏逻辑
            debug("账号防沉迷：点击防沉迷弹框确认按钮，触发退出逻辑");
            Quit();
        };

        //绑定实名认证成功的回调
        LGAbsRealName.GlobalRealNameSuccessCallback = (isAdult, rewardInfo) =>
        {
            //TODO 添加实名认证成功后逻辑
            debug(string.Format($"账号实名认证：成功isAdult:{isAdult},isRealNameVerified:"));
            InitController.instance.bRealName = true;
        };

        //绑定实名认证失败的回调
        LGAbsRealName.GlobalRealNameFailCallback = (errorCode, errorMessage) =>
        {
            //TODO 添加实名认证失败后逻辑 
            debug(string.Format($"账号实名认证：失败errorCode:{errorCode},errorMessage:{errorMessage}"));
        };
#endif
    }
    //检查设备是否实名认证，无帐号体系专用。
    public void FetchDeviceRealName()
    {
        //Debug.Log("FetchDeviceRealName 1111111");
    #if USE_OHAYOO_SDK
        #if USE_NEW
        #if UNITY_EDITOR
                        _realName = new LGEditorRealName();
        #elif UNITY_ANDROID
                                    _realName = new LGAndroidRealName();
        #elif UNITY_IOS
                                    _realName = new LGiOSRealName();
        #endif

        //Debug.Log("FetchDeviceRealName 22222222");
        _realName.FetchDeviceRealName(
            (isAdult, isRealNameVerified) =>
            {
                //TODO 添加实名认证成功后逻辑
                debug(string.Format($"设备实名认证：成功isAdult:{isAdult},isRealNameVerified:{isRealNameVerified}"));
                InitController.instance.bRealName = true;
            },
            (errorCode, errorMessage) =>
            {
                //TODO 添加实名认证失败后逻辑
                debug(string.Format($"设备实名认证：失败errorCode:{errorCode},errorMessage:{errorMessage}"));
            }
        );
#endif
#endif
    }


    // 设置打开个保法页面的显示/关闭回调
    public void SetPrivacyServiceCallback()
    {
#if USE_OHAYOO_SDK
#if USE_NEW
        ByteDance.Union.PersonalPrivacySetting.LGPersonalPrivacySettingService.Instance.OnShow += () =>
#else
        LGSDKCore.PersonalPrivacyService.OnShow = () =>
#endif
        {
            //TODO 添加个保法页面显示逻辑
            debug("个保法页面展示");
        };

#if USE_NEW
        ByteDance.Union.PersonalPrivacySetting.LGPersonalPrivacySettingService.Instance.OnClose += () =>
#else
        LGSDKCore.PersonalPrivacyService.OnClose = () =>
#endif
        {
            //TODO 添加个保法页面关闭逻辑
            debug("个保法页面关闭");
        };


#endif
    }
    // 打开个保法页面
    public void OpenPersonalPrivacySettingsWindow()
    {
        //debug("准备打开个保法页面");
#if USE_OHAYOO_SDK
#if USE_NEW
        ByteDance.Union.PersonalPrivacySetting.LGPersonalPrivacySettingService.Instance.OpenPersonalPrivacySettingsWindow();
#else
        LGSDKCore.PersonalPrivacyService.OpenPersonalPrivacySettingsWindow();
#endif
#endif

    }

    void debug(string msg)
    {
        //Debug.Log("*****" + msg);
    }
    public void Quit()
    {
#if UNITY_ANDROID
        Application.Quit(); //app本体退出
#endif
    }

    public void OpenYongHuXieYi()
    {
        //debug("展示用户协议");
#if USE_OHAYOO_SDK

#if USE_NEW
        LGSDKDevKit.DevKitBaseService.OpenUserProtocol();
#else
        LGSDKCore.ComplianceService.OpenUserProtocol();
#endif
#endif

    }
    public void OpenYinSiZhengCe()
    {
        //debug("展示隐私政策");
#if USE_OHAYOO_SDK
#if USE_NEW
        LGSDKDevKit.DevKitBaseService.OpenPrivacyProtocol();
#else
        LGSDKCore.ComplianceService.OpenPrivacyProtocol();
#endif
#endif
    }

    public void OpenRenZhengXieYi()
    {
        //debug("展示认证协议");
#if USE_OHAYOO_SDK
#if USE_NEW
        LGSDKDevKit.DevKitBaseService.OpenIdentifyProtocol();
#else
        LGSDKCore.ComplianceService.OpenIdentifyProtocol();
#endif
#endif
    }

#if USE_OHAYOO_SDK
    private void OnShow()
    {
        //"age level hint Show"
        debug("展示适龄提示");
    }
    private void OnClose()
    {
        //"age level hint close"
        debug("关闭适龄提示");
    }
#endif

    public void OpenAgeTip()
    {
        //打开弹窗
#if USE_OHAYOO_SDK
#if USE_NEW
        ByteDance.Union.AgeLevel.LGAgeLevelService.Instance.OpenAgeTip();
#else
        LGSDKCore.ComplianceService.GetAgeLevlService().OpenAgeTip();
#endif
#endif
    }

    public void CloseAgeLevelHint()
    {
        //你可以通过点击弹窗上的关闭按钮来关闭弹窗，或者你需要代码控制关闭时机，按需调⽤关闭弹窗的⽅法
#if USE_OHAYOO_SDK
#if USE_NEW
        ByteDance.Union.AgeLevel.LGAgeLevelService.Instance.CloseAgeLevelHint();
#else
        LGSDKCore.ComplianceService.GetAgeLevlService().CloseAgeLevelHint();
#endif
#endif
    }

    public void ZhuXiaoXinXi(Action<string> onSuccess, Action<int, string> onFail)
    {
#if USE_OHAYOO_SDK
#if USE_NEW
        LGDeviceInfoService.Instance.CloseDeviceInfo(onSuccess, onFail);
#else
        LGSDKCore.ComplianceService.CloseDeviceInfo(onSuccess, onFail);
#endif
#endif
    }

//    // 展示激励广告
//    public void ShowRewardAd(HallDefines.onClickADV callBack, string adp, string adptype)
//    {
//#if USE_OHAYOO_SDK
//#if UNITY_EDITOR
//        if (callBack != null) { callBack(true); }
//#elif UNITY_ANDROID
//#if USE_NEW
//        if (ohaAdv != null) { ohaAdv.PlayAd(callBack, adp, adptype); }
//#else
//        if (m_LGRewardAd_ADV_JH_H != null) { m_LGRewardAd_ADV_JH_H.PlayAd(callBack, adp, adptype); }
//#endif

//#elif UNITY_IOS
//#if USE_NEW
//        if (ohaAdv != null) { ohaAdv.PlayAd(callBack, adp, adptype); }
//#else
//        if (m_LGRewardAd_ADV_JH_H != null) { m_LGRewardAd_ADV_JH_H.PlayAd(callBack, adp, adptype); }
//#endif
//#endif
//#else
//        if (callBack != null) { callBack(true); }
//#endif

//    }

    public void LoadRewardAd()
    {
#if USE_OHAYOO_SDK
#if USE_NEW
#if UNITY_ANDROID
                    if (grad != null) { grad.LoadAd(); }
#elif UNITY_IOS
                    if (grad != null) { grad.LoadAd(); }
#endif
#endif
#endif

    }

    public void ClearAdvBallback()
    {
#if USE_OHAYOO_SDK
#if USE_NEW
    if (ohaAdv != null) { ohaAdv.ClearAdCallback(); }
#else
    if (m_LGRewardAd_ADV_JH_H != null) { m_LGRewardAd_ADV_JH_H.ClearAdvBallback(); }
#endif
#endif
    }

#if USE_OHAYOO_SDK
    void RequireIDFA()
    {
#if UNITY_IOS
            LGSDKAd.MediationAdService.iOS_RequireIDFA(delegate (bool hasAuthorized, string idfaString)
            {
                var msg = $"是否有权限 : {hasAuthorized} \n idfaString : {idfaString}";
                Debug.Log(msg);
            });
#endif
    }
#endif

    public void GameReport(string key, string value)
    {
        //Debug.Log("GameReport key = " +  key + ", value = " + value);
#if USE_OHAYOO_SDK
            LGSDKCore.AppLogService.OnEventV3(key, value);
#endif
    }

}