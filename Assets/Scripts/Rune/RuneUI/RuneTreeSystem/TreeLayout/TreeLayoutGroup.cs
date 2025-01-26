using System;
using System.Collections.Generic;
using runeSystem.RuneTreeSystem;
using UnityEngine;

public class TreeLayoutGroup : MonoBehaviour
{
    [SerializeField] ITreeLineDrawer lineDrawer;
    ITreeLayoutNode[] nodes;
    ITreeLayoutNode root;
    
    [SerializeField] float marginX = 100;
    [SerializeField] float marginY = 30;
    [SerializeField] float nodeWidth = 100;
    [SerializeField] float nodeHeight = 100;
    
    private RectTransform rectTransform => GetComponent<RectTransform>();

    private float width, height;

    private void Awake()
    {
        lineDrawer = GetComponentInChildren<ITreeLineDrawer>();
    }

    private void Update()
    {
        ReDraw();
    }

    public void ReDraw()
    {
        lineDrawer.ClearLines();
        width = rectTransform.sizeDelta.x;
        height = rectTransform.sizeDelta.y;
        nodes = GetComponentsInChildren<ITreeLayoutNode>();
        root = GetRoot();
        DrawNodes(root, new Vector2(marginX, height/2), height/2);
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
        node.transform.GetComponent<RectTransform>().anchoredPosition = position;
        
        if (node.GetChildrenCount() == 0) return;
        
        float nodePositionY = position.y - height/2 + marginY;
        float nodePositionDeltaY = (height - 2 * marginY) / Mathf.Max(1, node.GetChildrenCount() - 1);
        for (int i = 0; i < node.GetChildrenCount(); i++)
        {
            lineDrawer.ConnectNodes(position, 
                new Vector2(
                    position.x + marginX + nodeWidth, 
                    nodePositionY + nodePositionDeltaY * i
                    ));
            DrawNodes(node.GetChild(i), 
                new Vector2(
                    position.x + marginX + nodeWidth/2, 
                    nodePositionY + nodePositionDeltaY * i
                    ), 
                height/node.GetChildrenCount());
        }
    }
}
