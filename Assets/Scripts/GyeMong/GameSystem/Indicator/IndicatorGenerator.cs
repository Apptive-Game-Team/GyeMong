using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Attack;
using UnityEngine;
using Util;

namespace GyeMong.GameSystem.Indicator
{
    public class BoxCollider : IIndicatorShape
    {
        private const string ROUNDED_SHADER_PATH = "Shader/RoundedIndicator";

        private GameObject _boxObject;
        private float _cornerRoundness;

        public BoxCollider(GameObject boxObject, float cornerRoundness = 0f)
        {
            _boxObject = boxObject;
            _cornerRoundness = cornerRoundness;
        }

        public GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot)
        {
            var box = attackObject.GetComponent<BoxCollider2D>();
            if (box == null) return null;

            var indicator = UnityEngine.Object.Instantiate(_boxObject);
            indicator.transform.position = pos 
                                           + attackObject.transform.rotation * Vector3.Scale(box.offset, attackObject.transform.lossyScale);
            indicator.transform.rotation = rot;
            indicator.transform.localScale = Vector3.Scale(new Vector3(box.size.x, box.size.y, 1f), attackObject.transform.lossyScale);

            ApplyRoundedCorners(indicator);

            return indicator;
        }

        /// <summary>
        /// 사각 인디케이터는 콜라이더 크기에 맞춰 비균등 스케일로 늘어나므로,
        /// 라운드 처리된 스프라이트를 쓰면 모서리 반경이 찌그러진다.
        /// 반경을 월드 단위로 받는 셰이더로 그린다.
        /// </summary>
        private void ApplyRoundedCorners(GameObject indicator)
        {
            if (_cornerRoundness <= 0f) return;

            var renderer = indicator.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null) return;

            var shader = Resources.Load<Shader>(ROUNDED_SHADER_PATH);
            if (shader == null)
            {
                Debug.LogWarning($"Shader not found: {ROUNDED_SHADER_PATH}");
                return;
            }

            Vector2 spriteSize = renderer.sprite.bounds.size;
            Vector3 scale = indicator.transform.lossyScale;
            // 공격 오브젝트가 좌우 반전(음수 스케일)일 수 있다. 셰이더의 _HalfSize 는
            // 절댓값 반폭이어야 모서리 마스크가 뒤집히지 않는다.
            Vector2 halfSize = new Vector2(
                Mathf.Abs(spriteSize.x * scale.x),
                Mathf.Abs(spriteSize.y * scale.y)) * 0.5f;

            var material = new Material(shader);
            material.SetVector("_HalfSize", new Vector4(halfSize.x, halfSize.y, 0f, 0f));
            material.SetFloat("_CornerRadius", Mathf.Min(halfSize.x, halfSize.y) * _cornerRoundness);
            renderer.material = material;

            indicator.AddComponent<Indicator>().SetOwnedMaterial(material);
        }
    }

    public class CircleCollider : IIndicatorShape
    {
        private GameObject _circleObject;
        public CircleCollider(GameObject circleObject) => _circleObject = circleObject;
        public GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot)
        {
            var circle = attackObject.GetComponent<CircleCollider2D>();
            if (circle == null) return null;

            var indicator = UnityEngine.Object.Instantiate(_circleObject);
            indicator.transform.position = pos
                                           + attackObject.transform.rotation * Vector3.Scale(circle.offset, attackObject.transform.lossyScale);
            indicator.transform.rotation = rot;

            float diameter = circle.radius * 2f;
            // CircleCollider2D 의 실제 반지름은 축별 스케일 절댓값 중 큰 값을 따른다.
            float scale = Mathf.Max(
                Mathf.Abs(attackObject.transform.lossyScale.x),
                Mathf.Abs(attackObject.transform.lossyScale.y));
            indicator.transform.localScale = new Vector3(diameter * scale, diameter * scale, 1f);

            return indicator; 
        }
    }

    public class CapsuleCollider : IIndicatorShape
    {
        private GameObject _capsuleObject;
        public CapsuleCollider(GameObject capsuleObject) => _capsuleObject = capsuleObject;
        public GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot)
        {
            var capsule = attackObject.GetComponent<CapsuleCollider2D>();
            if (capsule == null) return null;

            var indicator = UnityEngine.Object.Instantiate(_capsuleObject);
            indicator.transform.position = pos
                                           + attackObject.transform.rotation * Vector3.Scale(capsule.offset, attackObject.transform.lossyScale);
            indicator.transform.rotation = rot * Quaternion.Euler(0f, 0f, 90f);

            SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
            Vector2 spriteSize = sr.sprite.bounds.size;

            // 캡슐 스프라이트는 긴 축이 가로로 그려져 있어 90도 돌려서 쓴다.
            // 그만큼 표시의 로컬 X 는 콜라이더의 Y 축에, 로컬 Y 는 X 축에 놓이므로
            // 크기도 같이 바꿔 넣어야 한다. CapsuleCollider2D 의 size 는 direction 과
            // 무관하게 (가로, 세로) 경계 상자이므로 direction 으로 분기하지 않는다.
            Vector3 lossyScale = attackObject.transform.lossyScale;
            Vector3 scaledSize = new Vector3(
                Mathf.Abs(capsule.size.y * lossyScale.y),
                Mathf.Abs(capsule.size.x * lossyScale.x),
                1f);

            Vector3 scale = new Vector3(
                scaledSize.x / spriteSize.x,
                scaledSize.y / spriteSize.y,
                1f
            );

            indicator.transform.localScale = scale;

            return indicator;
        }
    }
    
    public class IndicatorGenerator : SingletonObject<IndicatorGenerator>
    {
        [SerializeField] private GameObject boxPrefab;
        [SerializeField, Range(0f, 1f)] private float boxCornerRoundness = 0.35f;
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private GameObject capsulePrefab;
        
        private Dictionary<Type, IIndicatorShape> _shapeMap = new();

        protected override void Awake()
        {
            _shapeMap[typeof(BoxCollider2D)] = new BoxCollider(boxPrefab, boxCornerRoundness);
            _shapeMap[typeof(CircleCollider2D)] = new CircleCollider(circlePrefab);
            _shapeMap[typeof(CapsuleCollider2D)] = new CapsuleCollider(capsulePrefab);
        }
        

        public IEnumerator GenerateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot, float duration, Action action = null)
        {
            GameObject indicator = TryCreateIndicator(attackObject, pos, rot);
            if (indicator != null)
            {
                Indicator flicker = indicator.GetComponent<Indicator>();
                if (flicker == null) flicker = indicator.AddComponent<Indicator>();
                StartCoroutine(flicker.Flick(duration));
            }

            // 위험표시는 연출이고 공격은 게임 로직이다.
            // 표시를 못 만들어도 같은 시간을 기다린 뒤 공격은 그대로 실행한다.
            yield return new WaitForSeconds(duration);
            action?.Invoke();
        }

        private GameObject TryCreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot)
        {
            if (attackObject == null)
            {
                Debug.LogWarning("Attack object is null");
                return null;
            }

            Collider2D col = attackObject.GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogWarning($"Collider2D not found on {attackObject.name}");
                return null;
            }

            Type type = col.GetType();
            if (!_shapeMap.TryGetValue(type, out IIndicatorShape shape))
            {
                Debug.LogWarning($"No provider registered for {type}");
                return null;
            }

            GameObject indicator = shape.CreateIndicator(attackObject, pos, rot);
            if (indicator == null)
            {
                Debug.LogWarning($"Failed to create {type} indicator for {attackObject.name}");
            }
            return indicator;
        }

        public IEnumerator GenerateIndicator(AttackObjectController attackObjectController, float duration)
        {
            // 도형별 스프라이트 방향 보정은 각 IIndicatorShape 안에서 한다.
            // 여기서 임의로 90도를 더하면 캡슐은 이중 적용되고 사각형은 가로 세로가 뒤집힌다.
            return GenerateIndicator(
                attackObjectController.gameObject,
                attackObjectController.transform.position,
                attackObjectController.transform.rotation,
                duration,
                () => attackObjectController.StartRoutine()
            );
        }
    }
}
