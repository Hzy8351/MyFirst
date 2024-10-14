using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class RemoteVersion
    {
        /// <summary>
        /// ����汾
        /// </summary>
        public string build;

        /// <summary>
        /// Ŀ��汾��
        /// </summary>
        public string version;

        /// <summary>
        /// ��Ȩ��ַ
        /// </summary>
        public string authUrl;

        /// <summary>
        /// �Ƿ�����ģʽ
        /// </summary>
        public bool offline;

        /// <summary>
        /// ��Դ��������
        /// </summary>
        public string resUrl;

        /// <summary>
        /// ��������
        /// </summary>
        public string launch;

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public long buildDate;

        /// <summary>
        /// �������µ�ַ
        /// </summary>
        public string package;

        /// <summary>
        /// �������б����ʱ��
        /// </summary>
        public string serverListUrl;

        /// <summary>
        /// �ж��Ƿ�������ظ���
        /// </summary>
        /// <returns></returns>
        public bool canDownload()
        {
            if (!Uri.TryCreate(resUrl, UriKind.Absolute, out var url))
                return false;
            return true;
        }
    }

    [Serializable]
    public class RemoteSetting
    {
        // ��ǰ���汾����
        [SerializeField]
        RemoteVersion main;
        // �Ӱ汾
        [SerializeField]
        RemoteVersion[] match;
        // ƥ�䵱ǰ�汾
        public RemoteVersion MatchVersion(string version)
        {
            RemoteVersion matchVer = null;
            if (match != null)
            {
                var split = version.Split('.');
                foreach (var item in match)
                {
                    if (string.IsNullOrEmpty(item.build))
                        continue;
                    if (item.build == version)
                        return item;
                    // ƥ��汾
                    var tmp = item.build.Split('.');
                    if (tmp.Length != split.Length)
                        continue;
                    matchVer = item;
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        bool bEqual = tmp[i] == "*" || tmp[i] == split[i];
                        if (!bEqual)
                        {
                            matchVer = null;
                            break;
                        }
                    }
                }
            }
            return matchVer != null ? matchVer : main;
        }
    }
    /// <summary>
    /// ��ʼ������
    /// </summary>
    public class Setting : ScriptableObject
    {
        /// <summary>
        /// instance
        /// </summary>
        static Setting _instance;
        public static Setting Instance()
        {
            if (_instance == null)
                _instance = Resources.Load<Setting>(nameof(Setting));
            return _instance;
        }

        // Զ������
        public static RemoteVersion RemoteSetting;

        [Tooltip("��ʼ��URL")]
        public string RemoteURL;

        [Tooltip("������")]
        public string Channel;

        [Tooltip("�Ƿ�֧��Console")]
        public bool EnableDebugLogger;

        [Tooltip("������ɺ����������")]
        public string StartScene;
    }

}