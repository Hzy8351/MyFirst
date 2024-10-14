using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectMoney : MonoBehaviour
{
    public Image icon;
    public float delayTick;
    public Vector3[] offs;      // x,y为座标 z为时间

    private bool bToEnd;
    private int index;
    private Vector3 begin;
    private Vector3 end;

    public void initView(int id, Vector3 posBegin, Vector3 posEnd)
    {
        bToEnd = false;
        index = 0;
        icon.transform.localScale = Vector3.one;
        icon.sprite = UIManager.getIcon(id);
        icon.gameObject.SetActive(false);
        begin = posBegin;
        end = posEnd;

        transform.localPosition = posBegin;
        gameObject.SetActive(true);

        Invoke("updateViews", delayTick);
    }

    public bool isInfoComplete()
    {
        return bToEnd;
    }

    public void killDo()
    {
        transform.DOKill();
    }

    private bool updateViews()
    {
        if (index >= offs.Length)
        {
            return false;
        }

        icon.gameObject.SetActive(true);

        Vector3 info = offs[index];
        Vector3 off = new Vector3(info.x * transform.localScale.x, info.y * transform.localScale.y, 0f);
        transform.DOLocalMove(transform.localPosition + off, info.z).OnComplete(() =>
        {
            onInfoComplete();
        });

        return true;
    }

    private void onInfoComplete()
    {
        ++index;
        if (updateViews())
        {
            return;
        }

        transform.DOMove(end, 0.5f).OnComplete(() =>
        {
            onEndComplete();
        });
    }
    
    private void onEndComplete()
    {
        bToEnd = true;
        icon.transform.DOScale(Vector3.one * 1.5f, 0.15f).OnComplete(() =>
        {
            icon.transform.DOScale(Vector3.one, 0.08f).OnComplete(() =>
            {
                killDo();
                gameObject.SetActive(false);
            });
        });
    }
}
