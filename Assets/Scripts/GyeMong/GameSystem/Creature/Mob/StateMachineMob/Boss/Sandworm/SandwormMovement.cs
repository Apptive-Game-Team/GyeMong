using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class SandwormMovement : MonoBehaviour
    {
        [Header("Tip")]
        [SerializeField] private GameObject moveTip;
        [SerializeField] private GameObject headRotateTip;
        [SerializeField] private GameObject bodyTip1;
        [SerializeField] private GameObject bodyTip2;

        [Header("Constraint")] 
        [SerializeField] private MultiParentConstraint head;
        [SerializeField] private MultiParentConstraint root;
        [SerializeField] private MultiParentConstraint bodyConstraint1;
        [SerializeField] private MultiParentConstraint bodyConstraint2;
        
        [Header("Sprite")]
        [SerializeField] private List<Sprite> headSprites;
        [SerializeField] private List<Sprite> bodySprites;
        [SerializeField] private List<Sprite> tailSprites;

        [Header("Sandworm Body")]
        public List<SpriteRenderer> sandwormBody;

        [Header("Head Attack Move Value")] 
        [SerializeField] private float bodyRotateValue;

        private Tween _idleTween;
        public bool isIdle = true;
        private bool _check = true;
        private bool[] _hidden = new bool[13];
        private int _orderCriteria;

        private void Awake()
        {
            _orderCriteria = SceneContext.Character.GetComponent<SpriteRenderer>().sortingOrder + sandwormBody.Count;
        }

        private void Start()
        {
            //IdleMove();
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

        public IEnumerator HeadAttackMove(Vector3 dest, float dur, float preDelay = 0.5f, float postDelay = 0.1f,
            float backDelay = 1f)
        {
            _idleTween?.Kill();

            Vector3 originalBodyPos = bodyTip1.transform.position;
            Vector3 originalHeadPos = moveTip.transform.position;
            
            Vector3 rootDir = (dest - root.transform.position).normalized;
            float rootDist = Vector3.Magnitude(dest - root.transform.position);
            float angle = dest.x > originalBodyPos.x ? -bodyRotateValue : bodyRotateValue;

            Sequence seq = DOTween.Sequence()
                .SetUpdate(UpdateType.Late)
                .SetAutoKill(true);

            
            seq.Append(bodyTip1.transform.DOMove(originalBodyPos - rootDir / 2, preDelay).SetEase(Ease.Linear));
            seq.Join(moveTip.transform.DORotate(new Vector3(0,0, angle), preDelay).SetEase(Ease.Linear));

            Vector3 startPos = moveTip.transform.position;
            Vector3 endPos = dest;
            float dist = Vector3.Distance(startPos, endPos);
            Vector3 midPos = (startPos + endPos) / 2 + (Vector3)(rootDir.x > 0
                ? new Vector2(-rootDir.y, rootDir.x)
                : new Vector2(rootDir.y, -rootDir.x)) * dist / 4;
            
            //seq.AppendCallback(() => SmoothBlendSource(2, 0f, dur / 4));
            seq.Append(bodyTip1.transform.DOMove(originalBodyPos, dur).SetEase(Ease.OutCubic));
            seq.Join(moveTip.transform.DOPath(
                new[] { startPos, midPos, endPos },
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
        }

        public IEnumerator AttackMove(bool dir, float upAngle, float upTime, float downAngle, float downTime, float recoverTime, float recoverAngle = 0f)
        {
            var seq = DOTween.Sequence();

            seq.Append(headRotateTip.transform.DORotate
                    (new Vector3(0, 0, dir ? upAngle : -upAngle), 
                        upTime)
                .SetEase(Ease.OutSine));
            seq.Append(headRotateTip.transform.DORotate
                    (new Vector3(0, 0, dir ? downAngle : -downAngle), 
                        downTime)
                .SetEase(Ease.InQuad));
            seq.Append(headRotateTip.transform.DORotate(new Vector3(0, 0, recoverAngle), recoverTime)
                .SetEase(Ease.Linear));


            yield return seq.WaitForCompletion();
        }

        public IEnumerator BodyAttackMove(Vector3 targetPos, float speed)
        {
            isIdle = true;
            FacePlayer(true);
            isIdle = false;
            
            int length = sandwormBody.Count;
            Vector3 dir = (targetPos - root.transform.position).normalized;
            float dist = Vector3.Distance(root.transform.position, targetPos);
            float duration = dist / speed;
            var rootSource = root.data.sourceObjects;
            var bodySource1 = bodyConstraint1.data.sourceObjects;
            var bodySource2 = bodyConstraint2.data.sourceObjects;
            bodySource1.SetWeight(2, 0);
            bodyConstraint1.data.sourceObjects = bodySource1;
            bodySource2.SetWeight(2, 0);
            bodyConstraint2.data.sourceObjects = bodySource2;
            
            moveTip.transform.position = transform.position;
            Vector3 startPos = moveTip.transform.position;
            Vector3 midPos = (startPos + targetPos) / 2 + new Vector3(0, dist / 4, 0);
            var attackPath = new[]
            {
                startPos,
                midPos,
                targetPos,
            };

            var attackTween = moveTip.transform.DOPath(attackPath, duration * 0.7f, PathType.CatmullRom)
                .SetEase(Ease.OutQuad);
            Sequence seq = null;
            bool flag = true;
            rootSource.SetWeight(0, 0.5f);
            root.data.sourceObjects = rootSource;
            attackTween
            .OnUpdate(() =>
            {
                float progress = attackTween.ElapsedPercentage();
                for (int i = 0; i < length; i++)
                {
                    float threshold = (i + 1) / (float)length * 0.3f;
                    if (_hidden[i] && progress >= threshold)
                    {
                        sandwormBody[i].DOFade(1, 0.3f / length).SetEase(Ease.Linear);
                        _hidden[i] = false;
                    }
                }
                
                if (flag && progress >= 0.9f)
                {
                    flag = false;
                    SmoothBlendSource(root.data.sourceObjects, root, 0, 1f, 0.15f);
                }
            })
            .OnComplete(() =>
            {
                seq = DOTween.Sequence();
                for (int i = 0; i < length; i++)
                {
                    int index = i;
                    seq.Append(
                        sandwormBody[index]
                            .DOFade(0, 0.5f / length)
                            .SetEase(Ease.Linear)
                            .OnComplete(() => _hidden[index] = true)
                    );
                }
            });

            yield return attackTween.WaitForCompletion();
            yield return seq.WaitForCompletion();
            yield return new WaitForSeconds(duration * 0.3f);
            rootSource.SetWeight(0, 0f);
            root.data.sourceObjects = rootSource;
            transform.position = targetPos;
            root.transform.position = transform.TransformPoint(new Vector3(0, 0, 0));
            moveTip.transform.position = transform.TransformPoint(new Vector3(0, 0, 0));
            bodySource1.SetWeight(2, 0.1f);
            bodyConstraint1.data.sourceObjects = bodySource1;
            bodySource2.SetWeight(2, 0.1f);
            bodyConstraint2.data.sourceObjects = bodySource2;
            yield return null;
        }


        public IEnumerator HideOrShow(bool hide, float duration)
        {
            isIdle = false;
            Vector3 targetPos = SceneContext.Character.transform.position;
            Vector3 dir = (targetPos - root.transform.position).normalized;
            int length = sandwormBody.Count;
            
            if (hide)
            {
                float start = head.data.sourceObjects.GetWeight(0);
                var seq = DOTween.Sequence();
                seq.Append(DOTween.To(() => start, w =>
                {
                    var s = head.data.sourceObjects;
                    s.SetWeight(0, w);
                    s.SetWeight(1, 1 - w);
                    head.data.sourceObjects = s;
                    start = w;
                }, 0, duration / 8).SetEase(Ease.OutQuad));
                
                seq.Append(bodyTip1.transform.DOMove(transform.position, duration / 8).SetEase(Ease.OutQuad));
                seq.Join(bodyTip2.transform.DOMove(transform.position, duration / 8).SetEase(Ease.OutQuad));

                seq.AppendInterval(duration / 8);
                
                for (int i = 0; i < length; i++)
                {
                    int idx = length - 1 - i;
                    seq.Append(sandwormBody[idx].DOFade(0, duration / 4 / length).SetEase(Ease.OutQuad));
                    _hidden[idx] = true;
                }

                seq.AppendInterval(duration / 8 * 3);

                yield return seq.WaitForCompletion();
                var s = head.data.sourceObjects;
                s.SetWeight(0, 1);
                s.SetWeight(1, 0);
                head.data.sourceObjects = s;
                yield return null;
            }
            else
            {
                isIdle = true;
                FacePlayer(true);
                isIdle = false;
                Vector3 midPos = (moveTip.transform.position + transform.TransformPoint(new Vector3(0, 2.4f, 0)) + dir * 1f) / 2 - dir;
                moveTip.transform.position = transform.position;
                var tween = moveTip.transform.DOPath(
                    new[]
                    {
                        moveTip.transform.position,
                        midPos,
                        transform.TransformPoint(new Vector3(0, 2.4f, 0)) + dir * 1f
                    },
                    duration
                    , PathType.CatmullRom
                );
                
                tween.SetEase(Ease.OutCubic)
                    .OnUpdate(() =>
                    {
                        float progress = tween.ElapsedPercentage();
                        
                        for (int i = 0; i < length; i++)
                        {
                            float threshold = (i + 1) / (float)length * 0.5f;
                            if (_hidden[i] && progress >= threshold)
                            {
                                sandwormBody[i].DOFade(1, 0.1f);
                                _hidden[i] = false;
                            }
                        }
                    });
                
                yield return tween.WaitForCompletion();
                
                isIdle = true;
            }
        }

        private void SmoothBlendSource(WeightedTransformArray array, MultiParentConstraint constraint,
            int index, float target, float duration)
        {
            float start = array.GetWeight(index);
            DOTween.To(() => start, w =>
            {
                var s = constraint.data.sourceObjects;
                s.SetWeight(index, w);
                constraint.data.sourceObjects = s;
                start = w;
            }, target, duration).SetUpdate(UpdateType.Late);
        }

        public void FacePlayer(bool hide = false)
        {
            if (!isIdle) return;
            
            Vector3 dir = (SceneContext.Character.transform.position - root.transform.position).normalized;
            Vector3 bodyDir = (SceneContext.Character.transform.position - transform.TransformPoint(new Vector3(0, 2.8f, 0))).normalized;
            int length = sandwormBody.Count;
            
            if (!hide)
            {
                moveTip.transform.position = transform.TransformPoint(new Vector3(0, 2.4f, 0)) + dir * 1f;
            }
            bodyTip1.transform.position = transform.TransformPoint(new Vector3(0, 2.8f, 0)) - bodyDir * 1f;
            bodyTip2.transform.position = transform.TransformPoint(new Vector3(0, 1.8f, 0)) - dir * 1f;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            if (angle is (>= 0f and < 15f) or ( >= 345f and < 360f))
            {
                sandwormBody[0].sprite = headSprites[0];
                for (int i = 0; i < length; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[0];
                    sandwormBody[i].sortingOrder = _orderCriteria - i - 1;
                }
            }
            else if (angle is >= 15f and < 60f)
            {
                sandwormBody[0].sprite = headSprites[1];
                for (int i = 0; i < length; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[1];
                    sandwormBody[i].sortingOrder = _orderCriteria - (length - i) + 1;
                }
            }
            else if (angle is >= 60f and < 120f)
            {
                sandwormBody[0].sprite = headSprites[2];
                for (int i = 0; i < sandwormBody.Count; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[1];
                    sandwormBody[i].sortingOrder = _orderCriteria - (length - i) + 1;
                }
            }
            else if (angle is >= 120f and < 165f)
            {
                sandwormBody[0].sprite = headSprites[3];
                for (int i = 0; i < sandwormBody.Count; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[1];
                    sandwormBody[i].sortingOrder = _orderCriteria - (length - i) + 1;
                }
            }
            else if (angle is >= 165f and < 195f)
            {
                sandwormBody[0].sprite = headSprites[4];
                for (int i = 0; i < sandwormBody.Count; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[2];
                    sandwormBody[i].sortingOrder = _orderCriteria - i - 1;
                }
            }
            else if (angle is >= 195f and < 240f)
            {
                sandwormBody[0].sprite = headSprites[5];
                for (int i = 0; i < sandwormBody.Count; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[3];
                    sandwormBody[i].sortingOrder = _orderCriteria - i - 1;
                }
            }
            else if (angle is >= 240f and < 300f)
            {
                sandwormBody[0].sprite = headSprites[6];
                for (int i = 0; i < sandwormBody.Count; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[3];
                    sandwormBody[i].sortingOrder = _orderCriteria - i - 1;
                }
            }
            else
            {
                sandwormBody[0].sprite = headSprites[7];
                for (int i = 0; i < sandwormBody.Count; i++)
                {
                    if (i != 0) sandwormBody[i].sprite = bodySprites[3];
                    sandwormBody[i].sortingOrder = _orderCriteria - i - 1;
                }
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
