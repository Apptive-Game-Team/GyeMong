using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class SandwormMovement : MonoBehaviour
    {
        [Header("Tip")]
        [SerializeField] private GameObject moveTip;
        [SerializeField] private GameObject headRotateTip;
        
        [Header("Bend Root")]
        [SerializeField] private MultiParentConstraint bendRoot;
        [SerializeField] private bool isBend;
        
        [Header("Sprite")]
        [SerializeField] private List<Sprite> headSprites;
        [SerializeField] private List<Sprite> bodySprites;
        [SerializeField] private List<Sprite> tailSprites;

        [Header("Sandworm Body")]
        [SerializeField] private List<GameObject> sandwormBody;

        private Tween _idleTween;

        private void Start()
        {
            IdleMove();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                StartCoroutine(AttackMove(mousePos, 0.2f));
            }
        }
        
        public void IdleMove()
        {
            Vector3 randomOffset = new Vector3(0f, Random.Range(-0.1f, -0.05f), 0f);
            
            Vector3 targetPos = moveTip.transform.position + randomOffset;
            
            _idleTween = DOTween.Sequence()
                .Join(moveTip.transform.DOMove(targetPos, 1f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo))
                .AppendInterval(0.2f)
                .OnComplete(IdleMove);
        }

        public IEnumerator AttackMove(Vector3 dest, float dur, float preDelay = 0.5f, float postDelay = 0.1f, float backDelay = 1f)
        {
            _idleTween?.Kill();

            Vector3 originalPos = moveTip.transform.position;
            Vector3 originalRot = new Vector3(0, 0, 0);

            Vector3 line1 = (originalPos - dest).normalized;
            Vector3 line2 = (originalPos - bendRoot.transform.position).normalized;

            float angle = Vector3.SignedAngle(line1, line2, Vector3.forward) * -2f;
            float preRot = -angle / 2f;
            
            var seq = DOTween.Sequence();
            
            Vector3 prePos = originalPos - (dest - originalPos).normalized * (-0.2f);
            seq.Append(moveTip.transform.DOMove(prePos, preDelay).SetEase(Ease.Linear));
            seq.Join(moveTip.transform.DORotate(new Vector3(0, 0, preRot), preDelay).SetEase(Ease.Linear));
            
            seq.Append(moveTip.transform.DOMove(dest, dur).SetEase(Ease.InOutSine));
            seq.Join(moveTip.transform.DORotate(new Vector3(0, 0, angle), dur).SetEase(Ease.InOutSine));
            
            seq.AppendInterval(postDelay);
            
            seq.Append(moveTip.transform.DOMove(originalPos, backDelay).SetEase(Ease.Linear));
            seq.Join(moveTip.transform.DORotate(originalRot, backDelay).SetEase(Ease.Linear));
            
            seq.OnKill(() =>
            {
                moveTip.transform.position = originalPos;
                moveTip.transform.rotation = Quaternion.Euler(originalRot);
            });
            yield return seq.WaitForCompletion();
            
            moveTip.transform.position = originalPos;
            moveTip.transform.rotation = Quaternion.Euler(originalRot);
        }

        public IEnumerator RotateHead(bool direction, float upAngle, float upTime, float downAngle, float downTime, float recoverDelay, float recoverTime)
        {
            Sequence seq = DOTween.Sequence();
            if (direction)
            {
                upAngle = -upAngle;
                downAngle = -downAngle;
            }

            seq.Append(headRotateTip.transform.DORotate(new Vector3(0, 0, upAngle), upTime)
                .SetEase(Ease.Linear));
            seq.Append(headRotateTip.transform.DORotate(new Vector3(0, 0, downAngle), downTime)
                .SetEase(Ease.InCubic));
            seq.AppendInterval(recoverDelay);
            seq.Append(headRotateTip.transform.DORotate(new Vector3(0, 0, 0), recoverTime)
                .SetEase(Ease.OutBack));
            yield return seq.WaitForCompletion();
        }
    }
}
