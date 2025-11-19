using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components
{
    public class SlimeHpBar : MonoBehaviour
    {
        public DivisionSlime slime;
        public float currentHp;
        
        private Slider _hpBar;
        private float _maxHp;
        
        private const float TIME = 0.3f;
        private const float FILL_TIME = 0.7f;
        private const float DELTA_TIME = 0.02f;

        private void Initialize()
        {
            _hpBar = GetComponentInChildren<Slider>();
            _maxHp = slime.GetTotalHp();
            Debug.Log(_maxHp);
            currentHp = 0;
            BindAction(slime);
        }

        public void BindAction(DivisionSlime divisionSlime)
        {
            divisionSlime.OnHpChanged += UpdateHpBar;
        }

        private void UpdateHpBar()
        {
            _hpBar.value = currentHp / _maxHp;
            if (currentHp <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        public IEnumerator ShowHpBarEvent()
        {
            gameObject.SetActive(true);
            Initialize();
            yield return DropHpBar();
            yield return ReboundHpBar();
            yield return FillHpBar();
        }
        
        private IEnumerator DropHpBar()
        {
            Vector3 defaultPosition = _hpBar.transform.position;
            Vector3 startPosition = defaultPosition + new Vector3(0, 100, 0);
            _hpBar.transform.position = startPosition;
            float timer = 0;
            while (timer < TIME)
            {
                timer += DELTA_TIME;
                float progress = Mathf.Pow(timer / TIME, 2);
                _hpBar.transform.position = Vector3.Lerp(startPosition, defaultPosition, progress);
                yield return new WaitForSeconds(DELTA_TIME);
            }
            _hpBar.transform.position = defaultPosition;
        }
        
        private IEnumerator ReboundHpBar()
        {
            Vector3 defaultPosition = _hpBar.transform.position;
            Vector3 reboundPosition = defaultPosition + new Vector3(0, 20, 0);
            float timer = 0;
            while (timer < TIME/2)
            {
                timer += DELTA_TIME;
                float progress = Mathf.Sin((timer / (TIME / 2)) * Mathf.PI);
                _hpBar.transform.position = Vector3.Lerp(defaultPosition, reboundPosition, progress);
                yield return new WaitForSeconds(DELTA_TIME);
            }
            _hpBar.transform.position = defaultPosition;
        }
        
        private IEnumerator FillHpBar()
        {
            float timer = 0;
            float progress = 0;
            while (timer < FILL_TIME && progress <= 1)
            {
                timer += DELTA_TIME;
                currentHp = _maxHp * progress;
                _hpBar.value = currentHp;
                progress = Mathf.Pow(timer / FILL_TIME, 2);
                yield return new WaitForSeconds(DELTA_TIME);
            }
            currentHp = _maxHp;
        }
    }
}
