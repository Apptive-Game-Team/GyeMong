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
        [SerializeField] private GameObject bodyTip;
        
        [Header("Constraint")]
        [SerializeField] private MultiParentConstraint root;
        [SerializeField] private bool isBend;
        [SerializeField] private MultiParentConstraint bodyConstraint;
        
        [Header("Sprite")]
        [SerializeField] private List<Sprite> headSprites;
        [SerializeField] private List<Sprite> bodySprites;
        [SerializeField] private List<Sprite> tailSprites;

        [Header("Sandworm Body")]
        [SerializeField] private List<SpriteRenderer> sandwormBody;

        [Header("Motion Value")] 
        [SerializeField] private float bodyRotateValue;

        private Tween _idleTween;
        private bool _isIdle = true;
        private bool _check = true;

        private void Start()
        {
            //IdleMove();
        }

        private void Update()
        {
            FacePlayer();
            if (Input.GetMouseButton(1))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                StartCoroutine(HeadAttackMove(mousePos, 0.2f));
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(HideOrShow(_check, 0.5f));
                _check = !_check;
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
            Vector3 line2 = (originalPos - root.transform.position).normalized;

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
            IdleMove();
        }

        public IEnumerator HeadAttackMove(Vector3 dest, float dur, float preDelay = 0.5f, float postDelay = 0.1f,
            float backDelay = 1f)
        {
            _isIdle = false;
            _idleTween?.Kill();

            Vector3 originalBodyPos = bodyTip.transform.position;
            Vector3 originalHeadPos = moveTip.transform.position;
            
            Vector3 rootDir = (dest - root.transform.position).normalized;
            float rootDist = Vector3.Magnitude(dest - root.transform.position);
            float angle = dest.x > originalBodyPos.x ? -bodyRotateValue : bodyRotateValue;

            Sequence seq = DOTween.Sequence()
                .SetUpdate(UpdateType.Late)
                .SetAutoKill(true);

            
            seq.Append(bodyTip.transform.DOMove(originalBodyPos - rootDir / 2, preDelay).SetEase(Ease.Linear));
            seq.Join(moveTip.transform.DORotate(new Vector3(0,0, angle), preDelay).SetEase(Ease.Linear));

            Vector3 startPos = moveTip.transform.position;
            Vector3 endPos = root.transform.position + rootDir * (rootDist * 1.2f);
            Vector3 controlPos = (startPos + endPos) / 2f + Vector3.up * Vector3.Distance(startPos, endPos) / 5;
            //seq.AppendCallback(() => SmoothBlendSource(2, 0f, dur / 4));
            seq.Append(bodyTip.transform.DOMove(originalBodyPos, dur).SetEase(Ease.OutCubic));
            seq.Join(moveTip.transform.DOPath(
                new[] { startPos, controlPos, endPos },
                dur, 
                PathType.CatmullRom
            ).SetEase(Ease.OutCubic));
            seq.Join(moveTip.transform.DORotate(new Vector3(0,0,-angle), dur).SetEase(Ease.OutCubic));

            seq.AppendInterval(postDelay);
            
            seq.Append(moveTip.transform.DOMove(originalHeadPos, backDelay).SetEase(Ease.InOutSine));
            seq.Join(moveTip.transform.DORotate(Vector3.zero, backDelay).SetEase(Ease.InOutSine));
            float lastStartTime = seq.Duration() - backDelay;
            seq.InsertCallback(lastStartTime + backDelay / 2f, () =>
            {
                //SmoothBlendSource(2, 0.5f, backDelay);
            });
            
            yield return seq.WaitForCompletion();
            _isIdle = true;
        }

        public void BodyAttackMove()
        {
            
        }

        public IEnumerator HideOrShow(bool hide, float duration)
        {
            _isIdle = false;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3 dir = (mousePos - root.transform.position).normalized;
            if (hide)
            {
                yield return (moveTip.transform.DOPath(
                    new[]
                    {
                        moveTip.transform.position, transform.TransformPoint(new Vector3(0, 1.5f, 0)),
                        transform.position
                    }, 
                    duration
                    , PathType.CatmullRom
                ).SetEase(Ease.OutCubic)).WaitForCompletion();
            }
            else
            {
                _isIdle = true;
                FacePlayer(true);
                _isIdle = false;
                moveTip.transform.position = transform.position;
                yield return (moveTip.transform.DOPath(
                    new[]
                    {
                        moveTip.transform.position, transform.TransformPoint(new Vector3(0, 1.5f, 0)),
                        transform.TransformPoint(new Vector3(0, 2.5f, 0)) + dir * 0.4f
                    }, 
                    duration
                    , PathType.CatmullRom
                ).SetEase(Ease.OutCubic)).WaitForCompletion();
                _isIdle = true;
            }
        }

        private void SmoothBlendSource(int index, float target, float duration)
        {
            var sources = bodyConstraint.data.sourceObjects;
            float start = sources.GetWeight(index);
            DOTween.To(() => start, w =>
            {
                var s = bodyConstraint.data.sourceObjects;
                s.SetWeight(index, w);
                bodyConstraint.data.sourceObjects = s;
                start = w;
            }, target, duration).SetUpdate(UpdateType.Late);
        }

        private void FacePlayer(bool hide = false)
        {
            if (!_isIdle) return;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3 dir = (mousePos - root.transform.position).normalized;

            if (!hide)
            {
                moveTip.transform.position = transform.TransformPoint(new Vector3(0, 2.5f, 0)) + dir * 0.4f;
            }
            bodyTip.transform.position = transform.TransformPoint(new Vector3(0, 1.2f, 0)) - dir * 1f;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            if (angle < 0) angle += 360f;

            if (angle is (>= 0f and < 30f) or ( >= 330f and < 360f))
            {
                sandwormBody[0].sprite = headSprites[0];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[0];
                }
                sandwormBody[0].sortingOrder = 18;
            }
            else if (angle is >= 30f and < 60f)
            {
                sandwormBody[0].sprite = headSprites[1];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[1];
                }
                sandwormBody[0].sortingOrder = 12;
            }
            else if (angle is >= 60f and < 120f)
            {
                sandwormBody[0].sprite = headSprites[2];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[1];
                }
                sandwormBody[0].sortingOrder = 12;
            }
            else if (angle is >= 120f and < 150f)
            {
                sandwormBody[0].sprite = headSprites[3];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[1];
                }
                sandwormBody[0].sortingOrder = 12;
            }
            else if (angle is >= 150f and < 210f)
            {
                sandwormBody[0].sprite = headSprites[4];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[2];
                }
                sandwormBody[0].sortingOrder = 18;
            }
            else if (angle is >= 210f and < 240f)
            {
                sandwormBody[0].sprite = headSprites[5];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[3];
                }
                sandwormBody[0].sortingOrder = 18;
            }
            else if (angle is >= 240f and < 300f)
            {
                sandwormBody[0].sprite = headSprites[6];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[3];
                }
                sandwormBody[0].sortingOrder = 18;
            }
            else
            {
                sandwormBody[0].sprite = headSprites[7];
                for (int i = 1; i < sandwormBody.Count; i++)
                {
                    sandwormBody[i].sprite = bodySprites[3];
                }
                sandwormBody[0].sortingOrder = 18;
            }
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
