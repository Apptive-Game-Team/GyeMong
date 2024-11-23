using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectableState
{
    INACITVE = 0,
    ACTIVE = 1,
}

public class SelectableUI : MonoBehaviour, ISelectableUI
{
}

public interface ISelectableUI
{

}
