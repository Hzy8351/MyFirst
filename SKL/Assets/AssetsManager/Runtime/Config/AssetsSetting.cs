using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum AssetsModel
    {
        Editor,
        EditorAB,
        Runtime
    }


    /// <summary>
    /// 包体资源配置
    /// 用于初始化资源管理器
    /// </summary>
    public class AssetsSetting : ScriptableObject
    {
        [System.Serializable]
        public class Config
        {
            /// <summary>
            /// 版本号
            /// </summary>
            public string Version;

            /// <summary>
            /// 构建日期
            public long BuildDate;
            /// </summary>

            /// <summary>
            /// 是否离线模式
            /// </summary>
            public bool Offline;

            /// <summary>
            /// 加载场景
            /// </summary>
            public string LoadScene;

            /// <summary>
            /// 当前运行模式
            /// </summary>
            [System.NonSerialized]
            public AssetsModel PlayModel = AssetsModel.Runtime;
        }

        /// <summary>
        /// 当前运行配置
        /// </summary>
        public Config config;

        /// <summary>
        /// single instance
        /// </summary>
        static AssetsSetting _instance = null;

        /// <summary>
        /// 获取运行时的资源设置
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
