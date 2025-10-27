using System.Collections;
using System.Collections.Generic;
using GyeMong.UISystem.Social;
using GyeMong.UISystem.Social.Dto;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginPage : MonoBehaviour
{
    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private Button loginButton;
    
    [SerializeField] private Button toJoinButton;
    
    private void Awake()
    {
        loginButton.onClick.AddListener(OnClickLogin);
        toJoinButton.onClick.AddListener(OnClickToJoin);
    }
    
    private void OnClickToJoin()
    {
        AccountContext.CurrentPage = AccountContext.PageType.JOIN;
    }
    
    private void OnClickLogin()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        
        LoginRequest requestDto = new LoginRequest
        {
            email = email,
            password = password
        };
        StartCoroutine(Login(requestDto));
    }
    
    private IEnumerator Login(LoginRequest requestDto)
    {
        try
        {
            using(UnityWebRequest request = new UnityWebRequest($"{AccountContext.ServerUrl}/api/members/login", "POST"))
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
                    Debug.LogError(request.error);
                    AccountContext.IsLoading = false;
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);

                    AuthResponse response = new AuthResponse();
                    JsonUtility.FromJsonOverwrite(request.downloadHandler.text, response);
                    
                    AccountContext.IsLoading = false;
                    Debug.Log("Login Success End Of Loading");
                    AccountContext.OnLogin(response.jwt);
                    ResetText();
                }
            }
        } finally
        {
            AccountContext.IsLoading = false;
        }
    }
    
    private void ResetText()
    {
        emailInput.text = "";
        passwordInput.text = "";
    }
}
