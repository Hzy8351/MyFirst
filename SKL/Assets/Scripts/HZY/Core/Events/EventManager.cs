using System.Collections.Generic;
using System;
/// <summary>
/// 最为重要，大部分逻辑依赖的事件系统，可以监听和发送事件
/// </summary>
public class EventManager
{
    private static Dictionary<Enum, Delegate> _eventDic = new Dictionary<Enum, Delegate>();
    private static void AddListener(Enum eventType, Delegate callback)
    {
        if (!_eventDic.ContainsKey(eventType))
        {
            _eventDic.Add(eventType, null);
        }
        Delegate d = _eventDic[eventType];
        if (d != null && d.GetType() != callback.GetType())
        {
            throw new Exception(string.Format("error{0}==>{1}", eventType, callback.GetType()));
        }
    }
    private static bool RemoveListener(Enum eventType, Delegate callback)
    {
        if (_eventDic.ContainsKey(eventType))
        {
            Delegate d = _eventDic[eventType];
            if (d == null)
            {
                throw new Exception(string.Format("error{0}", eventType));
            }
            else if (d.GetType() != callback.GetType())
            {
                throw new Exception(string.Format("error{0}==>{1}", eventType, callback));
            }
            return true;
        }
        else
        {
            return false;
            throw new Exception(string.Format("RemoveListener {0} Erroy", eventType));
        }
    }
    private static void OnRemoveListener(Enum eventType)
    {
        if (_eventDic[eventType] == null)
        {
            _eventDic.Remove(eventType);
        }
    }
    public static void On(Enum eventType, EventCallBack callBack)
    {
        AddListener(eventType, callBack);
        _eventDic[eventType] = (EventCallBack)_eventDic[eventType] + callBack;
    }
    public static void On<T>(Enum eventType, EventCallBack<T> callBack)
    {
        AddListener(eventType, callBack);
        _eventDic[eventType] = (EventCallBack<T>)_eventDic[eventType] + callBack;
    }
    public static void On<T, X>(Enum eventType, EventCallBack<T, X> callBack)
    {
        AddListener(eventType, callBack);
        _eventDic[eventType] = (EventCallBack<T, X>)_eventDic[eventType] + callBack;
    }
    public static void On<T, X, Y>(Enum eventType, EventCallBack<T, X, Y> callBack)
    {
        AddListener(eventType, callBack);
        _eventDic[eventType] = (EventCallBack<T, X, Y>)_eventDic[eventType] + callBack;
    }
    public static void On<T, X, Y, Z>(Enum eventType, EventCallBack<T, X, Y, Z> callBack)
    {
        AddListener(eventType, callBack);
        _eventDic[eventType] = (EventCallBack<T, X, Y, Z>)_eventDic[eventType] + callBack;
    }
    public static void On<T, X, Y, Z, W>(Enum eventType, EventCallBack<T, X, Y, Z, W> callBack)
    {
        AddListener(eventType, callBack);
        _eventDic[eventType] = (EventCallBack<T, X, Y, Z, W>)_eventDic[eventType] + callBack;
    }
    public static void Once(Enum eventType, EventCallBack callBack)
    {
        AddListener(eventType, callBack);
        EventCallBack e = () => { Off(eventType, callBack); };
        _eventDic[eventType] = (EventCallBack)_eventDic[eventType] + callBack + e;
    }
    public static void Once<T>(Enum eventType, EventCallBack<T> callBack)
    {
        AddListener(eventType, callBack);
        EventCallBack<T> e = (arg) => { Off(eventType, callBack); };
        _eventDic[eventType] = (EventCallBack<T>)_eventDic[eventType] + callBack + e;
    }
    public static void Once<T, X>(Enum eventType, EventCallBack<T, X> callBack)
    {
        AddListener(eventType, callBack);
        EventCallBack<T, X> e = (arg1, arg2) => { Off(eventType, callBack); };
        _eventDic[eventType] = (EventCallBack<T, X>)_eventDic[eventType] + callBack + e;
    }
    public static void Once<T, X, Y>(Enum eventType, EventCallBack<T, X, Y> callBack)
    {
        AddListener(eventType, callBack);
        EventCallBack<T, X, Y> e = (arg1, arg2, arg3) => { Off(eventType, callBack); };
        _eventDic[eventType] = (EventCallBack<T, X, Y>)_eventDic[eventType] + callBack + e;
    }
    public static void Once<T, X, Y, Z>(Enum eventType, EventCallBack<T, X, Y, Z> callBack)
    {
        AddListener(eventType, callBack);
        EventCallBack<T, X, Y, Z> e = (arg1, arg2, arg3, arg4) => { Off(eventType, callBack); };

        _eventDic[eventType] = (EventCallBack<T, X, Y, Z>)_eventDic[eventType] + callBack + e;
    }
    public static void Once<T, X, Y, Z, W>(Enum eventType, EventCallBack<T, X, Y, Z, W> callBack)
    {
        AddListener(eventType, callBack);
        EventCallBack<T, X, Y, Z, W> e = (arg1, arg2, arg3, arg4, arg5) => { Off(eventType, callBack); };
        _eventDic[eventType] = (EventCallBack<T, X, Y, Z, W>)_eventDic[eventType] + callBack + e;
    }
    public static void Off(Enum eventType, EventCallBack callBack)
    {
        if (!RemoveListener(eventType, callBack)) return;
        _eventDic[eventType] = (EventCallBack)_eventDic[eventType] - callBack;
        OnRemoveListener(eventType);
    }
    public static void Off<T>(Enum eventType, EventCallBack<T> callBack)
    {
        if (!RemoveListener(eventType, callBack)) return;
        _eventDic[eventType] = (EventCallBack<T>)_eventDic[eventType] - callBack;
        OnRemoveListener(eventType);
    }
    public static void Off<T, X>(Enum eventType, EventCallBack<T, X> callBack)
    {
        if (!RemoveListener(eventType, callBack)) return;
        _eventDic[eventType] = (EventCallBack<T, X>)_eventDic[eventType] - callBack;
        OnRemoveListener(eventType);
    }
    public static void Off<T, X, Y>(Enum eventType, EventCallBack<T, X, Y> callBack)
    {
        if (!RemoveListener(eventType, callBack)) return;
        _eventDic[eventType] = (EventCallBack<T, X, Y>)_eventDic[eventType] - callBack;
        OnRemoveListener(eventType);
    }
    public static void Off<T, X, Y, Z>(Enum eventType, EventCallBack<T, X, Y, Z> callBack)
    {
        if (!RemoveListener(eventType, callBack)) return;
        _eventDic[eventType] = (EventCallBack<T, X, Y, Z>)_eventDic[eventType] - callBack;
        OnRemoveListener(eventType);
    }
    public static void Off<T, X, Y, Z, W>(Enum eventType, EventCallBack<T, X, Y, Z, W> callBack)
    {
        if (!RemoveListener(eventType, callBack)) return;
        _eventDic[eventType] = (EventCallBack<T, X, Y, Z, W>)_eventDic[eventType] - callBack;
        OnRemoveListener(eventType);
    }
    public static void Send(Enum eventType)
    {
        if (_eventDic.TryGetValue(eventType, out Delegate d))
        {
            EventCallBack callBack = d as EventCallBack;
            callBack?.Invoke();
        }
    }
    public static void Send<T>(Enum eventType, T arg)
    {
        if (_eventDic.TryGetValue(eventType, out Delegate d))
        {
            EventCallBack<T> callBack = d as EventCallBack<T>;
            callBack?.Invoke(arg);
        }
    }
    public static void Send<T, X>(Enum eventType, T arg1, X arg2)
    {
        if (_eventDic.TryGetValue(eventType, out Delegate d))
        {
            EventCallBack<T, X> callBack = d as EventCallBack<T, X>;
            callBack?.Invoke(arg1, arg2);
        }
    }
    public static void Send<T, X, Y>(Enum eventType, T arg1, X arg2, Y arg3)
    {
        if (_eventDic.TryGetValue(eventType, out Delegate d))
        {
            EventCallBack<T, X, Y> callBack = d as EventCallBack<T, X, Y>;
            callBack?.Invoke(arg1, arg2, arg3);
        }
    }
    public static void Send<T, X, Y, Z>(Enum eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        if (_eventDic.TryGetValue(eventType, out Delegate d))
        {
            EventCallBack<T, X, Y, Z> callBack = d as EventCallBack<T, X, Y, Z>;
            callBack?.Invoke(arg1, arg2, arg3, arg4);
        }
    }
    public static void Send<T, X, Y, Z, W>(Enum eventType, T arg1, X arg2, Y arg3, Z arg4, W agr5)
    {
        if (_eventDic.TryGetValue(eventType, out Delegate d))
        {
            EventCallBack<T, X, Y, Z, W> callBack = d as EventCallBack<T, X, Y, Z, W>;
            callBack?.Invoke(arg1, arg2, arg3, arg4, agr5);
        }
    }
}