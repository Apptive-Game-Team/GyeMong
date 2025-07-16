using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class ElfPhaseChange : MonoBehaviour
    {
        [SerializeField] private Animator someAnimator;

        [SerializeField] private MultiChatMessageData chatData;
        [SerializeField] private float autoSkipTime = 3f;

        [SerializeField] private Creature.Mob.StateMachineMob.Boss.Boss boss;

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            var animBoolEvent = new SetAnimatorParameterBool();
            animBoolEvent.SetAnimator(someAnimator);
            animBoolEvent.SetParameter("isDown", true);

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());

            yield return StartCoroutine((new OpenChatEvent().Execute()));

            yield return new ShowMessages(chatData, autoSkipTime).Execute();

            yield return StartCoroutine((new CloseChatEvent().Execute()));

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            animBoolEvent.SetAnimator(someAnimator);
            animBoolEvent.SetParameter("isDown", false);
            yield return animBoolEvent.Execute();

            boss.Trigger();

            yield return new HideBossHealthBarEvent().Execute();

            var showHpBar = new ShowBossHealthBarEvent();
            showHpBar.SetBoss(boss);
            yield return showHpBar.Execute();
        }
    }
}

