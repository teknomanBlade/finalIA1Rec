using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Node> AllNodes;
    public List<Node> CornerNodes;
    private Node _startingNode;
    private Node _goalNode;
    public AStar aStar;
    public ThetaStar thetaStar;
    public NodeGrid grid;

    public LayerMask wallMask;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        aStar = new AStar();
        thetaStar = new ThetaStar();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetStartingNode(Node n)
    {
        if (_startingNode != null) ChangeColor(_startingNode.gameObject, Color.white);
        _startingNode = n;
        ChangeColor(_startingNode.gameObject, Color.red);
    }
    public void SetGoalNode(Node n)
    {
        if (_goalNode != null) ChangeColor(_goalNode.gameObject, Color.white);
        _goalNode = n;
        ChangeColor(_goalNode.gameObject, Color.green);
    }

    public void ChangeColor(GameObject obj, Color color)
    {
        obj.GetComponent<MeshRenderer>().material.color = color;
    }

    public void PaintPath(List<Node> path)
    {
        float index = 0;
        foreach (var node in path)
        {
            index++;
            ChangeColor(node.gameObject, Color.Lerp(Color.yellow, Color.red, index / path.Count));
        }
    }


    //IN SIGHT
    public bool InSight(Vector3 start, Vector3 end)
    {
        return !Physics.Raycast(start, end - start, (end - start).magnitude, wallMask); //chequear si funciona esto 
    }
}
