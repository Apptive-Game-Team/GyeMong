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
            //0
            var animBoolEvent = new SetAnimatorParameterBool();
            animBoolEvent.SetAnimator(someAnimator);
            animBoolEvent.SetParameter("isDown", true);
            yield return animBoolEvent.Execute();

            //1 : Input Key False
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            Debug.Log("0");

            //2
            yield return StartCoroutine((new OpenChatEvent().Execute()));
            Debug.Log("7");
            
            //3
            yield return new ShowMessages(chatData, autoSkipTime).Execute();
            Debug.Log("11");

            //4
            yield return StartCoroutine((new CloseChatEvent().Execute()));
            Debug.Log("12");

            //5
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
            Debug.Log("17");

            //6
            animBoolEvent.SetAnimator(someAnimator);
            animBoolEvent.SetParameter("isDown", false);
            yield return animBoolEvent.Execute();

            //7
            boss.Trigger();

            //8
            yield return new HideBossHealthBarEvent().Execute();

            //9
            var showHpBar = new ShowBossHealthBarEvent();
            showHpBar.SetBoss(boss);
            yield return showHpBar.Execute();
        }
    }
}

