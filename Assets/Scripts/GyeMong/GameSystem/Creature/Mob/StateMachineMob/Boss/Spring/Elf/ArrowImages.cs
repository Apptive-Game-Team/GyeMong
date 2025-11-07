using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElfArrowImages", menuName = "ScriptableObjects/ElfArrowImages")]
public class ArrowImages : ScriptableObject
{
    [Serializable]
    public class SeedArrow
    {
        [Header("Æø¹ßÈ­»ì")]
        public Sprite seedArrowImage;
    }

    [Header("Æø¹ßÈ­»ì")]
    public SeedArrow seedArrow;

    [Serializable]
    public class BindingArrow
    {
        [Header("µ¢Äð È­»ì")]
        public Sprite bindingArrowImage;
    }

    [Header("µ¢Äð È­»ì")]
    public BindingArrow bindingArrow;

    [Serializable]
    public class HommingArrow
    {
        [Header("ÃßÀû È­»ì")]
        public Sprite hommingArrowImage;
    }

    [Header("ÃßÀû È­»ì")]
    public HommingArrow hommingArrow;

    [Serializable]
    public class SplitArrow
    {
        [Header("ºÐ¿­ È­»ì")]
        public Sprite splitArrowImage;
    }

    [Header("µ¢Äð È­»ì")]
    public SplitArrow splitArrow;
}