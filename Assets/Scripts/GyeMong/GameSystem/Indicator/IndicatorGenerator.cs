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
            Vector2 halfSize = new Vector2(spriteSize.x * scale.x, spriteSize.y * scale.y) * 0.5f;

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
            float scale = Mathf.Max(attackObject.transform.lossyScale.x, attackObject.transform.lossyScale.y);
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
            
            Vector3 desiredSize = capsule.direction == CapsuleDirection2D.Horizontal ? 
                new Vector3(capsule.size.y, capsule.size.x, 1f) : new Vector3(capsule.size.x, capsule.size.y, 1f);

            Vector3 scaledSize = Vector3.Scale(desiredSize, attackObject.transform.lossyScale);
            
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
            Collider2D col = attackObject.GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogWarning("Collider2D not found");
                yield return null;
            }

            var type = col.GetType();
            if (_shapeMap.TryGetValue(type, out var shape))
            {
                GameObject indicator = shape.CreateIndicator(attackObject, pos, rot);
                Indicator flicker = indicator.GetComponent<Indicator>();
                if (flicker == null) flicker = indicator.AddComponent<Indicator>();
                StartCoroutine(flicker.Flick(duration));
                yield return new WaitForSeconds(duration);
                action?.Invoke();
            }
            else
            {
                Debug.LogWarning($"No provider registered for {type}");
                yield return null;
            }
        }
        
        public IEnumerator GenerateIndicator(AttackObjectController attackObjectController, float duration)
        {
            return GenerateIndicator(
                attackObjectController.gameObject,
                attackObjectController.transform.position,
                attackObjectController.transform.rotation * Quaternion.Euler(0, 0, 90f),
                duration,
                () => attackObjectController.StartRoutine()
            );
        }
    }
}
