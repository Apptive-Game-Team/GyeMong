using GyeMong.EventSystem;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaWarrior;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer;
using GyeMong.GameSystem.Map.Boss;
using GyeMong.SoundSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class WandererEntrance : MonoBehaviour
    {
        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;
        [SerializeField] private Wanderer wanderer;
        private float delayTime = 1f;
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

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));
            //boss.GetComponent<NagaWarrior>().curBGM = Sound.Play("BGM_Summer_NagaWarrior", true);

            yield return new WaitForSeconds(delayTime);

            //yield return StartCoroutine((new ShowBossHealthBarEvent() { _boss = boss }).Execute());
            yield return StartCoroutine((new CameraFollowPlayer()).Execute());
            wanderer.StartMove();
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }
}
