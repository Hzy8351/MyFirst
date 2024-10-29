using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTriggter : MonoBehaviour
{
    public HeroBehaviour hb;
    public SphereCollider sc;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter: " + other.name);
        onTrigs(other);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    //Debug.Log("OnTriggerStay: " + other.name);
    //    //onTrigs(other);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //}

    private void onTrigs(Collider other)
    {
        int layer = other.gameObject.layer;
        if (layer == 7) //block
        {
            onTrigBlock(other.gameObject.GetComponent<MapBlock>());
            return;
        }
        if (layer == 8) //enemy
        {
            onTrigEnemy(other.gameObject.GetComponent<ColliderBehaviour>().objRoot.GetComponent<EnemyBehaviour>());
            return;
        }
        if (layer == 9) //item
        {
            onTrigItem(other.gameObject.GetComponent<MapItem>());
            return;
        }
        if (layer == 10) //boss
        {
            onTrigBoss(other.gameObject.GetComponent<ColliderBehaviour>().objRoot.GetComponent<BossBehaviour>());
            return;
        }
        if (layer == 11) //attack
        {
            return;
        }
    }

    private void onTrigBlock(MapBlock mb)
    {
        int damage = (hb.cbInfo.hp >= MapManager.instance.CFGD.heroMinHpCheck) ? hb.cbInfo.hp / 2 : hb.cbInfo.hp;
        hb.addHp(-damage);
        //GameManager.instance.CreateTextTips("-" + damage);
    }

    private void onTrigItem(MapItem mi)
    {
        if (mi.State != 1)
        {
            return;
        }

        hb.addHp(mi.TB.val);
        mi.destoryThis();
        SoundManager.instance.playSound("Chop");
    }

    private void onTrigEnemy(EnemyBehaviour eb)
    {
        if (hb.cbInfo.hp >= eb.cbInfo.hp)
        {
            hb.addHp(eb.ETB.score);
            eb.setDie();
            return;
        }

        int damage = (hb.cbInfo.hp >= MapManager.instance.CFGD.heroMinHpCheck) ? hb.cbInfo.hp / 2 : hb.cbInfo.hp;
        hb.addHp(-damage);
    }

    private void onTrigBoss(BossBehaviour bb)
    {
        if (hb.cbInfo.hp >= bb.cbInfo.hp)
        {
            hb.addHp(bb.ETB.score);
            bb.setDie();
            return;
        }

        int damage = (hb.cbInfo.hp >= MapManager.instance.CFGD.heroMinHpCheck) ? hb.cbInfo.hp / 2 : hb.cbInfo.hp;
        hb.addHp(-damage);
    }

    private void onTrigAttack()
    {

    }
}
