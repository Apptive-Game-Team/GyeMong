using GyeMong.EventSystem.Controller;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.SoundSystem;
using Unity.VisualScripting;
using UnityEngine;
using Visual.Camera;

public class SceneContext : MonoBehaviour
{
    private static PlayerCharacter _character;

    public static PlayerCharacter Character
    {
        get
        {
            if (_character == null)
            {
                _character = FindObjectOfType<PlayerCharacter>();
            }
            return _character;
        }
    }
    
    private static EffectManager _effectManager;
    public static EffectManager EffectManager
    {
        get
        {
            if (_effectManager == null || _effectManager.IsDestroyed())
            {
                _effectManager = FindObjectOfType<EffectManager>();
            }
            return _effectManager;
        }
    }
    
    private static CameraManager _cameraManager;
    public static CameraManager CameraManager
    {
        get
        {
            if (_cameraManager == null || _cameraManager.IsDestroyed())
            {
                _cameraManager = FindObjectOfType<CameraManager>();
            }
            return _cameraManager;
        }
    }
    
    private static SoundObject _bgmObject;
    public static SoundObject BgmObject
    {
        get
        {
            if (_bgmObject == null || _bgmObject.IsDestroyed())
            {
                _bgmObject = Sound.GetSoundObject();
            }

            return _bgmObject;
        }
    }

    private void Awake()
    {
        _character = FindObjectOfType<PlayerCharacter>();
    }
}