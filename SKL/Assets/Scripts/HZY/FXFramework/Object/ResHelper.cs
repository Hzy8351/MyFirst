using System;
using System.Collections;
using UnityEngine;

namespace FXFramework
{
    public class ResHelper
    {
        public static T SyncLoad<T>(string name) where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(name);
            return res is GameObject ? GameObject.Instantiate(res) : res;
        }
        private static IEnumerator AsyncLoadRes<T>(string name, Action<T> callBack) where T : UnityEngine.Object
        {
            var r = Resources.LoadAsync<T>(name);
            while (r.isDone) yield return null;
            callBack(r.asset is GameObject ? GameObject.Instantiate(r.asset) as T : r.asset as T);
        }
        private static IEnumerator AsyncLoadResAB<T>(string name,string abName, Action<T> callBack) where T : UnityEngine.Object
        {
            var r = ABManager.instance.LoadResource<T>(abName, name);
            while (r==null) yield return null;
            callBack(r is GameObject ? GameObject.Instantiate(r) as T : r as T);
        }
        public static void AsyncLoad<T>(string name, Action<T> callBack) where T : UnityEngine.Object
        {
            PublicMono.Instance.StartCoroutine(AsyncLoadRes(name, callBack));
        }
        public static void AsyncLoadAB<T>(string name,string abName, Action<T> callBack) where T : UnityEngine.Object
        {
            PublicMono.Instance.StartCoroutine(AsyncLoadResAB(name,abName, callBack));
        }
        public static void Clear()
        {
            //卸载未占用的asset资源
            Resources.UnloadUnusedAssets();
            ABManager.instance.UnLoadAll();
            //回收内存
            GC.Collect();
        }
    }
}