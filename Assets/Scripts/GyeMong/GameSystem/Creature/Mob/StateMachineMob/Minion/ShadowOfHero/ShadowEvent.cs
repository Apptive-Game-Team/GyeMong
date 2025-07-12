using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.ShadowOfHero
{
    public class ShadowEvent : MonoBehaviour
    {
        [SerializeField] private ShadowMovementController _shadow;

        private void Start()
        {
            StartCoroutine(TriggerEvent());
        }

        private IEnumerator TriggerEvent()
        {
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            yield return (new MoveCreatureEvent()
            {
                creatureType = MoveCreatureEvent.CreatureType.Selectable,
                iControllable = _shadow,
                speed = 5f,
                target = new Vector3(57.5299988f,-70.5999985f,0)
            }).Execute();
            yield return (new SetActiveObject() {_gameObject = _shadow.gameObject, isActive = false}).Execute();
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }
}
