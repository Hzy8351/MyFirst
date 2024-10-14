using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyBehaviour : MonoBehaviour
{
    public Image icon;
    public Text val;

    private long valLast;
    private int idLast;

    public bool checkValChanged(long val)
    {
        if (val == valLast)
        {
            return false;
        }

        valLast = val;
        return true;
    }

    public void setIcon(int id)
    {
        if (id == idLast)
        {
            return;
        }

        icon.sprite = UIManager.getIcon(id);
        idLast = id;
    }


}

