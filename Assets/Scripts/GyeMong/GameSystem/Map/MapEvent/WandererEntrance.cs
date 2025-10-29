using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Map.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class WandererEntrance : MonoBehaviour
    {
        [Header("Boss Room Entrance")]
        [SerializeField] private BossRoomEntrance bossRoomEntrance;

        private bool _isTriggered = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isTriggered)
            {
                StartCoroutine(TriggerEvents());
            }
        }

        private IEnumerator TriggerEvents()
        {
            _isTriggered = true;

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            bossRoomEntrance.Trigger();
        }
    }
}
