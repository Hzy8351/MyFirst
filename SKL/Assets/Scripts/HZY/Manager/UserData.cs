using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LitJson;
using UnityEngine;
using UnityEngine.UIElements;

//�������
public class UserDatas
{
    public static string saveKey = "datas";

    #region ���켰�����
    public UserDatas()
    {
        isSave = true;
        mainData = new MainData();
        //playerLocalizeState = 1;
    }
    //public TableManager configMag = new TableManager();
    //public UserDatas playerData = null;
    public static bool isSave;                          //�Ƿ�浵
    //public int playerLocalizeState;                     //1����2Ӣ��
    //public DateTime playerLoginTime;                    //��ҵ�¼��Ϸ��ʱ��

    public MainData mainData;

    #endregion

    #region GetLocalData()��ȡ��ұ�������
    //��ȡ�����������
    public static UserDatas GetLocalData(ref bool bNew)
    {
        UserDatas newData = null;
        //��ȡ��������
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

    #region ClearLocalData() ������ػ�����
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

    #region InitData()�������ñ�ͱ���ı��ػ����ݳ�ʼ�������Ϸ����
    public void InitData()
    {
    }
    #endregion

    #region Save()���汾���������
    //���汾���������
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






