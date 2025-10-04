using System;
using System.Collections;
using GyeMong.DataSystem;
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
    
    [SerializeField] private TMP_Text localProgressText;
    [SerializeField] private TMP_Text remoteProgressText;
    
    [SerializeField] private Button uploadButton;
    [SerializeField] private Button downloadButton;
    
    [SerializeField] private Button feedbackButton;
    [SerializeField] private GameObject feedbackPage;
    
    private void OnEnable()
    {
        logoutButton.onClick.AddListener(OnClickLogout);
        uploadButton.onClick.AddListener(UploadProgress);
        downloadButton.onClick.AddListener(DownloadProgress);
        feedbackButton.onClick.AddListener(() => feedbackPage.SetActive(true));
        
        StartCoroutine(LoadUserInfo());
        LoadProgress();
    }
    
    private IEnumerator LoadUserInfo()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{AccountContext.ServerUrl}/api/members/me");
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
    
    private void LoadProgress()
    {
        LoadLocalProgress();
        LoadRemoteProgress();
    }

    private void LoadLocalProgress()
    {
        int stageId = PlayerPrefs.GetInt("CurrentStageId");
        localProgressText.text = $"Stage {stageId}";
    }
    
    private void LoadRemoteProgress()
    {
        StartCoroutine(KeyValue.GetValue("CurrentStageId", (value) =>
        {
            if (string.IsNullOrEmpty(value))
            {
                remoteProgressText.text = "No Data";
            }
            else
            {
                remoteProgressText.text = $"Stage {value}";
            }
        }));
    }
    
    private void UploadProgress() 
    {
        int stageId = PlayerPrefs.GetInt("CurrentStageId");
        StartCoroutine(KeyValue.SetValue("CurrentStageId", stageId.ToString(), (success) =>
        {
            if (success)
            {
                Debug.Log("Progress Uploaded");
                transform.parent.GetComponent<ErrorDisplay>().ShowError("Progress Uploaded");
                LoadRemoteProgress();
            }
            else
            {
                Debug.LogError("Failed to upload progress");
                transform.parent.GetComponent<ErrorDisplay>().ShowError("Failed to upload progress");
            }
        }));
    }
    
    private void DownloadProgress() 
    {
        StartCoroutine(KeyValue.GetValue("CurrentStageId", (value) =>
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.Log("No remote progress data");
            }
            else
            {
                if (int.TryParse(value, out int stageId))
                {
                    PlayerPrefs.SetInt("CurrentStageId", stageId);
                    Debug.Log($"Progress Downloaded: Stage {stageId}");
                    transform.parent.GetComponent<ErrorDisplay>().ShowError($"Progress Downloaded: Stage {stageId}");
                    LoadLocalProgress();
                }
                else
                {
                    Debug.LogError("Invalid remote progress data");
                    transform.parent.GetComponent<ErrorDisplay>().ShowError("Invalid remote progress data");
                }
            }
        }));
    }
    
    private void OnClickLogout()
    {
        AccountContext.OnLogout();
    }
}