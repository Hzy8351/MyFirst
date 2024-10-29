using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaryMark : MonoBehaviour
{
    public EnemyBehaviour eb;
    public BoxCollider bc;
    public float scale = 1f;

    private QuaryMarkUI qmUI;
    public QuaryMarkUI QMUI { get { return qmUI; } }

    void Awake()
    {
    }

    void Update()
    {
        onUpdatePos();
    }

    private void onUpdatePos()
    {
        if (qmUI == null || !MapManager.instance.cameraBehaviour.gameObject.activeSelf)
        {
            return;
        }

        Vector3 screenVec3Pos = MapManager.instance.cameraBehaviour.CAM.WorldToScreenPoint(transform.position);
        Vector2 screenVec2Pos = new Vector2(screenVec3Pos.x, screenVec3Pos.y);
        Vector2 textPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasManager.instance.tranFront,
                                                                screenVec2Pos,
                                                                CanvasManager.instance.camFront.worldCamera,
                                                                out textPos);

        qmUI.transform.localPosition = textPos;
    }

    public void onClick()
    {
        //int ret = eb.onClickQuaryMark();
        //if (ret >= 0)
        //{
        //    SoundManager.instance.playSound("Notification");
        //    if (ret == 0)
        //    {
        //        onHide();
        //    }
        //}
    }

    private void createQuaryMarkUI()
    {
        destroyQuaryMarkUI();

        GameObject go = GameManager.instance.AddPrefab("UI/QuaryMarkUI", CanvasManager.instance.tranFront);
        qmUI = go.GetComponent<QuaryMarkUI>();
    }

    private void destroyQuaryMarkUI()
    {
        if (qmUI == null)
        {
            return;
        }

        qmUI.onHide();
        qmUI.transform.localPosition = new Vector3(GameManager.offHide, 0, 0);
        GameManager.instance.DestroyPrefab("UI/QuaryMarkUI", qmUI.gameObject);
        qmUI = null;
    }

    public int getShowState()
    {
        return (qmUI == null) ? 0 : qmUI.getShowState();
    }

    public void onShowHp(string str)
    {
        if (qmUI != null && qmUI.objHp.activeSelf)
        {
            return;
        }

        onShow();
        float width = qmUI.initHp(str, scale, eb);
        setColliderScale(width);
    }

    private void onShow()
    {
        createQuaryMarkUI();
        gameObject.SetActive(true);
    }

    public void onHide()
    {
        destroyQuaryMarkUI();
        gameObject.SetActive(false);
    }

    public void setColliderScale(float width = 100f, float z = 0.8f)
    {
        bc.size = new Vector3(0.009f * width, 0.1f, z);
    }

}
