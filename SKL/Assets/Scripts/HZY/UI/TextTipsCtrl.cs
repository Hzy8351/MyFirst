using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextTipsCtrl : MonoBehaviour
{
    public Text msg;

    public void Init(Vector3 pos, string des, float dic, float sec)
    {
        msg.text = des;
        transform.position = pos;
        float cy = transform.localPosition.y;
        transform.localScale = Vector3.zero * 0.2f;
        transform.DOScale(Vector3.one, 0.2f);
        transform.DOLocalMoveY(cy + dic, sec).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

}
