using System;
using runeSystem.RuneTreeSystem;
using UnityEngine;

public class TreeLayoutGroup : MonoBehaviour
{
    ITreeLayoutNode[] nodes;
    ITreeLayoutNode root;

    private float width, height;
    
    private void Update()
    {
        // ReDraw();
    }

    public void ReDraw()
    {
        width = transform.localScale.x;
        height = transform.localScale.y;
        nodes = GetComponentsInChildren<ITreeLayoutNode>();
        root = nodes[0];
        DrawNodes(root);
    }

    private void DrawNodes(ITreeLayoutNode node)
    {
        Vector2 screenPosition = new Vector3(0, -height / 2, 0);
        node.transform.localPosition = screenPosition;
        node.transform.rotation = Quaternion.identity;
        node.transform.localScale = Vector3.one;
        
        for (int i = 0; i < node.GetChildrenCount(); i++)
        {
            DrawNodes(node.GetChild(i));
        }
    }
}
