using TMPro;
using UnityEngine;

namespace System.UI.Game.GoldUI
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
