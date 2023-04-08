using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    Dictionary<Node, float> _allNodes = new Dictionary<Node, float>();
    public int Count { get { return _allNodes.Count; } }

    public void Put(Node key, float value)
    {
        if (_allNodes.ContainsKey(key)) _allNodes[key] = value;
        else _allNodes.Add(key, value);
    }

    public Node Get()
    {
        Node priorityNode = null;
        foreach (var item in _allNodes)
        {
            if (!priorityNode)
            {
                priorityNode = item.Key;
                continue;
            }
            if (item.Value < _allNodes[priorityNode]) priorityNode = item.Key;

        }
        _allNodes.Remove(priorityNode);
        return priorityNode;
    }
}
