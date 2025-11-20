using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaWarrior;
using GyeMong.GameSystem.Creature.Player.Component;
using System.Collections;
using System.Collections.Generic;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class NagaWarriorDown : MonoBehaviour
    {
        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            BgmManager.Stop();

            yield return new HideBossHealthBarEvent().Execute();
        }
    }
}
