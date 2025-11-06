using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime;
using GyeMong.GameSystem.Map.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererManager : MonoBehaviour
{
    public static WandererManager Instance;

    [SerializeField] private List<MultiChatMessageData> afterScript;
    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }
    public void StartWandererDeath()
    {
        StartCoroutine(WandererDeadEvent());
    }
    public IEnumerator WandererDeadEvent()
    {
        Debug.Log("Start Wanderer Dead Coroutine");
        yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
        if (afterScript != null)
        {
            foreach (var script in afterScript)
            {
                yield return script.Play();
            }
        }
        yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        StageManager.ClearStage(this);
    }
}
