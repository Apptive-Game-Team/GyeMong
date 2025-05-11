using Gyemong.EventSystem;
using UnityEngine;

namespace Gyemong.GameSystem.Rune
{
    public class RuneGameObject : MonoBehaviour
    {
        [SerializeField] RuneData runeData;
        SpriteRenderer spriteRenderer;
        EventObject eventObject;

        public void TryInit(RuneData runeData)
        {
            if (runeData == null)
            {
                Debug.LogError("This RuneObject try to init, but it's runeData doesn't exist, so it destroyed itself.");
                Destroy(this);
            }

            Init(runeData);
        }

        private void Init(RuneData rune)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        
            SetRuneData(rune);
            SetSprite(runeData.runeImage);
        }

        private void SetRuneData(RuneData newData)
        {
            runeData = newData;
        }

        private void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
