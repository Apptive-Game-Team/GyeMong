using System.Collections;
using UnityEngine;
using DG.Tweening;
using GyeMong.SoundSystem;
using UnityEngine.Serialization;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class SpringBeachWave : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] waves;
        [SerializeField] private float waveDuration = 10f;

        private void Start()
        {
            var waveSound = Sound.Play("EFFECT_Spring_Wave", true);
            StartCoroutine(WaveAnimation());
        }

        private IEnumerator WaveAnimation()
        {
            while (true)
            {
                Tween t1 = transform.DOMove(new Vector3(0, -0.57f, 0) * 3, waveDuration * 1.5f)
                    .SetEase(Ease.InOutSine);

                bool waveOut1 = false;
                bool waveOut2 = false;
                
                t1.OnUpdate(() =>
                {
                    float p = t1.ElapsedPercentage();
                    if (p > 0.15f && !waveOut1)
                    {
                        waveOut1 = true;
                        float endP = 0.5f;
                        float remain = Mathf.Max(0f, endP - p);
                        float dur = remain * waveDuration;
                        waves[1].DOFade(0, dur);
                    }
                    
                    if (p > 0.5f && !waveOut2)
                    {
                        waveOut2 = true;
                        float endP = 0.85f;
                        float remain = Mathf.Max(0f, endP - p);
                        float dur = remain * waveDuration;
                        waves[0].DOFade(0, dur);
                    }
                });
                yield return t1.WaitForCompletion();
                yield return new WaitForSeconds(1f);
                
                
                Tween t2 = transform.DOMove(Vector3.zero, waveDuration)
                    .SetEase(Ease.InOutSine);

                bool waveIn1 = false;
                bool waveIn2 = false;
                
                t2.OnUpdate(() =>
                {
                    float p = t2.ElapsedPercentage();
                    if (p > 0.35f && !waveIn1)
                    {
                        waveIn1 = true;
                        float endP = 0.7f;
                        float remain = Mathf.Max(0f, endP - p);
                        float dur = remain * waveDuration;
                        waves[0].DOFade(1, dur);
                    }
                    if (p > 0.7f && !waveIn2)
                    {
                        waveIn2 = true;
                        float endP = 1f;
                        float remain = Mathf.Max(0f, endP - p);
                        float dur = remain * waveDuration;
                        waves[1].DOFade(1, dur);
                    }
                });
                yield return t2.WaitForCompletion();
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
