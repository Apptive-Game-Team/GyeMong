using Gyemong.StatusSystem.Gold;
using TMPro;
using UnityEngine;

namespace Gyemong.UISystem.Game.GoldUI
{
    public class RuneGoldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldText;

        private void Awake()
        {
            GoldManager.OnGoldChanged += ChangeText;
        }

        private void ChangeText(int amount)
        {
            goldText.text = amount.ToString();
        }
    }
}
