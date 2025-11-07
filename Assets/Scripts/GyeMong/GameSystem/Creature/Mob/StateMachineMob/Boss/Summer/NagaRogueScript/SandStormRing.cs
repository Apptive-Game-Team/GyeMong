using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandStormRing : MonoBehaviour
{
    private readonly HashSet<Collider2D> outer = new();
    private readonly HashSet<Collider2D> inner = new();
    [SerializeField] private Transform centerPos;

    public void Mark(bool isInner, Collider2D col, bool inside)
    {
        var set = isInner ? inner : outer;
        if (inside) set.Add(col); else set.Remove(col);
    }

    void FixedUpdate()
    {
        foreach (var col in outer)
            if (col && !inner.Contains(col) && col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 dir = centerPos.position - col.transform.position;
                if (dir.sqrMagnitude < 0.000001f) return;
                SceneContext.Character.playerRb.AddForce(dir.normalized * 18f, ForceMode2D.Force);
            }
    }
}