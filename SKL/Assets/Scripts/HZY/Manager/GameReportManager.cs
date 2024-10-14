using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReportManager
{
    public static string getTimeYYMMSS()
    {
        return System.DateTime.Now.ToString();
    }

    public static string randomNetworkStr()
    {
        int r = Random.Range(0, 3);
        if (r == 1)
        {
            return "4G";
        }
        if (r == 2)
        {
            return "5G";
        }

        return "wifi";
    }
}

public class ohayoo_game_init
{
    public int initid = 12809;
    public string initname = "¿œ∞ÂœÎ≥‘”„¡À";
    public int initresult = 0;
    public string initerror = "-1";
    public string network = "wifi";
    public string updatepkgid = "-1";
    public float updatepkgsize = -1f;
    public string inittime = "";

    public static string myNetwork = "";

    public ohayoo_game_init(int id, string n)
    {
        inittime = GameReportManager.getTimeYYMMSS();
        if (myNetwork.Length == 0)
        {
            myNetwork = GameReportManager.randomNetworkStr();
        }
        network = myNetwork;

        initid = id;
        initname = n;
    }
}

public class ohayoo_game_guide
{
    public int guideid = 0;
    public string guidedesc = "";
    public string guidereceivetiem = "";
    public string guidecompletetime = "";

    public ohayoo_game_guide(int id, string desc)
    {
        guideid = id;
        guidedesc = desc;
        guidereceivetiem = GameReportManager.getTimeYYMMSS();
        guidecompletetime = guidereceivetiem;
    }
}

public class ohayoo_game_levelup
{
    public int aflevel = 0;
    public int beflevel = 0;
    public string costtime = "";

    public ohayoo_game_levelup(int af, int be)
    {
        aflevel = af;
        beflevel = be;
        costtime = GameReportManager.getTimeYYMMSS();
    }
}

public class ohayoo_game_request
{
    public string ad_type = "º§¿¯ ”∆µ";
    public string rit_id;
}

public class ohayoo_game_send
{
    public string ad_type = "º§¿¯ ”∆µ";
    public string rit_id;
    public string ad_code;
    public string result;
}

public class ohayoo_game_button_show
{
    public string ad_type = "º§¿¯ ”∆µ";
    public string rit_id;
    public string ad_position;
    public string ad_position_type;
}

public class ohayoo_game_button_click
{
    public string ad_type = "º§¿¯ ”∆µ";
    public string rit_id;
    public string ad_position;
    public string ad_position_type;
}

public class ohayoo_game_show
{
    public string ad_type = "º§¿¯ ”∆µ";
    public string rit_id;
    public string ad_position;
    public string ad_position_type;
    public string rit_scene = "";
    public string rit_scene_describe = "";
}

public class ohayoo_game_show_end
{
    public string ad_type = "º§¿¯ ”∆µ";
    public string rit_id;
    public string ad_position;
    public string ad_position_type;
    public string result;
    public int ad_times;
}

public class ohayoo_game_bufflevelup
{
    public string scenetype = "µÍ∆Ã";
    public int sceneid = 1;
    public int subsceneid = 1;
    public string bufftype;
    public int buffid;
    public string buffname;
    public string uptype;
    public int bef = 0;
    public int af = 0;
    public int befscenepoint = 0;
    public int afscenepoint = 0;
    public string reason;
    public string subreason = "";
}

public class ohayoo_game_task
{
    public string tasktype = "»ŒŒÒ";
    public string taskid = "0";
    public string taskname = "";
    public string taskdesc = "";
    public int taskresult = 0;
    public string taskreceive = "";
    public string taskcomplete = "";
}

public class ohayoo_game_itemflow
{
    public int itemtype = 1;
    public int itemid = 0;
    public string itemname = "";
    public int addorreduce = 1;
    public int ichange = 1;
    public int itemleft = 1;
    public string reason = "";
    public string subreason = "";
    public int isbind = 0;
}

public class ohayoo_game_battlemap
{
    public string battletype = "";
    public int battleid = 0;
    public int subbattleid = 0;
    public int battleresult = 0;
    public int battlepoint = 0;
    public int score = 0;
    public int star = 0;
    public int costtime = 0;
    public string goal = "";
    public string leftgoal = "";
    public int fishid = 0;
    public int step = 0;
    public int collect = 0;
    public int exchangecollect = 0;
}

public class ohayoo_game_moneyflow
{
    public int moneytype = 0;
    public int moneyid = 0;
    public string moneyname = "";
    public int addorreduce = 0;
    public int mchange = 0;
    public int moneyleft = 0;
    public string reason = "";
    public string subreason = "";

}




