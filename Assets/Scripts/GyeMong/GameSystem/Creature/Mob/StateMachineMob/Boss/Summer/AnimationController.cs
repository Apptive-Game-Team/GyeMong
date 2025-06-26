using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animation _animation;
    [SerializeField] private AnimationClip[] _animationclip;

    private void Start()
    {
        foreach (var clip in _animationclip)
        {
            _animation.AddClip(clip, clip.name);
        }
    }

    public enum Direction
    {
        Up, Down, Left, Right
    }

    public static Direction GetDirectionToTarget(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? Direction.Right : Direction.Left;
        else
            return dir.y > 0 ? Direction.Up : Direction.Down;
    }
    
    public void PlayAnimation(string animationName, Vector2 direction)
    {
        
        Direction dir = GetDirectionToTarget(direction);
        _animation.Play($"{animationName}_{dir}");
    }
}
