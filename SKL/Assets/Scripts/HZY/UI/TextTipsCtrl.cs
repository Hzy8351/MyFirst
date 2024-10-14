using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextTipsCtrl : MonoBehaviour
{
    public Text msg;

    public void Init(string des, float dic, float sec)
    {
        msg.text = des;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f);
        transform.DOLocalMoveY(dic, sec).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

}
