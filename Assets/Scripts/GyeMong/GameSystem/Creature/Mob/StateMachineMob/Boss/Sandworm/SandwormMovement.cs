using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.SoundSystem;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class SandwormMovement : MonoBehaviour
    {
        private static readonly int BlinkTrigger = Shader.PropertyToID("_BlinkTrigger");

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
        [SerializeField] private List<Sprite> headRotateSprites;

        [Header("Sandworm Body")]
        public List<SpriteRenderer> sandwormBody;

        [Header("Head Attack Move Value")] 
        [SerializeField] private float bodyRotateValue;

        [Header("Movement Effect")] 
        [SerializeField] private GameObject showEffectPrefab;
        [SerializeField] private ParticleSystem showEffectParticle;

        private Tween _idleTween;
        public bool isIdle = true;
        private bool _check = true;
        private bool[] _hidden = new bool[13];
        private int _orderCriteria;

        private void Awake()
        {
            _orderCriteria = SceneContext.Character.GetComponent<SpriteRenderer>().sortingOrder + sandwormBody.Count;
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
            Vector3 originalBodyPos = bodyTip1.transform.position;
            Vector3 originalHeadPos = moveTip.transform.position;
            
            Vector3 rootDir = (dest - root.transform.position).normalized;
            float rootDist = Vector3.Magnitude(dest - root.transform.position);
            float angle = dest.x > originalBodyPos.x ? -bodyRotateValue : bodyRotateValue;
            
            int spriteIdx = HeadRotateIndex(rootDir);
            Sprite originSprite = sandwormBody[0].sprite;

            Sequence seq = DOTween.Sequence();
            seq.Append(bodyTip1.transform.DOMove(originalBodyPos - rootDir / 2, preDelay).SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    if (spriteIdx != -1)
                    {
                        sandwormBody[0].sprite = headRotateSprites[spriteIdx];
                    }
                }));
            seq.Join(moveTip.transform.DORotate(new Vector3(0,0, angle), preDelay).SetEase(Ease.Linear));

            Vector3 startPos = moveTip.transform.position;
            Vector3 endPos = dest;
            float dist = Vector3.Distance(startPos, endPos);
            Vector3 midPos = (startPos + endPos) / 2 + Vector3.up * dist / 4;
            
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

            float halfway = preDelay + dur * 0.5f;
            seq.InsertCallback(halfway, () => 
            {
                if (spriteIdx != -1)
                {
                    sandwormBody[0].sprite = originSprite;
                }
            });
            
            yield return seq.WaitForCompletion();
        }

        public IEnumerator AttackMove(Vector3 playerDir, bool dir, float upAngle, float upTime, float downAngle, float downTime, float recoverTime, float recoverAngle = 0f)
        {
            int spriteIdx = HeadRotateIndex(playerDir);
            float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            Sprite originSprite = sandwormBody[0].sprite;
            
            var seq = DOTween.Sequence();
            Tween t1, t2, t3;

            if (angle is (>= 240f and < 300f) or (>= 60f and < 120f))
            {
                Vector3 originPos = moveTip.transform.position;
                
                t1 = moveTip.transform
                    .DOMove(originPos - playerDir * 0.15f, upTime)
                    .SetEase(Ease.OutSine)
                    .OnStart(() =>
                    {
                        if (spriteIdx != -1)
                        {
                            sandwormBody[0].sprite = headRotateSprites[spriteIdx];
                        }
                    });

                t2 = moveTip.transform
                    .DOMove(originPos + playerDir * 0.2f, downTime)
                    .SetEase(Ease.InQuad);
                
                t3 = moveTip.transform
                    .DOMove(originPos, recoverTime)
                    .SetEase(Ease.Linear)
                    .OnStart(() =>
                    {
                        if (spriteIdx != -1)
                        {
                            sandwormBody[0].sprite = headRotateSprites[spriteIdx];
                        }
                    });
            }
            else
            {
                t1 = headRotateTip.transform
                    .DORotate(new Vector3(0, 0, dir ? upAngle : -upAngle), upTime)
                    .SetEase(Ease.OutSine)
                    .OnStart(() =>
                    {
                        if (spriteIdx != -1)
                        {
                            sandwormBody[0].sprite = headRotateSprites[spriteIdx];
                        }
                    });

                t2 = headRotateTip.transform
                    .DORotate(new Vector3(0, 0, dir ? downAngle : -downAngle), downTime)
                    .SetEase(Ease.InQuad);

                t3 = headRotateTip.transform
                    .DORotate(new Vector3(0, 0, recoverAngle), recoverTime)
                    .SetEase(Ease.Linear)
                    .OnStart(() =>
                    {
                        if (spriteIdx != -1)
                        {
                            sandwormBody[0].sprite = headRotateSprites[spriteIdx];
                        }
                    });
            }

            seq.Append(t1).Append(t2).Append(t3);
            
            float halfway1 = upTime + downTime * 0.3f;
            float halfway2 = upTime + downTime + recoverTime * 0.7f;

            seq.InsertCallback(halfway1, () => 
            {
                if (spriteIdx != -1)
                {
                    sandwormBody[0].sprite = headRotateSprites[spriteIdx + 1];
                }
            });
            
            seq.InsertCallback(halfway2, () => 
            {
                if (spriteIdx != -1)
                {
                    sandwormBody[0].sprite = originSprite;
                }
            });



            yield return seq.WaitForCompletion();
        }

        private int HeadRotateIndex(Vector3 playerDir)
        {
            int spriteIdx;
            
            float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            if (angle is (>= 0f and < 15f) or (>= 345f and < 360f))
            {
                spriteIdx = 0;
            }
            else if (angle is >= 300f and < 345f)
            {
                spriteIdx = 2;
            }
            else if (angle is >= 240f and < 300f)
            {
                spriteIdx = 4;
            }
            else if (angle is >= 195f and < 240f)
            {
                spriteIdx = 6;
            }
            else if (angle is >= 165f and < 195f)
            {
                spriteIdx = 8;
            }
            else
            {
                spriteIdx = -1;
            }

            return spriteIdx;
        }

        public IEnumerator BodyAttackMove(Vector3 targetPos, float speed)
        {
            isIdle = true;
            FacePlayer(true);
            isIdle = false;
            
            int length = sandwormBody.Count;
            float dist = Vector3.Distance(root.transform.position, targetPos);
            float duration = dist / speed;
            
            moveTip.transform.position = transform.position;
            SetRigState(true);
            sandwormBody[0].GetComponent<Collider2D>().enabled = true;
            sandwormBody[0].GetComponent<AttackObjectController>().isAttacked = false;
            
            Sound.Play("ENEMY_Long_Burst_Action");
            showEffectParticle.Play();
            Instantiate(showEffectPrefab, root.transform.position + new Vector3(0, -0.6f, 0), Quaternion.identity);

            var attackTween = moveTip.transform.DOJump(targetPos, dist / 6, 1, duration)
                .SetEase(Ease.OutQuad);
            Sequence seq = null;
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
                            .OnComplete(() =>
                            {
                                _hidden[index] = true;
                                if (index == 0)
                                {
                                    sandwormBody[0].GetComponent<Collider2D>().enabled = false;
                                }
                            })
                    );
                }
            });

            yield return attackTween.WaitForCompletion();
            yield return seq.WaitForCompletion();
            yield return new WaitForSeconds(duration * 0.3f);
            
            SetRigState(false);
            transform.position = targetPos;
            root.transform.position = transform.TransformPoint(new Vector3(0, 0, 0));
            moveTip.transform.position = transform.TransformPoint(new Vector3(0, 0, 0));
            yield return null;
        }


        public IEnumerator HideOrShow(bool hide, float duration, bool fixedDirection = false)
        {
            isIdle = false;
            Vector3 targetPos = SceneContext.Character.transform.position;
            Vector3 dir = fixedDirection ? new Vector3(0, -1, 0) : (targetPos - root.transform.position).normalized;
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
                moveTip.transform.position = transform.position;
                var s = head.data.sourceObjects;
                s.SetWeight(0, 1);
                s.SetWeight(1, 0);
                head.data.sourceObjects = s;
                yield return null;
            }
            else
            {
                if (!fixedDirection)
                {
                    isIdle = true;
                    FacePlayer(true);
                    isIdle = false;
                }
                Vector3 midPos = (moveTip.transform.position + transform.TransformPoint(new Vector3(0, 2.4f, 0)) + dir * 1f) / 2 - dir;
                moveTip.transform.position = transform.position;

                Sound.Play("ENEMY_Short_Burst_Action");
                showEffectParticle.Play();
                Instantiate(showEffectPrefab, root.transform.position + new Vector3(0, -0.6f, 0), Quaternion.identity);
                
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

        public void FacePlayer(bool hide = false, bool fixedDirection = false)
        {
            if (!isIdle) return;
            
            Vector3 dir = fixedDirection ? new Vector3(0, -1, 0) :
                (SceneContext.Character.transform.position - root.transform.position).normalized;
            Vector3 bodyDir = fixedDirection ? new Vector3(0, -1, 0) :
                (SceneContext.Character.transform.position - transform.TransformPoint(new Vector3(0, 2.8f, 0))).normalized;
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

        public void SetBlink(float value)
        {
            foreach (var s in sandwormBody)
            {
                s.material.SetFloat(BlinkTrigger, value);
            }
        }

        private void SetRigState(bool damped)
        {
            foreach (var s in sandwormBody)
            {
                s.GetComponent<MultiParentConstraint>().weight = damped ? 0f : 1f;
                s.GetComponent<DampedTransform>().weight = damped ? 1f : 0f;
            }
        }

        public IEnumerator ChangeScreamImage(bool scream, float delay)
        {
            if (scream)
            {
                sandwormBody[0].sprite = headSprites[8];
                yield return new WaitForSeconds(delay);
                sandwormBody[0].sprite = headSprites[9];
            }
            else
            {
                sandwormBody[0].sprite = headSprites[8];
                yield return new WaitForSeconds(delay);
                sandwormBody[0].sprite = headSprites[6];
            }
        }
    }
}
