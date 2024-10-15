using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Text perText1;
    public Text perText;

    public bool useBundle;
    private int downState;
    private float tick;
    private float maxTick;

    void Awake()
    {
        tick = 0f;
        maxTick = 1.7f;
    }

    void Update()
    {
        //if (useBundle)
        //{
        //    if (downState == 0)
        //    {
        //        downState = 1;
        //        BundleManager.Ins.beginDownload(onDownFailed);
        //        return;
        //    }
        //    if (downState == 1)
        //    {
        //        setProcess(BundleManager.Ins.getProgress());
        //        if (BundleManager.Ins.IsDownComplete())
        //        {
        //            downState = 2;
        //            //onTestBundle();
        //        }
        //        return;
        //    }
        //}

        tick += Time.deltaTime;
        float amount = (tick / maxTick);
        if (amount > 1f) { amount = 1f; }
        setProcess(amount);

        if (tick < 1.0f)
        {
            return;
        }

        if (tick < maxTick + 0.6f)
        {
            return;
        }

        if (InitController.bInitSDK && InitController.instance.bRealName)
        {
            if (amount >= 1f)
            {
                ohayoo_game_init gr = new ohayoo_game_init(3, "Ω¯»Î”Œœ∑");
                OhayooSDKManager.instance.GameReport("ohayoo_game_init", JsonUtility.ToJson(gr));
                SceneManager.LoadScene("Game");
            }

            return;
        }
    }

    private void setProcess(float amount)
    {
        int intAmount = (int)(amount * 1000f);
        perText1.text = perText.text = (intAmount / 10f) + "%";
    }

    //private void onTestBundle()
    //{
    //    BuffPopupUI bpui = null;
    //    WebBundleInfo wbi = BundleManager.Ins.FindWebBundleInfo("BuffPopupUI");
    //    if (wbi != null && wbi.ab != null)
    //    {
    //        GameObject go = wbi.ab.LoadAsset<GameObject>("BuffPopupUI");
    //        GameObject obj = GameObject.Instantiate(go);
    //        obj.transform.parent = this.transform.parent;
    //        obj.transform.localPosition = Vector3.zero;
    //        bpui = obj.GetComponent<BuffPopupUI>();
    //        //Debug.Log(go);
    //    }

    //    WebBundleInfo tb = BundleManager.Ins.FindWebBundleInfo("AdvertisementTb");
    //    if (tb != null && tb.ab != null)
    //    {
    //        TextAsset tta = tb.ab.LoadAsset<TextAsset>("AdvertisementTb");
    //        AdvertisementData data = new AdvertisementData();
    //        data.Init(tta.text);
    //    }

    //    WebBundleInfo tbSprite = BundleManager.Ins.FindWebBundleInfo("Icon_1");
    //    if (tbSprite != null && tbSprite.ab != null)
    //    {
    //        Sprite sp = tbSprite.ab.LoadAsset<Sprite>("Icon_1");
    //        if (bpui != null)
    //        {
    //            bpui.icon.sprite = sp;
    //        }
    //    }

    //}
}
