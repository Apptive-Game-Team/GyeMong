using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.DataSystem;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackPage : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private InputField feedbackInput;
    
    private void OnEnable()
    {
        submitButton.onClick.AddListener(OnClickSubmit);
        closeButton.onClick.AddListener(OnClickClose);
    }
    
    private void OnDisable()
    {
        submitButton.onClick.RemoveListener(OnClickSubmit);
        closeButton.onClick.RemoveListener(OnClickClose);
    }
    
    private void OnClickSubmit()
    {
        string feedback = feedbackInput.text;
        if (!string.IsNullOrEmpty(feedback))
        {
            Debug.Log($"Feedback submitted: {feedback}");
            StartCoroutine(new Feedback().SendFeedback(feedback, (success) =>
            {
                if (success)
                {
                    Debug.Log("Feedback sent successfully.");
                    feedbackInput.text = "";
                }
                else
                {
                    Debug.Log("Failed to send feedback.");
                }
            }));
        }
        else
        {
            Debug.Log("Feedback is empty.");
        }
    }
    
    private void OnClickClose()
    {
        gameObject.SetActive(false);
    }
    
}

public class Feedback
{

    public IEnumerator SendFeedback(string feedback, Action<bool> callback)
    {
        int feedbackId = -1;
        yield return GetFeedbackId((id) => { feedbackId = id; });

        feedbackId++;
        
        bool success = false;
        yield return KeyValue.SetValue("feedback-id", feedbackId.ToString(), (b) => { success = b; });

        if (!success)
        {
            callback.Invoke(false);
            yield break;
        }

        yield return KeyValue.SetValue($"feedback-{feedbackId}", feedback, callback);
    }
    
    public IEnumerator GetFeedbackId(Action<int> callback)
    {
        yield return KeyValue.GetValue("feedback-id", (value) =>
        {
            if (int.TryParse(value, out int id))
            {
                callback.Invoke(id);
            }
            else
            {
                callback.Invoke(0);
            }
        });
    }
    
}
