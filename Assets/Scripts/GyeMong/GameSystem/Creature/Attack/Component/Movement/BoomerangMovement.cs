using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Movement
{
    public class BoomerangMovement : IAttackObjectMovement
    {
        private readonly Vector3 _start;
        private readonly Vector3 _target;
        private readonly float _totalDuration;
        private readonly float _curve;     // 직선 거리 대비 횡오프셋 비율 (예: 0.1f → 10%)
        private readonly float _midSlowK;  // 0~0.99 권장. 0이면 균일 속도, 0.8~0.9면 반환점에서 확 느려짐
        private readonly int _sign;        // +1/-1 : 좌/우로 휘는 방향

        public BoomerangMovement(
            Vector3 startPosition,
            Vector3 targetPosition,
            float speed,           // 직선 거리 기준 왕복 속도(편도 거리/속도 = 편도 시간)
            float curveAmount,     // 0.05~0.15 권장
            float midSlowK = 0.85f,// 중간 감속 강도
            bool clockwise = false // true면 시계방향(화살표 기준 오른쪽)으로 휨
        )
        {
            _start = startPosition;
            _target = targetPosition;
            _curve = Mathf.Max(0f, curveAmount);
            _midSlowK = Mathf.Clamp01(midSlowK);
            float dist = Vector3.Distance(_start, _target);
            _totalDuration = dist > Mathf.Epsilon ? (dist / Mathf.Max(1e-4f, speed)) * 2f : 0f; // 왕복
            _sign = clockwise ? -1 : 1;
        }

        public Vector3? GetPosition(float time)
        {
            if (_totalDuration <= Mathf.Epsilon) return _start;
            if (time > _totalDuration) return null;

            // s: 왕복 전체 구간 정규화 시간 [0,1]
            float s = Mathf.Clamp01(time / _totalDuration);

            // ── 1) 반환점에서 느려지는 재파라미터화 (중간(s=0.5)에서 속도 최소)
            // f(s) = s + k*sin(2πs)/(2π)  => f'(s) = 1 + k*cos(2πs)
            // s=0,1에서 속도 최대, s=0.5에서 최소
            float f = s + _midSlowK * Mathf.Sin(Mathf.PI * 2f * s) / (Mathf.PI * 2f);

            // ── 2) 0→1(가출)→0(귀환) 진행량 u
            float u = 1f - Mathf.Abs(2f * f - 1f); // 삼각파를 매끈하게 타임워핑한 값

            // 기본 직선 성분
            Vector3 chord = _target - _start;
            float dist = chord.magnitude;
            Vector3 dir = dist > Mathf.Epsilon ? chord / dist : Vector3.right;

            // XY 탑다운용 수직 벡터(시계/반시계는 _sign으로)
            Vector3 perp = new Vector3(-dir.y, dir.x, 0f) * _sign;

            // 직선 위 위치(왕복)
            Vector3 basePos = _start + dir * (u * dist);

            // 횡 오프셋: sin(πu) → 시작/반환점/끝에서 0, 중간에서 최대
            float amp = _curve * dist; 
            float legSign = (f < 0.5f) ? 1f : -1f; // 왕복 전반/후반에 따라 부호 전환
            Vector3 sideOffset = perp * (Mathf.Sin(Mathf.PI * u) * amp * legSign);

            Vector3 pos = basePos + sideOffset;
            pos.z = _start.z; // 2D면 Z 고정

            return pos;
        }

        public Vector3? GetDirection(float time)
        {
            return null;
        }
    }
}
