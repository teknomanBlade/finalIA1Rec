using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private NodeGrid _grid;
    private int _x;
    private int _y;

    public bool isBlocked;
    public int cost = 1;

    //public TextMeshProUGUI textUGUI;

    public void Initialize(NodeGrid g, int x, int y)
    {
        _grid = g;
        _x = x;
        _y = y;
        ChangeCost(0);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            GameManager.instance.SetStartingNode(this);
        if (Input.GetMouseButtonDown(1))
            GameManager.instance.SetGoalNode(this);

        if (Input.GetKey(KeyCode.UpArrow))
            ChangeCost(cost + 1);
        if (Input.GetKey(KeyCode.DownArrow))
            ChangeCost(cost - 1);
        if (Input.GetKey(KeyCode.R))
            ChangeCost(0);

    }

    void ChangeCost(int cst)
    {
        cost = Mathf.Clamp(cst, 0, 98);
        //textUGUI.enabled = (cost > 0);
        //textUGUI.text = cost.ToString();
    }

    public List<Node> GetNeighbors()
    {
        return _grid.GetNeighborsBasedOnPosition(_x, _y);
    }

    private void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
        GameManager.instance.ChangeColor(gameObject, isBlocked ? Color.red : Color.white);
        ChangeCost(0);
        //Definir como layer bloqueada
        int layer = isBlocked ? 6 : 0;
        gameObject.layer = layer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) 
        {
            SetBlocked(true);
        }
    }
}
