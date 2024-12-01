using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionSet
{
    string title;
    string description;

    public DescriptionSet(string title, string description)
    {
        this.title = title;
        this.description = description;
    }

    public string Title
    { get { return title; } }
    public string Description
    { get { return description; } }
}

public interface IDescriptionUI
{
    public void SetDescription(DescriptionSet description);
}

public class RuneDescriptionUI : MonoBehaviour, IDescriptionUI
{

    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textDescription;

    public void SetDescription(DescriptionSet descriptionSet)
    {
        textTitle.text = descriptionSet.Title; 
        textDescription.text = descriptionSet.Description;
    }

}
