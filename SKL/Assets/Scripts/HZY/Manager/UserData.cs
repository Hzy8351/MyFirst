using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LitJson;
using UnityEngine;
using UnityEngine.UIElements;

//玩家数据
public class UserDatas
{
    public static string saveKey = "datas";

    #region 构造及其变量
    public UserDatas()
    {
        isSave = true;
        mainData = new MainData();
        //playerLocalizeState = 1;
    }
    //public TableManager configMag = new TableManager();
    //public UserDatas playerData = null;
    public static bool isSave;                          //是否存档
    //public int playerLocalizeState;                     //1中文2英文
    //public DateTime playerLoginTime;                    //玩家登录游戏的时间

    public MainData mainData;

    #endregion

    #region GetLocalData()读取玩家本地数据
    //读取本地玩家数据
    public static UserDatas GetLocalData(ref bool bNew)
    {
        UserDatas newData = null;
        //读取本地数据
        if (PlayerPrefs.HasKey(saveKey))
        {
            string data = PlayerPrefs.GetString(saveKey).ToString();
            newData = JsonMapper.ToObject<UserDatas>(data);
        }
        else
        {
            newData = new UserDatas();
            newData.InitData();
            bNew = true;
        }

        return newData;
    }

    public static UserDatas GetCloudData(string receiveData)
    {
        var newData = JsonMapper.ToObject<UserDatas>(receiveData);
        return newData;
    }
    #endregion

    #region ClearLocalData() 清除本地化数据
    public static void ClearLocalData()
    {
        isSave = false;
        if (PlayerPrefs.HasKey(saveKey))
        {
            PlayerPrefs.DeleteKey(saveKey);
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region InitData()根据配置表和保存的本地化数据初始化玩家游戏数据
    public void InitData()
    {
    }
    #endregion

    #region Save()保存本地玩家数据
    //保存本地玩家数据
    public void SaveGame()
    {
        if (isSave)
        {
            string data = JsonMapper.ToJson(this);
            PlayerPrefs.SetString(saveKey, data);
            PlayerPrefs.Save();
        }
    }
    #endregion




}






