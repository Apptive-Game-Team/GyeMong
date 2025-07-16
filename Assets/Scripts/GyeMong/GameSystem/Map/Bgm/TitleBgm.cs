using GyeMong.EventSystem.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Bmg
{
    public class TitleBgm : MonoBehaviour
    {
        [SerializeField] private string bgmName;

        private void Start()
        {
            BgmManager.Initialize();
            // StartCoroutine(TriggerEvents());
        }

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            yield return StartCoroutine((new BGMEvent(bgmName)).Execute());
        }
    }
}

