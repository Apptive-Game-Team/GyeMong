using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Interface;

namespace Gyemong.GameSystem.Map.Puzzle.ImagePuzzle
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private Image _timeWatchImage;
        [SerializeField] private TextMeshProUGUI _timeWatch;
        private bool _isTimerRunning = false;
        public bool IsTimerRunning => _isTimerRunning;
        
        private Coroutine _timerCoroutine;

        public void Play(float time, ICallback callback)
        {
            if (_timerCoroutine == null)
            {
                _timerCoroutine = StartCoroutine(StartTimeWatchCoroutine(time, callback));
            }
        }
        
        public void Stop()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
            _timeWatchImage.gameObject.SetActive(false);
        }
        
        private IEnumerator StartTimeWatchCoroutine(float time, ICallback callback)
        {
            _timeWatchImage.gameObject.SetActive(true);
            float remainTime = time;

            while (remainTime > 0)
            {
                remainTime -= Time.deltaTime;
                _timeWatch.text = remainTime.ToString("F2");
                yield return null;
            }

            _timeWatchImage.gameObject.SetActive(false);

            callback.OnProcessCompleted();
            _timerCoroutine = null;
        }
    }
}
