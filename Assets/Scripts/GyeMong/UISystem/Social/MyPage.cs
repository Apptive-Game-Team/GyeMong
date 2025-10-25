using System.Collections;
using System.Collections.Generic;
using GyeMong.UISystem.Social;
using GyeMong.UISystem.Social.Dto;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyPage : MonoBehaviour
{

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button logoutButton;
    
    private void OnEnable()
    {
        logoutButton.onClick.AddListener(OnClickLogout);
        StartCoroutine(LoadUserInfo());
    }
    
    private IEnumerator LoadUserInfo()
    {
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{AccountContext.ServerUrl}/api/members/me"))
            {
                request.SetRequestHeader("Authorization", AccountContext.GetAuthorizationHeader());
                request.timeout = 5;
                Debug.Log($"Start Request: {request.url}");
                
                yield return request.SendWebRequest();
                Debug.Log($"End Request: {request.url}");
                
                
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(request.error);
                    AccountContext.OnLogout();
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);

                    MemberResponse response = new MemberResponse();
                    JsonUtility.FromJsonOverwrite(request.downloadHandler.text, response);
                    
                    Debug.Log("User Info Loaded");
                    nameText.text = response.name;
                }
            }
            
        }
        finally
        {
        }
    }

    private void OnClickLogout()
    {
        AccountContext.OnLogout();
    }
    
}
