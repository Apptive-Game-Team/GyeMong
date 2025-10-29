using GyeMong.EventSystem;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer;
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

        [Header("Wanderer")]
        [SerializeField] private GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer.Wanderer wanderer;

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

            var showHpEvent = new ShowWandererHealthBarEvent();
            showHpEvent.SetWanderer(wanderer);
            yield return showHpEvent.Execute();

            yield return new WaitForSeconds(5f);

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }

    public class ShowWandererHealthBarEvent
    {
        [SerializeField] private GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer.Wanderer _wanderer;

        public void SetWanderer(GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer.Wanderer wanderer)
        {
            _wanderer = wanderer;
        }

        public IEnumerator Execute()
        {
            var hpBar = SceneContext.EffectManager.GetHpBarController();
            hpBar.gameObject.SetActive(true);
            hpBar.ClearBoss();
            hpBar.UpdateHp(_wanderer.CurrentHp, _wanderer.MaxHp);
            yield return null;
        }
    }
}
