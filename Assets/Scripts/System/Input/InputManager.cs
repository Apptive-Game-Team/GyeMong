using System.Collections;
using System.Collections.Generic;
using System.Input.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System.Input
{
    public enum ActionCode
    {
        Attack,
        Skill,
        Dash,
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
        private const float KeyListenerDelay = 0.05f;
        private const float KetDownDelay = 1f;
    
        private Dictionary<ActionCode, bool> _keyDownBools = new Dictionary<ActionCode, bool>();
        private Dictionary<ActionCode, bool> _keyDownBoolsForListener = new Dictionary<ActionCode, bool>();
        private Dictionary<ActionCode, Coroutine> _keyDownCounterCoroutine = new Dictionary<ActionCode, Coroutine>();
        private Dictionary<ActionCode, bool> _keyActiveFlags = new Dictionary<ActionCode, bool>();
        private Dictionary<ActionCode, KeyCode> _keyMappings = new Dictionary<ActionCode, KeyCode>();
    
        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneUnloading;
        }

        private void OnSceneUnloading(Scene arg0, LoadSceneMode arg1)
        {
            // Reset all key states when the scene is unloaded
            foreach (ActionCode action in Enum.GetValues(typeof(ActionCode)))
            {
                _keyDownBools[action] = false;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneUnloading;
        }
        
        public void SetDefaultKey()
        {
            _keyMappings = new Dictionary<ActionCode, KeyCode>()
            {
                { ActionCode.Attack, KeyCode.Mouse0 },
                { ActionCode.Skill, KeyCode.Mouse1 },
                { ActionCode.Dash, KeyCode.LeftShift },
                { ActionCode.Interaction, KeyCode.Space },
                { ActionCode.MoveUp, KeyCode.W },
                { ActionCode.MoveDown, KeyCode.S },
                { ActionCode.MoveRight, KeyCode.D },
                { ActionCode.MoveLeft, KeyCode.A },
                { ActionCode.RunePage, KeyCode.I },
                { ActionCode.Option, KeyCode.Escape},
                { ActionCode.SelectClick, KeyCode.Mouse0 },
            };
        }

        Vector3 _moveVector = new Vector3();
        private List<Vector2> _directionList = new List<Vector2>();
        public static event Action<ActionCode, InputType> OnKeyEvent;

        /// <summary>
        /// Checks if the specified action code represents a movement action.
        /// </summary>
        /// <param name="action">Action code to check</param>
        /// <returns>True if action is movement-related, otherwise false</returns>
        public bool IsMoveActioncode(ActionCode action)
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
            _keyActiveFlags[action] = active;
        }


        /// <summary>
        /// Activates or deactivates all actions.
        /// </summary>
        /// <param name="active">True to activate movement actions, false to deactivate</param>
        public void SetActionState(bool active)
        {
            foreach (ActionCode actionCode in _keyMappings.Keys)
            {
                if (actionCode != ActionCode.Interaction)
                    SetKeyActive(actionCode, active);
            }
        }

        /// <summary>
        /// Gets the active state of a specified action key.
        /// </summary>
        /// <param name="action">Action code to check</param>
        /// <returns>True if active, false otherwise</returns>
        public bool GetKeyActive(ActionCode action)
        {
            return _keyActiveFlags[action];
        }

        /// <summary>
        /// Gets the keyMappings.
        /// </summary>
        public Dictionary<ActionCode,KeyCode> GetKeyActions()
        {
            return _keyMappings;
        }

        /// <summary>
        /// Sets a new key mapping for a specified action.
        /// </summary>
        /// <param name="actionCode">Action code to map</param>
        /// <param name="newKey">New KeyCode to assign</param>
        public void SetKey(ActionCode actionCode, KeyCode newKey)
        {
            if (_keyMappings.ContainsKey(actionCode))
            {
                _keyMappings[actionCode] = newKey;
            }
        }

        /// <summary>
        /// Returns true if the specified key was pressed down in this frame.
        /// </summary>
        /// <param name="action">Action code to check</param>
        /// <returns>True if key was pressed down, false otherwise</returns>
        public bool GetKeyDown(ActionCode action)
        {
            try
            {
                if (_keyActiveFlags[action] && _keyDownBools[action])
                {
                    _keyDownBools[action] = false;
                    return true;
                }

            }
            catch (KeyNotFoundException e)
            {
                _keyActiveFlags[action] = true;
                _keyDownBools[action] = false;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the specified key is currently being held down.
        /// </summary>
        /// <param name="action">Action code to check</param>
        /// <returns>True if key is held down, otherwise false</returns>
        public bool GetKey(ActionCode action)
        {
            try
            {
                return (UnityEngine.Input.GetKey(_keyMappings[action]) && _keyActiveFlags[action]);
            }
            catch (KeyNotFoundException e)
            {
                SetDefaultKey();
                return false;
            }
        }

        private IEnumerator KeyDownCounter(ActionCode action)
        {
            yield return new WaitForSeconds(KetDownDelay);
            _keyDownBools[action] = false;
            _keyDownCounterCoroutine[action] = null;
        }

        private void Update()
        {
            foreach (ActionCode action in _keyMappings.Keys)
            {
                try
                {
                    if (_keyActiveFlags[action])
                    {
                        if (UnityEngine.Input.GetKeyDown(_keyMappings[action]))//
                        {
                            _keyDownBools[action] = true;
                            _keyDownBoolsForListener[action] = true;
                            Coroutine tempCoroutine = _keyDownCounterCoroutine[action];
                            if (tempCoroutine != null)
                            {
                                StopCoroutine(tempCoroutine);
                            }

                            _keyDownCounterCoroutine[action] = StartCoroutine(KeyDownCounter(action));
                        }
                    }
                }
                catch (KeyNotFoundException e)
                {
                    SetDefaultKey();
                    _keyDownCounterCoroutine[action] = null;
                    _keyDownBools[action] = false;
                    _keyActiveFlags[action] = true;
                }
            
            }

        }

        private void InitKeyDownDictionary()
        {
            foreach (ActionCode action in Enum.GetValues(typeof(ActionCode)))
            {
                _keyDownBools[action] = false;
                _keyDownCounterCoroutine[action] = null;
                _keyActiveFlags[action] = true;
                _keyDownBoolsForListener[action] = false;
            }
        }

        private IEnumerator CallListenersCoroutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(KeyListenerDelay);
                foreach (ActionCode action in _keyMappings.Keys)
                {
                    if (_keyActiveFlags[action])
                    {
                        if (_keyDownBoolsForListener[action])
                        {
                            _keyDownBoolsForListener[action] = false;
                            OnKeyEvent?.Invoke(action, InputType.Down);
                        }
                        else if (UnityEngine.Input.GetKey(_keyMappings[action]))
                        {
                            OnKeyEvent?.Invoke(action, InputType.Press);
                        }
                        else if (UnityEngine.Input.GetKeyUp(_keyMappings[action]))
                        {
                            OnKeyEvent?.Invoke(action, InputType.Up);
                        }
                    }
                }
            }
        }

        private void Start()
        {
            SetDefaultKey();
            InitKeyDownDictionary();
            StartCoroutine(CallListenersCoroutine());
        }
    }
}