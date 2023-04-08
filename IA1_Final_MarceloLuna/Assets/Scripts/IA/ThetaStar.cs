using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThetaStar : AStar
{
    public List<Node> GetPath(Node startingNode, Node goalNode)
    {
        if (!startingNode || !goalNode) return null;

        List<Node> path = ConstructPath(startingNode, goalNode); // de la herencia de A*

        //Post Smoothing
        int index = 0;
        int indexNextNext = index + 2;

        while (indexNextNext < path.Count)
        {
            if (GameManager.instance.InSight(path[index].transform.position, path[indexNextNext].transform.position))
            {
                path.RemoveAt(index + 1);
            }
            else
            {
                index++;
                indexNextNext++;
            }
        }

        return path;
    }

}
