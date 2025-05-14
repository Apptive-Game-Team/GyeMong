using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.EventSystem.Event.Chat
{
    [CreateAssetMenu(fileName = "BackgroundImageData", menuName = "ScriptableObject/BackgroundImageData")]
    public class BackgroundImageData : ScriptableObject
    {
        [Serializable]
        public struct BackgroundImageInfo
        {
            public BackgroundImage backgroundImage;
            public Sprite image;
        }

        public List<BackgroundImageInfo> backgroundImages;

        public Sprite GetBackgroundImage(BackgroundImage backgroundImage)
        {
            foreach (var bg in backgroundImages)
            {
                if (bg.backgroundImage == backgroundImage)
                    return bg.image;
            }
            return null;
        }
    }
}

