using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionCode
{
    Attack,
    Defend,
    Dash,
    Run,
    Interaction,
    MoveUp,
    MoveDown,
    MoveRight,
    MoveLeft,
    RunePage,
    Option,
    SelectClick,
}

public class InputManager : SingletonObject<InputManager>
{
    private const float KEY_LISTENER_DELAY = 0.05f;
    private const float KET_DOWN_DELAY = 1f;


    private Dictionary<ActionCode, bool> keyDownBools = new Dictionary<ActionCode, bool>();
    private Dictionary<ActionCode, bool> keyDownBoolsForListener = new Dictionary<ActionCode, bool>();
    private Dictionary<ActionCode, Coroutine> keyDownCounterCoroutine = new Dictionary<ActionCode, Coroutine>();
    private Dictionary<ActionCode, bool> keyActiveFlags = new Dictionary<ActionCode, bool>();
    private Dictionary<ActionCode, KeyCode> keyMappings = new Dictionary<ActionCode, KeyCode>()
    {
        { ActionCode.Attack, KeyCode.A},
        { ActionCode.Defend, KeyCode.S},
        { ActionCode.Dash, KeyCode.X},
        { ActionCode.Run, KeyCode.LeftShift},
        { ActionCode.Interaction, KeyCode.Z },
        { ActionCode.MoveUp, KeyCode.UpArrow },
        { ActionCode.MoveDown, KeyCode.DownArrow },
        { ActionCode.MoveRight, KeyCode.RightArrow },
        { ActionCode.MoveLeft, KeyCode.LeftArrow },
        { ActionCode.RunePage, KeyCode.I },
        { ActionCode.Option, KeyCode.Escape},
        { ActionCode.SelectClick, KeyCode.Mouse0 },
    };

    Vector3 moveVector = new Vector3();
    private List<Vector2> directionList = new List<Vector2>();

    private List<IInputListener> inputListeners = new List<IInputListener>();

    /// <summary>
    /// Checks if the specified action code represents a movement action.
    /// </summary>
    /// <param name="action">Action code to check</param>
    /// <returns>True if action is movement-related, otherwise false</returns>
    public bool isMoveActioncode(ActionCode action)
    {
        return (int)action >= (int)ActionCode.MoveUp && (int)action <= (int)ActionCode.MoveLeft;
    }

    /// <summary>
    /// Sets the active state of a specific action key.
    /// </summary>
    /// <param name="action">Action code to set</param>
    /// <param name="active">True to activate, false to deactivate</param>
    public void SetKeyActive(ActionCode action, bool active)
    {
        keyActiveFlags[action] = active;
    }


    /// <summary>
    /// Activates or deactivates all movement-related actions.
    /// </summary>
    /// <param name="active">True to activate movement actions, false to deactivate</param>
    public void SetMovementState(bool active)
    {
        SetKeyActive(ActionCode.MoveUp, active);
        SetKeyActive(ActionCode.MoveDown, active);
        SetKeyActive(ActionCode.MoveLeft, active);
        SetKeyActive(ActionCode.MoveRight, active);
    }

    /// <summary>
    /// Gets the active state of a specified action key.
    /// </summary>
    /// <param name="action">Action code to check</param>
    /// <returns>True if active, false otherwise</returns>
    public bool GetKeyActive(ActionCode action)
    {
        return keyActiveFlags[action];
    }

    /// <summary>
    /// Gets the keyMappings.
    /// </summary>
    public Dictionary<ActionCode,KeyCode> GetKeyActions()
    {
        return keyMappings;
    }

    /// <summary>
    /// Sets a new key mapping for a specified action.
    /// </summary>
    /// <param name="actionCode">Action code to map</param>
    /// <param name="newKey">New KeyCode to assign</param>
    public void SetKey(ActionCode actionCode, KeyCode newKey)
    {
        if (keyMappings.ContainsKey(actionCode))
        {
            keyMappings[actionCode] = newKey;
        }
    }

    /// <summary>
    /// Returns true if the specified key was pressed down in this frame.
    /// </summary>
    /// <param name="action">Action code to check</param>
    /// <returns>True if key was pressed down, false otherwise</returns>
    public bool GetKeyDown(ActionCode action)
    {
        if (keyDownBools[action])
        {
            keyDownBools[action] = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns a Vector3 representing the current movement direction based on input.
    /// </summary>
    /// <returns>Movement direction as Vector3</returns>
    public Vector3 GetMoveVector()
    {
        if (GetKey(ActionCode.MoveUp))
        {
            if (!directionList.Contains(Vector2.up))
            {
                directionList.Add(Vector2.up);
            }
        }
        else
        {
            directionList.Remove(Vector2.up);
        }

        if (GetKey(ActionCode.MoveDown))
        {
            if (!directionList.Contains(Vector2.down))
            {
                directionList.Add(Vector2.down);
            }
        }
        else
        {
            directionList.Remove(Vector2.down);
        }

        if (GetKey(ActionCode.MoveLeft))
        {
            if (!directionList.Contains(Vector2.left))
            {
                directionList.Add(Vector2.left);
            }
        }
        else
        {
            directionList.Remove(Vector2.left);
        }

        if (GetKey(ActionCode.MoveRight))
        {
            if (!directionList.Contains(Vector2.right))
            {
                directionList.Add(Vector2.right);
            }
        }
        else
        {
            directionList.Remove(Vector2.right);
        }

        if (directionList.Count > 0)
        {
            moveVector.x = directionList[^1].x;
            moveVector.y = directionList[^1].y;
        }
        else
        {
            moveVector.x = 0;
            moveVector.y = 0;
        }

        return moveVector;
    }

    /// <summary>
    /// Returns true if the specified key is currently being held down.
    /// </summary>
    /// <param name="action">Action code to check</param>
    /// <returns>True if key is held down, otherwise false</returns>
    public bool GetKey(ActionCode action)
    {
        return (Input.GetKey(keyMappings[action]) && keyActiveFlags[action]);
    }

    /// <summary>
    /// Registers a listener to receive input events.
    /// </summary>
    /// <param name="listener">Listener to add</param>
    public void SetInputListener(IInputListener listener)
    {
        inputListeners.Add(listener);
    }

    private IEnumerator KeyDownCounter(ActionCode action)
    {
        yield return new WaitForSeconds(KET_DOWN_DELAY);
        keyDownBools[action] = false;
        keyDownCounterCoroutine[action] = null;
    }

    private void Update()
    {
        foreach (ActionCode action in keyMappings.Keys)
        {
            if (keyActiveFlags[action])
            {
                if (Input.GetKeyDown(keyMappings[action]))
                {
                    keyDownBools[action] = true;
                    keyDownBoolsForListener[action] = true;
                    Coroutine tempCoroutine = keyDownCounterCoroutine[action];
                    if (tempCoroutine != null)
                    {
                        StopCoroutine(tempCoroutine);
                    }
                    keyDownCounterCoroutine[action] = StartCoroutine(KeyDownCounter(action));
                }
            }
        }

    }

    private void InitKeyDownDictionarys()
    {
        foreach (ActionCode action in Enum.GetValues(typeof(ActionCode)))
        {
            keyDownBools.Add(action, false);
            keyDownCounterCoroutine.Add(action, null);
            keyActiveFlags.Add(action, true);
            keyDownBoolsForListener.Add(action, false);
        }
    }

    private IEnumerator CallListenersCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(KEY_LISTENER_DELAY);
            foreach (ActionCode action in keyMappings.Keys)
            {
                if (keyActiveFlags[action])
                {
                    if (keyDownBoolsForListener[action])
                    {
                        keyDownBoolsForListener[action] = false;
                        CallOnKeyDownListeners(action);
                    }
                    else if (Input.GetKey(keyMappings[action]))
                    {
                        CallOnKeyListeners(action);
                    }
                    else if (Input.GetKeyUp(keyMappings[action]))
                    {
                        CallOnKeyUpListeners(action);
                    }
                }

            }
        }
    }

    private void Start()
    {
        InitKeyDownDictionarys();
        StartCoroutine(CallListenersCoroutine());
    }

    private void CallOnKeyListeners(ActionCode action)
    {
        foreach (IInputListener listener in inputListeners)
        {
            listener.OnKey(action);

        }
    }

    private void CallOnKeyDownListeners(ActionCode action)
    {
        foreach (IInputListener listener in inputListeners)
        {
            listener.OnKeyDown(action);

        }
    }

    private void CallOnKeyUpListeners(ActionCode action)
    {
        foreach (IInputListener listener in inputListeners)
        {
            listener.OnKeyUp(action);

        }
    }
}