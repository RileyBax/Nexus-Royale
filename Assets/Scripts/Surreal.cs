using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Response<T>
{
    public Result<T>[] result;
}

[System.Serializable]
public class Result<T>
{
    public T[] result;
}

public static class UnityWebRequestExtensions
{
    public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();
        request.SendWebRequest().completed += operation =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                tcs.SetResult(request);
            }
            else
            {
                tcs.SetException(new Exception(request.error));
            }
        };
        return tcs.Task;
    }
}

public static class Surreal
{
    public static bool IsLoading { get; private set; }
    private static string url = "https://nexus.ryanfolio.live";
    private static string dbUser = "root";
    private static string dbPassword = "NexusRoyale";
    private static string dbAuth = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(dbUser + ":" + dbPassword));
    private static string dbDB = "nexus";
    private static string dbNS = "nexus";

    public static async Task<T[]> Query<T>(string query)
    {
        IsLoading = true;
        var request = new UnityWebRequest(url + "/sql", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(query);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", "Basic " + dbAuth);
        request.SetRequestHeader("NS", dbNS);
        request.SetRequestHeader("DB", dbDB);

        try
        {
            await request.SendWebRequestAsync();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                Response<T> responseArray = JsonUtility.FromJson<Response<T>>("{\"result\":" + request.downloadHandler.text + "}");
                Result<T> response = responseArray.result[0];
                return response.result;            }
            else
            {
                Debug.Log(request.error);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        return null;
    }
}
