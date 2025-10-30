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
        [Header("����ȭ��")]
        public Sprite seedArrowImage;
    }

    [Header("����ȭ��")]
    public SeedArrow seedArrow;

    [Serializable]
    public class BindingArrow
    {
        [Header("���� ȭ��")]
        public Sprite bindingArrowImage;
    }

    [Header("���� ȭ��")]
    public BindingArrow bindingArrow;

    [Serializable]
    public class HommingArrow
    {
        [Header("���� ȭ��")]
        public Sprite hommingArrowImage;
    }

    [Header("���� ȭ��")]
    public HommingArrow hommingArrow;

    [Serializable]
    public class SplitArrow
    {
        [Header("�п� ȭ��")]
        public Sprite splitArrowImage;
    }

    [Header("���� ȭ��")]
    public SplitArrow splitArrow;
}