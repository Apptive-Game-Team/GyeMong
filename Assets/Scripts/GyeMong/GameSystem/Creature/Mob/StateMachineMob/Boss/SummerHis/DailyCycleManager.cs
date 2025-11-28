using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DailyCycleManager : MonoBehaviour
{
    public enum TimeOfDay { Dawn, Day, Dusk, Night }

    public float secondsPerFullDay = 120f;
    public float currentTimePercent;
    public Light2D globalLight2D;

    public Light2D sunLight2D;

    private float timePerSecond;

    [Header("태양 궤도")]
    public Vector2 sunOrbitCenter = Vector2.zero; // 맵 중심(원하는 좌표)
    private float sunOrbitRadiusX = 30f;           // 좌우 반지름
    private float sunOrbitRadiusY = 30f;           // 상하 반지름(타원 모양)
    private float sunMinFalloff = 0f;
    private float sunMaxFalloff = 40f;

    private Color dawnColor = new Color32(145, 145, 145, 255);
    private Color dayColor = new Color32(255, 255, 255, 255);
    private Color duskColor = new Color32(145, 145, 145, 255);
    private Color nightColor = new Color32(30, 30, 30, 255);

    public TimeOfDay currentTime;
    //public DailyCycleIndicatorUi dailyCycleIndicatorUi;

    private void Start()
    {
        currentTimePercent = 0.75f;
        timePerSecond = 1f / secondsPerFullDay;
    }

    public IEnumerator DayCycleRoutine()
    {
        while (true)
        {
            currentTimePercent += timePerSecond * Time.deltaTime;
            if (currentTimePercent >= 1f)
                currentTimePercent -= 1f;

            UpdateLighting();
            UpdateTime();
            UpdateSun();
            //dailyCycleIndicatorUi.UpdateClock(currentTimePercent);

            yield return null;
        }
    }

    void UpdateTime()
    {
        TimeOfDay newPhase;

        if (currentTimePercent < 0.25f) newPhase = TimeOfDay.Dawn;
        else if (currentTimePercent < 0.5f) newPhase = TimeOfDay.Day;
        else if (currentTimePercent < 0.75f) newPhase = TimeOfDay.Dusk;
        else newPhase = TimeOfDay.Night;

        if (newPhase != currentTime)
        {
            currentTime = newPhase;
        }
    }
    void UpdateLighting()
    {
        Color targetColor;

        if (currentTimePercent < 0.25f)
        {
            float t = currentTimePercent / 0.25f;
            targetColor = Color.Lerp(dawnColor, dayColor, t);
        }
        else if (currentTimePercent < 0.5f)
        {
            float t = (currentTimePercent - 0.25f) / 0.25f;
            targetColor = Color.Lerp(dayColor, duskColor, t);
        }
        else if (currentTimePercent < 0.75f)
        {
            float t = (currentTimePercent - 0.5f) / 0.25f;
            targetColor = Color.Lerp(duskColor, nightColor, t);
        }
        else
        {
            float t = (currentTimePercent - 0.75f) / 0.25f;
            targetColor = Color.Lerp(nightColor, dawnColor, t);
        }

        globalLight2D.color = targetColor;
    }
    void UpdateSun()
    {
        if (sunLight2D == null) return;

        // 0~1 → 0~2π (라디안) 변환
        float angle = currentTimePercent * Mathf.PI * 2f;
        angle = Mathf.Repeat(angle, Mathf.PI * 2f); // 혹시 모를 범위 초과 방지

        // 태양 궤도 위치(원/타원)
        float x = sunOrbitCenter.x + Mathf.Cos(angle) * sunOrbitRadiusX;
        float y = sunOrbitCenter.y + Mathf.Sin(angle) * sunOrbitRadiusY;

        Vector3 pos = sunLight2D.transform.position;
        pos.x = x;
        pos.y = y;
        sunLight2D.transform.position = pos;

        float radius;

        // 0 ~ π 구간: 낮 (동 → 북 → 서)
        if (angle <= Mathf.PI)
        {
            // 동(0)에서 서(π)까지를 0~1로 정규화
            float dayPhase = angle / Mathf.PI;      // 0(동) → 0.5(북) → 1(서)

            // 0→1→0 형태의 곡선: sin(dayPhase * π)
            float curve = Mathf.Sin(dayPhase * Mathf.PI); // 동 0, 북 1, 서 0

            radius = Mathf.Lerp(sunMinFalloff, sunMaxFalloff, curve);
        }
        // π ~ 2π 구간: 밤 (반지름 0 유지)
        else
        {
            radius = sunMinFalloff;
        }

        // Point Light 실제 범위
        sunLight2D.pointLightOuterRadius = radius;

        // 밝기는 4로 고정
        sunLight2D.intensity = 4f;
    }
}

