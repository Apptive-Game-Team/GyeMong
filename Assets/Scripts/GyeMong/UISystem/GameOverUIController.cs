using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Util;

public class GameOverUIController : MonoBehaviour
{
    
    [SerializeField] GameObject gameOverUI;
    private void Awake()
    {
        PlayerChangeListenerCaller.OnPlayerDied += SetGameOverUI;
    }
    private void OnDestroy()
    {
        PlayerChangeListenerCaller.OnPlayerDied -= SetGameOverUI;
    }
    void SetGameOverUI()
    {
        StartCoroutine(WaitForRestart());
    }

    IEnumerator WaitForRestart()
    {
        yield return new WaitForSeconds(2f);
        gameOverUI.SetActive(true);
        int openedFrame = Time.frameCount;
        yield return new WaitUntil(() => InputManager.Instance.ConsumeAdvanceInput(openedFrame));
        gameOverUI.SetActive(false);
        StageManager.LoseStage(this);
        #if UNITY_EDITOR
        SceneLoader.LoadScene(SceneManager.GetActiveScene().name);
        #endif
    }
}
