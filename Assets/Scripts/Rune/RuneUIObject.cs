using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectableState
{
    INACITVE = 0,
    ACTIVE = 1,
}

public class SelectableUI : ISelectableUI
{
    SelectableState uiState;
}

public interface ISelectableUI
{

}

public class RuneUIObject : SelectableUI
{

}
