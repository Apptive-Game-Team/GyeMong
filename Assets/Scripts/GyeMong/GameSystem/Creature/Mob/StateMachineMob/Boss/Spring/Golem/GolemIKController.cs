using System;
using System.Collections;
using UnityEngine;
using Anima2D;
using GyeMong.SoundSystem;
namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    public enum HandSpriteID
    {
        Idle = 0,
        Down = 1,
        Up = 2,
        Fist = 3
    }
    public enum HandSide
    {
        None = 0,
        Left = 1,
        Right = 2
    }
    public class GolemIKController : MonoBehaviour
    {
        [Header("IK Targets")]
        [SerializeField] private GameObject ikRight;
        [SerializeField] private GameObject ikRArm;
        [SerializeField] private GameObject ikRHand;
        [SerializeField] private GameObject ikLeft;
        [SerializeField] private GameObject ikLArm;
        [SerializeField] private GameObject ikLHand;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        [Header("Changed Sprite")]
        [SerializeField] private SpriteRenderer lHandSpriteRenderer;
        [SerializeField] private SpriteRenderer rHandSpriteRenderer;
        [SerializeField] private SpriteRenderer headSpriteRenderer;
        [SerializeField] private Sprite[] lHandSprites;
        [SerializeField] private Sprite[] rHandSprites;
        [SerializeField] private Sprite[] headSprites;

        private Vector3 rightIdlePos;
        private Vector3 rArmIdlePos;
        private Vector3 rHandIdlePos;
        private Vector3 leftIdlePos;
        private Vector3 lArmIdlePos;
        private Vector3 lHandIdlePos;

        //캐싱
        private void Awake()
        {
            rightIdlePos = ikRight.transform.position;
            rArmIdlePos = ikRArm.transform.position;
            rHandIdlePos = ikRHand.transform.position;
            leftIdlePos = ikLeft.transform.position;
            lArmIdlePos = ikLArm.transform.position;
            lHandIdlePos = ikLHand.transform.position;
        }

        //테스트
        /*private void Start()
        {
            StartCoroutine(IdleStateAnimation());
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StopAllCoroutines();
                StartCoroutine(HandUpDownLoop());
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                StopAllCoroutines();
                StartCoroutine(HandAlternateUpDownLoop());
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                StopAllCoroutines();
                StartCoroutine(HandSmash());
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                StopAllCoroutines();
                StartCoroutine(DefenseStance());
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                StopAllCoroutines();
                StartCoroutine(PushOutAttackAnimation());
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                StopAllCoroutines();
                StartCoroutine(UpStoneAnimation());
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                StopAllCoroutines();
                StartCoroutine(FallingCubekAnimation());
            }
        }*/
        //보조 매서드
        public void MoveIKToPosition(GameObject target, Vector3 targetPos, float duration = 0.3f)
        {
            StartCoroutine(MoveIKCoroutine(target, targetPos, duration));
        }

        private IEnumerator MoveIKCoroutine(GameObject target, Vector3 targetPos, float duration)
        {
            Vector3 startPos = target.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                target.transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            target.transform.position = targetPos; //도착 위치 보정
        }
        private void SetHandSprite(HandSide side, HandSpriteID state)
        {
            if (side == HandSide.Left)
            {
                if (lHandSprites.Length > (int)state)
                    lHandSpriteRenderer.sprite = lHandSprites[(int)state];
            }
            else if (side == HandSide.Right)
            {
                if (rHandSprites.Length > (int)state)
                    rHandSpriteRenderer.sprite = rHandSprites[(int)state];
            }
        }
        private void SetHeadSprite(int idx)
        {
            headSpriteRenderer.sprite = headSprites[idx];
        }
        private Vector3 GetIKPos(GameObject target)
        {
            return target.transform.position;
        }
        //상태 초기화 매서드
        public void ResetToIdle()
        {
            MoveIKToPosition(ikRight, rightIdlePos);
            MoveIKToPosition(ikRArm, rArmIdlePos);
            MoveIKToPosition(ikRHand, rHandIdlePos);
            MoveIKToPosition(ikLeft, leftIdlePos);
            MoveIKToPosition(ikLArm, lArmIdlePos);
            MoveIKToPosition(ikLHand, lHandIdlePos);

            SetHandSprite(HandSide.Left, HandSpriteID.Idle);
            SetHandSprite(HandSide.Right, HandSpriteID.Idle);
            SetHeadSprite(2);

            StartCoroutine(IdleStateAnimation());
        }
        // 행동 호출
        public void StopAnimation()
        {
            StopAllCoroutines();
        }
        public void StartIdleAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(IdleStateAnimation());
        }
        public void DownAnimation()
        {
            StopAllCoroutines();
            SetHandSprite(HandSide.Left, HandSpriteID.Idle);
            SetHandSprite(HandSide.Right, HandSpriteID.Idle);
            SetHeadSprite(0);
        }
        public void CallAnimation(string animName)
        {
            StopAllCoroutines();

            switch (animName)
            {
                case "Idle":
                    StartCoroutine(IdleStateAnimation());
                    break;
                case "HandUpDown":
                    StartCoroutine(HandUpDownLoop());
                    break;
                case "HandAlternateUpDown":
                    StartCoroutine(HandAlternateUpDownLoop());
                    break;
                case "HandSmash":
                    StartCoroutine(HandSmash());
                    break;
                case "DefenseStance":
                    StartCoroutine(DefenseStance());
                    break;
                case "PushOutAttack":
                    StartCoroutine(PushOutAttackAnimation());
                    break;
                case "UpStone":
                    StartCoroutine(UpStoneAnimation());
                    break;
                case "FallingCube":
                    StartCoroutine(FallingCubekAnimation());
                    break;
                case "Down":
                    DownAnimation();
                    break;
                default:
                    Debug.LogWarning($"Animation '{animName}' not found!");
                    break;
            }
        }
        // 행동 정의
        public IEnumerator IdleStateAnimation()
        {
            //기본 애니메이션
            float amplitude = 0.1f;
            float frequency = 0.5f; //초당 무브번트 횟수
            float elapsed = 0f;

            while (true)
            {
                elapsed += Time.deltaTime;

                float rOffset = Mathf.Sin(elapsed * frequency * Mathf.PI * 2f) * amplitude;
                float lOffset = Mathf.Sin(elapsed * frequency * Mathf.PI * 2f) * amplitude;

                ikRight.transform.position = rightIdlePos + Vector3.up * rOffset;
                ikLeft.transform.position = leftIdlePos + Vector3.up * lOffset;

                yield return null;
            }
        }
        public IEnumerator HandUpDownLoop(float duration = 2f, float amplitude = 0.5f, int frequency = 2)
        {
            //양손 위아래
            SetHandSprite(HandSide.Left, HandSpriteID.Fist);
            SetHandSprite(HandSide.Right, HandSpriteID.Fist);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float offset = (Mathf.Sin(elapsed * frequency * Mathf.PI * 2) + 1) * amplitude;
                ikRight.transform.position = rightIdlePos + Vector3.up * offset;
                ikLeft.transform.position = leftIdlePos + Vector3.up * offset;
                yield return null;
            }
            ResetToIdle();
        }
        public IEnumerator HandAlternateUpDownLoop(float duration = 2f, float amplitude = 0.5f, int frequency = 2)
        {
            //양손 교대로 위아래
            SetHandSprite(HandSide.Left, HandSpriteID.Fist);
            SetHandSprite(HandSide.Right, HandSpriteID.Fist);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float rOffset = (Mathf.Sin(elapsed * frequency * Mathf.PI * 2) + 1) * amplitude;
                float lOffset = (Mathf.Sin(elapsed * frequency * Mathf.PI * 2 + Mathf.PI) + 1) * amplitude;
                ikRight.transform.position = rightIdlePos + Vector3.up * rOffset;
                ikLeft.transform.position = leftIdlePos + Vector3.up * lOffset;
                yield return null;
            }
            ResetToIdle();
        }
        public IEnumerator HandSmash(float duration = 1f, float distance = 1f)
        {
            // 양 손 쿵 찍기
            SetHandSprite(HandSide.Left, HandSpriteID.Fist);
            SetHandSprite(HandSide.Right, HandSpriteID.Fist);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float offset = Mathf.Sin(t * Mathf.PI) * distance;

                ikRight.transform.position = rightIdlePos + Vector3.up * offset;
                ikLeft.transform.position = leftIdlePos + Vector3.up * offset;

                yield return null;
            }
            Sound.Play("ENEMY_Rock_Falled");
            yield return new WaitForSeconds(0.3f);
            ResetToIdle();
        }
        public IEnumerator DefenseStance(float duration = 0.5f, float distance = 0.5f)
        {
            //앞 막기
            SetHandSprite(HandSide.Left, HandSpriteID.Fist);
            SetHandSprite(HandSide.Right, HandSpriteID.Fist);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                ikRHand.transform.position = Vector3.Lerp(rHandIdlePos, rHandIdlePos + Vector3.up * distance, t);
                ikLHand.transform.position = Vector3.Lerp(lHandIdlePos, lHandIdlePos + Vector3.up * distance, t);
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
            ResetToIdle();
        }
        public IEnumerator UpStoneAnimation(float duration = 1f, float distance = 10f)
        {
            // 왼손 찍기
            SetHandSprite(HandSide.Left, HandSpriteID.Down);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                ikLHand.transform.position = lHandIdlePos + Vector3.up * t * distance;
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);

            elapsed = 0f;
            float downDuration = duration / 10f;

            while (elapsed < downDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / downDuration;
                ikLHand.transform.position = lHandIdlePos + Vector3.up * (1 - t) * distance;
                yield return null;
            }
            Sound.Play("ENEMY_Rock_Falled");
            yield return new WaitForSeconds(0.3f);
            ResetToIdle();
        }
        public IEnumerator PushOutAttackAnimation(float duration = 1f, float distance = 10f)
        {
            // 오른손 찍기

            float upDuration = duration;           // 짧게 (빠르게 올림)
            float downDuration = duration / 10f; // 길게 (느리게 내림)

            float elapsed = 0f;

            // 올리는 구간
            while (elapsed < upDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / upDuration);
                ikRHand.transform.position = rHandIdlePos + Vector3.up * t * distance;
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);

            // 내려오는 구간
            elapsed = 0f;
            while (elapsed < downDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / downDuration);
                ikRHand.transform.position = rHandIdlePos + Vector3.up * (1 - t) * distance;
                yield return null;
            }
            SetHandSprite(HandSide.Right, HandSpriteID.Down);
            Sound.Play("ENEMY_Rock_Falled");
            yield return new WaitForSeconds(0.3f);
            ResetToIdle();
        }
        public IEnumerator FallingCubekAnimation(float duration = 0.5f, float distance = 10f)
        {
            // 오른손 들기
            SetHandSprite(HandSide.Right, HandSpriteID.Up);
            float elapsed = 0f;

            while (elapsed < duration / 5)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                ikRHand.transform.position = rHandIdlePos + Vector3.up * t * distance;

                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
            ResetToIdle();
        }
    }
}
