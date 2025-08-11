using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.Input;
using Unity.VisualScripting;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.ShadowOfHero
{
    public class ShadowBattleEvent : MonoBehaviour
    {
        [SerializeField] private ShadowOfHero shadow;
        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;
        [SerializeField] private float cameraSize;
        [SerializeField] private float cameraDuration;
        [SerializeField] private List<MultiChatMessageData> beforeScript;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(TriggerEvent());
            }
        }

        private IEnumerator TriggerEvent()
        {
            GetComponent<Collider2D>().enabled = false;
            yield return (new SetKeyInputEvent() { _isEnable = false }).Execute();
            yield return SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed);
            yield return SceneContext.CameraManager.CameraZoomInOut(cameraSize, cameraDuration);
            if (beforeScript != null)
            {
                foreach (var script in beforeScript)
                {
                    yield return script.Play();
                }
            }
            SceneContext.CameraManager.CameraFollow(SceneContext.Character.gameObject.transform);
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
            shadow.ChangeState(new ShadowOfHero.DetectingPlayer() { mob = shadow });
        }
    }
}
