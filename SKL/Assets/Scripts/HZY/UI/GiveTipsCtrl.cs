using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GiveTipsCtrl : MonoBehaviour
{
    public Text mName;
    public Text mNum;
    public Image image;

    public void Iint(string iconName, string textName, string num, float dis, float sec) 
    {
        mName.text = textName;
        mNum.text = num;
        string path = "Icons/Icon/" + iconName;
        image.sprite = ResourcesLoad.Instance.Load<Sprite>(path);
        DoMove(dis, sec);
    }
    
    private void DoMove(float dis, float sec)
    {
        transform.DOLocalMoveY(transform.localPosition.y + dis, sec).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject, 0.1f);
        });
    }

}
