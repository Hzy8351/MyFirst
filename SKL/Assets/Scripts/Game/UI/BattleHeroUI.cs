using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHeroUI : BaseUI
{
    public static bool isBattlePause;

    public UIButton btnStart;
    public UIButton btnPause;
    public Text textTime;
    public Text textStage;
    public Text textScore;
    public Text textLv;

    public Image imgBarHp;
    public RectTransform rtBarIcon;

    private HeroBehaviour hb;
    private StageTb stb;
    private float tickTime;
    private Vector3 iconDefaultPos;
    private float iconWidth;

    private int[] scores;

    protected override void OnInit()
    {
        base.OnInit();
        UITween = false;

        btnPause.AddUniqueClickListerer(onBtnPause);
        btnStart.AddUniqueClickListerer(onBtnStart);
        iconDefaultPos = rtBarIcon.localPosition;
        iconWidth = iconDefaultPos.x * 2f;
    }

    private void onBtnPause()
    {
        isBattlePause = true;
        updateButtonView();
    }

    private void onBtnStart()
    {
        isBattlePause = false;
        updateButtonView();
    }

    private void updateButtonView()
    {
        btnStart.gameObject.SetActive(isBattlePause);
        btnPause.gameObject.SetActive(!isBattlePause);
    }

    public void inits(HeroBehaviour h, StageTb tb)
    {
        hb = h;
        stb = tb;
        isBattlePause = false;
        updateButtonView();

        tickTime = 0f;
        updateTickTime();

        string[] arr = GameManager.instance.CM.Split(stb.scores, "|");
        scores = new int[arr.Length];
        for (int i=0; i<arr.Length; ++i)
        {
            scores[i] = int.Parse(arr[i]);
        }

        hb.cbInfo.hp = stb.hphero;
        updateHpBar();
    }

    void FixedUpdate()
    {
        updateTickTime();
        updateHpBar();
    }

    private void updateTickTime() 
    {
        if (isBattlePause)
        {
            return;
        }

        tickTime += Time.deltaTime;

        int secAll = (int)tickTime;
        string secStr = (secAll % 60 <= 9) ? ":0" : ":";
        int min = (secAll / 60);
        string minStr = min <= 9 ? "0" + min : min.ToString();
        textTime.text = minStr + secStr + secAll % 60;
    }

    private void updateHpBar()
    {
        textScore.text = hb.cbInfo.hp.ToString();
        imgBarHp.fillAmount = hb.cbInfo.hp / (float)scores[scores.Length - 1];
        if (imgBarHp.fillAmount > 1.0f) { imgBarHp.fillAmount = 1.0f; }

        Vector3 pos = rtBarIcon.localPosition;
        pos.x = imgBarHp.fillAmount * iconWidth - iconDefaultPos.x;
        rtBarIcon.localPosition = pos;
    }
}
