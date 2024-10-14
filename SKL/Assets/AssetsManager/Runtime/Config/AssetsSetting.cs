using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// ����ģʽ
    /// </summary>
    public enum AssetsModel
    {
        Editor,
        EditorAB,
        Runtime
    }


    /// <summary>
    /// ������Դ����
    /// ���ڳ�ʼ����Դ������
    /// </summary>
    public class AssetsSetting : ScriptableObject
    {
        [System.Serializable]
        public class Config
        {
            /// <summary>
            /// �汾��
            /// </summary>
            public string Version;

            /// <summary>
            /// ��������
            public long BuildDate;
            /// </summary>

            /// <summary>
            /// �Ƿ�����ģʽ
            /// </summary>
            public bool Offline;

            /// <summary>
            /// ���س���
            /// </summary>
            public string LoadScene;

            /// <summary>
            /// ��ǰ����ģʽ
            /// </summary>
            [System.NonSerialized]
            public AssetsModel PlayModel = AssetsModel.Runtime;
        }

        /// <summary>
        /// ��ǰ��������
        /// </summary>
        public Config config;

        /// <summary>
        /// single instance
        /// </summary>
        static AssetsSetting _instance = null;

        /// <summary>
        /// ��ȡ����ʱ����Դ����
        /// </summary>
        /// <returns></returns>
        public static AssetsSetting GetInstance()
        {
            if (_instance == null)
            {
                _instance = Resources.Load<AssetsSetting>(nameof(AssetsSetting));
            }
            return _instance;
        }
    }
}
