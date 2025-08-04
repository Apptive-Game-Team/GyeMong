using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using UnityEngine;
using Visual.Camera;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.ShadowOfHero
{
    public class ShadowEvent : MonoBehaviour
    {
        [SerializeField] private ShadowMovementController _shadow;

        [Header("First Event")]
        [SerializeField] private float cameraSize1;
        [SerializeField] private float cameraDuration1;
        [SerializeField] private Vector3 cameraDestination1;
        [SerializeField] private float cameraSpeed1;
        [SerializeField] private Vector3 playerDestination1;
        [SerializeField] private float playerSpeed1;
        
        [Header("Second Event")]
        [SerializeField] private Vector3 cameraDestination2;
        [SerializeField] private float cameraSpeed2;
        [SerializeField] private Vector3 shadowDestination2;
        [SerializeField] private float shadowSpeed2;

        private void Start()
        {
            StartCoroutine(TriggerEvent());
        }

        private IEnumerator TriggerEvent()
        {
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            
            // 그림자 숲 도착
            yield return SceneContext.CameraManager.CameraZoomInOut(cameraSize1, cameraDuration1);
            yield return SceneContext.CameraManager.CameraMove(cameraDestination1, cameraSpeed1);
            yield return StartCoroutine(SceneContext.EffectManager.FadeOut(0f));
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SceneContext.EffectManager.FadeIn(1f));
            var playerMoveEvent = new MoveCreatureEvent()
            {
                creatureType = MoveCreatureEvent.CreatureType.Player,
                iControllable = null,
                target = playerDestination1,
                speed = playerSpeed1
            };
            yield return playerMoveEvent.Execute();
            
            yield return new WaitForSeconds(1f);
            
            // 그림자 발견
            yield return SceneContext.CameraManager.CameraMove(cameraDestination2, cameraSpeed2);
            yield return (new MoveCreatureEvent()
            {
                creatureType = MoveCreatureEvent.CreatureType.Selectable,
                iControllable = _shadow,
                speed = shadowSpeed2,
                target = shadowDestination2
            }).Execute();
            yield return (new SetActiveObject() {_gameObject = _shadow.gameObject, isActive = false}).Execute();
            var playerAnimatorParameter = new SetAnimatorParameter() {_creatureType = SetAnimatorParameter.CreatureType.Player};
            playerAnimatorParameter.SetParameter("Speed", 10);
            yield return playerAnimatorParameter.Execute();
            yield return (new CameraFollowPlayer()).Execute();
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }
}
