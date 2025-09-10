using DG.Tweening;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Movement
{
    public class BoomerangMovement : IAttackObjectMovement
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _duration;
        private float _curveAmount;
        
        public BoomerangMovement(Vector3 startPosition, Vector3 targetPosition, float speed, float curveAmount)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _curveAmount = curveAmount;
            float distance = Vector3.Distance(_startPosition, _targetPosition);
            _duration = distance / speed;
        }

        public Vector3? GetPosition(float time)
        {
            if (time > _duration) return null;
            if (_duration <= Mathf.Epsilon) return _startPosition;

            // 지름(코드) 벡터
            Vector3 chord = _targetPosition - _startPosition;
            float dist = chord.magnitude;
            if (dist <= Mathf.Epsilon) return _startPosition; // 같은 점이면 이동 없음

            // 지름 방향 단위벡터(스타트→타겟)
            Vector3 dir = chord / dist;

            // 2D 탑다운(XY) 기준의 수직 벡터 (시계/반시계는 부호로 결정)
            Vector3 perp = new Vector3(-dir.y, dir.x, 0f);

            // 원의 중심과 반지름
            Vector3 center = (_startPosition + _targetPosition) * 0.5f;
            float r = dist * 0.5f;

            // 0~1 정규화 → 0~2π 각도
            float s = Mathf.Clamp01(time / _duration);
            float theta = s * Mathf.PI * 2f;

            // 시계/반시계 선택 (true면 시계방향)
            bool clockwise = false;
            float sign = clockwise ? -1f : 1f;

            // 원의 파라메트릭 방정식:
            // center + (-dir * cosθ + sign * perp * sinθ) * r
            Vector3 offset = (-dir * Mathf.Cos(theta) + sign * perp * Mathf.Sin(theta)) * r;

            return center + offset;
        }
    }
}