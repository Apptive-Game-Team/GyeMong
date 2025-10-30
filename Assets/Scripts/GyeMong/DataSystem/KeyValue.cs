using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.UISystem.Social;
using UnityEngine;
using UnityEngine.Networking;

namespace GyeMong.DataSystem
{
public class KeyValue
{
    [Serializable]
    class ValueDto 
    {
        public string value;
    }
    public static IEnumerator GetValue(string key, Action<string> callback)
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{AccountContext.ServerUrl}/api/systems/gyemong/members/me/key-values/{key}");
        request.SetRequestHeader("Authorization", AccountContext.GetAuthorizationHeader());
        request.timeout = 5;
        request.downloadHandler = new DownloadHandlerBuffer();
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            callback.Invoke(null);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);

            ValueDto response = new ValueDto();
            JsonUtility.FromJsonOverwrite(request.downloadHandler.text, response);
                
            Debug.Log("Value Loaded");
            callback.Invoke(response.value);
        }
    }
    
    public static IEnumerator SetValue(string key, string value, Action<bool> callback)
    {
        using UnityWebRequest request = new UnityWebRequest($"{AccountContext.ServerUrl}/api/systems/gyemong/members/me/key-values/{key}", "PUT");
        request.SetRequestHeader("Authorization", AccountContext.GetAuthorizationHeader());
        request.timeout = 5;
        ValueDto dto = new ValueDto { value = value };
        string json = JsonUtility.ToJson(dto);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            callback.Invoke(false);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            Debug.Log("Value Set");
            callback.Invoke(true);
        }
    }
}

}