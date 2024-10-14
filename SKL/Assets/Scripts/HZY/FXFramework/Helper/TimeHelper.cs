using System;
using System.Collections.Generic;
using System.Net;

public class HttpHelper
{
    /// <summary>
    /// 网页请求数据
    /// </summary>
    /// <param name="getUrl">请求数据接口</param>
    /// <param name="callback">请求回调</param>
    public static void RequestWeb(string getUrl, Action<WebResponse> callback)
    {
        WebRequest request = WebRequest.Create(getUrl);
        request.Timeout = 1000;
        request.Credentials = CredentialCache.DefaultCredentials;
        using (WebResponse response = request.GetResponse())
        {
            callback?.Invoke(response);
            request?.Abort();
        }
    }
    /// <summary>
    /// 请求web头部数据
    /// </summary>
    /// <param name="getUrl">接口地址</param>
    /// <returns>网页JSON数据</returns>
    public static Dictionary<string, string> RequestWebHeader(string getUrl)
    {
        Dictionary<string, string> dic = null;

        RequestWeb(getUrl, response =>
        {
            WebHeaderCollection headerCollection = response.Headers;

            string[] allKeys = headerCollection.AllKeys;

            dic = new Dictionary<string, string>(allKeys.Length);

            foreach (var key in allKeys) dic.Add(key, headerCollection[key]);

            headerCollection?.Clear();
        });

        return dic;
    }
}

public class TimeHelper
{
    public static DateTime GetNetDateTime()
    {
        var dic = HttpHelper.RequestWebHeader("https://www.baidu.com");
        return dic.TryGetValue("Date", out string date) ? DateTime.Parse(date) : default;
    }
}