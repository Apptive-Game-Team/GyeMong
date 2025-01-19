using System;
using System.Collections.Generic;
using runeSystem.RuneTreeSystem;
using UnityEngine;

public class TreeLayoutGroup : MonoBehaviour
{
    ITreeLayoutNode[] nodes;
    ITreeLayoutNode root;
    
    [SerializeField] float margin = 30;
    [SerializeField] float nodeWidth = 100;
    [SerializeField] float nodeHeight = 100;
    
    private RectTransform rectTransform => GetComponent<RectTransform>();

    private float width, height;
    
    private void OnEnable()
    {
        ReDraw();
    }

    public void ReDraw()
    {
        width = rectTransform.sizeDelta.x;
        height = rectTransform.sizeDelta.y;
        nodes = GetComponentsInChildren<ITreeLayoutNode>();
        root = GetRoot();
        DrawNodes(root, new Vector2(margin, height/2), height);
    }
    
    private ITreeLayoutNode GetRoot()
    {
        foreach (ITreeLayoutNode node in nodes)
        {
            if (node.GetDepth() == 0)
            {
                return node;
            }
        }
        throw new Exception("Root not found");
    }
    
    private void DrawNodes(ITreeLayoutNode node, Vector2 position, float height)
    {
        node.transform.GetComponent<RectTransform>().localPosition = position;
        float nodePositionY = position.y - height/2 + margin;
        float nodePositionDeltaY = (height - 2 * margin) / Mathf.Max(1, node.GetChildrenCount() - 1);
        for (int i = 0; i < node.GetChildrenCount(); i++)
        {
            DrawNodes(node.GetChild(i), 
                new Vector2(
                    position.x + margin + nodeWidth/2, 
                    nodePositionY + nodePositionDeltaY * i
                    ), 
                height/node.GetChildrenCount());
        }
    }

}
