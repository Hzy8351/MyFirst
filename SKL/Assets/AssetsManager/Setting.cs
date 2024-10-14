using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class RemoteVersion
    {
        /// <summary>
        /// 编译版本
        /// </summary>
        public string build;

        /// <summary>
        /// 目标版本号
        /// </summary>
        public string version;

        /// <summary>
        /// 授权地址
        /// </summary>
        public string authUrl;

        /// <summary>
        /// 是否离线模式
        /// </summary>
        public bool offline;

        /// <summary>
        /// 资源更新链接
        /// </summary>
        public string resUrl;

        /// <summary>
        /// 启动场景
        /// </summary>
        public string launch;

        /// <summary>
        /// 构建时间
        /// </summary>
        public long buildDate;

        /// <summary>
        /// 整包更新地址
        /// </summary>
        public string package;

        /// <summary>
        /// 服务器列表更新时间
        /// </summary>
        public string serverListUrl;

        /// <summary>
        /// 判断是否可以下载更新
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
        // 当前主版本配置
        [SerializeField]
        RemoteVersion main;
        // 子版本
        [SerializeField]
        RemoteVersion[] match;
        // 匹配当前版本
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
                    // 匹配版本
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
    /// 初始化设置
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

        // 远程配置
        public static RemoteVersion RemoteSetting;

        [Tooltip("初始化URL")]
        public string RemoteURL;

        [Tooltip("渠道号")]
        public string Channel;

        [Tooltip("是否支持Console")]
        public bool EnableDebugLogger;

        [Tooltip("加载完成后的启动场景")]
        public string StartScene;
    }

}