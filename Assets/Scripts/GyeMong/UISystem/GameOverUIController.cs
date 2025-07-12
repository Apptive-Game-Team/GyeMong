using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.InputSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class GameOverUIController : MonoBehaviour
{
    
    [SerializeField] GameObject gameOverUI;
    private void Awake()
    {
        PlayerChangeListenerCaller.OnPlayerDied += SetGameOverUI;
    }

    void SetGameOverUI()
    {
        gameOverUI.SetActive(true);
        StartCoroutine(WaitForInteract());
    }

    IEnumerator WaitForInteract()
    {
        yield return new WaitUntil(() => InputManager.Instance.GetKeyDown(ActionCode.Interaction));
        gameOverUI.SetActive(false);
        StageManager.LoseStage(this);
    }
}
