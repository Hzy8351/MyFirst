using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoSingleton<EffectManager>
{
    private string pathUIDestory;
    private string pathUI;
    private string pathScene;

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);

        pathUIDestory = "Prefabs/Effect/EffectUI/";
        pathUI = "Effect/EffectUI/";
        pathScene = "Effect/EffectScene/";
    }

    public EffectUIBehaviourDestory createEffectUIDestory(string effName, Transform rtParent, Vector3 pos)
    {
        Object obj = Resources.Load(pathUIDestory + effName);
        GameObject go = (GameObject)Instantiate(obj);
        if (go == null)
        {
            return null;
        }

        go.transform.SetParent(rtParent);
        EffectUIBehaviourDestory eb = go.GetComponent<EffectUIBehaviourDestory>();
        if (eb == null)
        {
            Destroy(go);
            return null;
        }

        eb.initViews(effName, pos);
        return eb;
    }

    public EffectUIBehaviour createEffectUI(string effName, Transform rtParent, Vector3 pos)
    {
        GameObject go = GameManager.instance.AddPrefab(pathUI + effName, rtParent);
        if (go == null)
        {
            return null;
        }

        EffectUIBehaviour eb = go.GetComponent<EffectUIBehaviour>();
        if (eb == null)
        {
            destoryEffectUI(effName, go);
            return null;
        }

        eb.initViews(effName, pos);
        return eb;
    }

    public void destoryEffectUI(string effName, GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(GameManager.offHide, 0f, 0f);
        }
        GameManager.instance.DestroyPrefab(pathUI + effName, obj);
    }

    public EffectSceneBehaviour createEffectScene(string effName, Transform parentTrans, Vector3 pos, int order, float delay)
    {
        GameObject go = GameManager.instance.AddPrefab(pathScene + effName, parentTrans);
        if (go == null)
        {
            return null;
        }

        EffectSceneBehaviour eb = go.GetComponent<EffectSceneBehaviour>();
        if (eb == null)
        {
            destoryEffectScene(effName, go);
            return null;
        }

        eb.initViews(effName, pos, order, delay);
        return eb;
    }

    public void destoryEffectScene(string effName, GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(GameManager.offHide, 0f, 0f);
        }
        GameManager.instance.DestroyPrefab(pathScene + effName, obj);
    }

    public void destoryEffectMoney(string effName, GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.localPosition = new Vector3(GameManager.offHide, 0f, 0f);
        }
        GameManager.instance.DestroyPrefab(pathUI + effName, obj);
    }
}
