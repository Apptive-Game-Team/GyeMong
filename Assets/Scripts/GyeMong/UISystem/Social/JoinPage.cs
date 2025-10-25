using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.UISystem.Social;
using GyeMong.UISystem.Social.Dto;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JoinPage : MonoBehaviour
{

    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button toLoginButton;
    
    private void Awake()
    {
        joinButton.onClick.AddListener(OnClickJoin);
        toLoginButton.onClick.AddListener(OnClickToLogin);
    }
    
    private void OnClickToLogin()
    {
        AccountContext.CurrentPage = AccountContext.PageType.LOGIN;
    }
    
    private void OnClickJoin()
    {
        string email = emailInput.text;
        string name = nameInput.text;
        string password = passwordInput.text;
        
        JoinRequest requestDto = new JoinRequest
        {
            email = email,
            name = name,
            password = password
        };
        StartCoroutine(Join(requestDto));
    }

    private IEnumerator Join(JoinRequest requestDto)
    {
        try
        {
            using(UnityWebRequest request = new UnityWebRequest($"{AccountContext.ServerUrl}/api/members", "POST"))
            {
                string json = JsonUtility.ToJson(requestDto);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                AccountContext.IsLoading = true;
            
                yield return request.SendWebRequest();
            
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(request.downloadHandler.text);
                    ShowError(request.downloadHandler.text);
                    AccountContext.IsLoading = false;
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);

                    AuthResponse response = new AuthResponse();
                    JsonUtility.FromJsonOverwrite(request.downloadHandler.text, response);
                    
                    Debug.Log($"jwtToken: {response.jwt}");
                    
                    AccountContext.IsLoading = false;
                    AccountContext.OnJoin(response.jwt);
                    ResetText();
                }
            }
        } finally
        {
            AccountContext.IsLoading = false;
        }
    }
    
    private void ShowError(string message)
    {
        // Implement error display logic here
        Debug.LogError(message);
        GetComponentInChildren<ErrorDisplay>().ShowError(message.Replace("|", "\n"));
    }

    private void ResetText()
    {
        emailInput.text = "";
        nameInput.text = "";
        passwordInput.text = "";
    }
}
