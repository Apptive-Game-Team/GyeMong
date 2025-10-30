using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.UISystem.Social;
using GyeMong.UISystem.Social.Dto;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AutoLoginPage : MonoBehaviour
{
    
    private void Awake()
    {
        string jwt = PlayerPrefs.GetString(AccountContext.JwtTokenKey, "");
        if (!string.IsNullOrEmpty(jwt))
        {
            CheckAndLogin(jwt);
            AccountContext.OnLogin(jwt);
        }
        else
        {
            OnFail();
        }
    }
    
    private void CheckAndLogin(string jwt)
    {
        StartCoroutine(Check(jwt, () =>
        {
            AccountContext.OnLogin(jwt);
            Debug.Log("Auto Login Success");
        }, OnFail));
    }
    
    private void OnFail()
    {
        PlayerPrefs.SetString(AccountContext.JwtTokenKey, "");
        Debug.Log("Auto Login Failed");
        AccountContext.CurrentPage = AccountContext.PageType.LOGIN;
    }
    
    private IEnumerator Check(string jwt, Action OnSuccess, Action OnFail)
    {
        using(UnityWebRequest request = new UnityWebRequest($"{AccountContext.ServerUrl}/api/members/check", "GET"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {AccountContext.JwtToken}");
        
            yield return request.SendWebRequest();
        
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                OnFail();
            }
            else
            {
                OnSuccess();
            }
        }
    }
}
