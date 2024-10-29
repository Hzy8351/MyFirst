using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuaryMarkUI : MonoBehaviour
{
    public GameObject objHp;
    public Transform transPanel;

    public Text hpDesc;
    public RectTransform hpRect;
    public Color[] hpColors;

    private CharaterBehaviour me;

    public int getShowState()
    {
        if (objHp.activeSelf) { return 1; }
        return 0;
    }

    public void onHide()
    {
        transPanel.localScale = Vector3.one;
        me = null;

        objHp.SetActive(false);
    }

    public float initHp(string str, float sc, EnemyBehaviour eb)
    {
        onHide();
        objHp.SetActive(true);

        me = eb;
        hpDesc.text = str;
        hpDesc.transform.localScale = Vector3.one * sc;

        Vector2 sizeDelta = hpRect.sizeDelta;
        sizeDelta.x = hpDesc.preferredWidth + 20f;
        hpRect.sizeDelta = sizeDelta;
        return sizeDelta.x;
    }

    void Update()
    {
        updateHpColor();
    }

    private void updateHpColor()
    {
        if (me == null || MapManager.instance.charManager.HB == null)
        {
            return;
        }

        int i = (MapManager.instance.charManager.HB.cbInfo.hp >= me.cbInfo.hp) ? 0 : 1;
        hpDesc.color = hpColors[i];
    }
}
