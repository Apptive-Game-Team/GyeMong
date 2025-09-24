using GyeMong.GameSystem.Creature.Player.Interface.Listener;
using UnityEngine;
using System.Collections;
using Util.ChangeListener;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerDashCooltimeController : MonoBehaviour, IDashListener, IChangeListener<float>
    {
        [SerializeField] private Slider dashSlider;

        private Coroutine cooldownRoutine;

        private void Awake()
        {
            if (dashSlider == null) dashSlider = GetComponent<Slider>();
        }

        private void Start()
        {
            SceneContext.Character.changeListenerCaller.AddDashListener(this);
            if (dashSlider != null) dashSlider.value = 1f;
        }

        public void OnChanged(float value)
        {
            OnDashUsed(value);
        }

        public void OnDashUsed(float cooldown)
        {
            if (dashSlider == null) return;

            if (cooldownRoutine != null)
                StopCoroutine(cooldownRoutine);

            cooldownRoutine = StartCoroutine(DashCooldownRoutine(cooldown));
        }

        private IEnumerator DashCooldownRoutine(float cooldown)
        {
            float dashDuration = Mathf.Max(0.0001f, SceneContext.Character.stat.DashDuration);
            float t = 0f;

            while (t < dashDuration)
            {
                t += Time.deltaTime;
                dashSlider.value = Mathf.Lerp(1f, 0f, t / dashDuration);
                yield return null;
            }
            dashSlider.value = 0f;

            t = 0f;
            cooldown = Mathf.Max(0f, cooldown);
            while (t < cooldown)
            {
                t += Time.deltaTime;
                dashSlider.value = (cooldown <= 0f) ? 1f : Mathf.Lerp(0f, 1f, t / cooldown);
                yield return null;
            }
            dashSlider.value = 1f;

            cooldownRoutine = null;
        }
    }
}
