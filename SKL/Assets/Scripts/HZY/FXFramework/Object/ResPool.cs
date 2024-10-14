using System;
using System.Collections.Generic;

namespace FXFramework
{
    public class ResPool<T> where T : UnityEngine.Object
    {
        private Dictionary<string, T> mResDic = new Dictionary<string, T>();

        public void Get(string key, Action<T> callback)
        {
            if (mResDic.TryGetValue(key, out T data))
            {
                callback(data);
                return;
            }
            ResHelper.AsyncLoad<T>(key, o =>
            {
                callback(o);
                mResDic.Add(key, o);
            });
        }
        /// <summary>
        /// 清理资源
        /// </summary>
        public void Clear() => mResDic.Clear();
    }
}