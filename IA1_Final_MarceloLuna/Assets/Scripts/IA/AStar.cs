using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public List<Node> ConstructPath(Node startingNode, Node goalNode)
    {
        if (!startingNode || !goalNode) return null;

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Get();

            //PathBuilding
            if (current == goalNode)
            {
                List<Node> path = new List<Node>();
                while (current != null)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse(); // opcional
                return path;
            }

            foreach (var next in current.GetNeighbors())
            {
                if (next.isBlocked) continue;
                float newCost = costSoFar[current] + 1 + next.cost;
                float dist = Vector3.Distance(next.transform.position, goalNode.transform.position);
                float priority = newCost + dist; // Elevar al cuadrado si se lo necesita

                if (!cameFrom.ContainsKey(next)) // Lo agregamos normalmente
                {
                    frontier.Put(next, priority);
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                }
                else if (costSoFar[next] > newCost) // pero si por alguna razon ya esta y el costo es menor
                {
                    frontier.Put(next, priority);//Lo modificamos en frontier (adentro de prio Queue)
                    costSoFar[next] = newCost; //Le cambiamos el costo que existia en este diccionario 
                    cameFrom[next] = current; //Le decimos que ahora viene por aca (que es mas barato)
                }
            }
        }
        return default;
    }

    public IEnumerator PaintAStar(Node startingNode, Node goalNode)
    {
        if (!startingNode || !goalNode) yield return null;

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Get();
            //GameManager.instance.ChangeColor(current.gameObject, Color.blue);
            yield return new WaitForSeconds(0.1f);

            //PathBuilding
            if (current == goalNode)
            {
                List<Node> path = new List<Node>();
                while (current != null)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse(); // opcional

                //GameManager.instance.PaintPath(path);
                break;
            }

            foreach (var next in current.GetNeighbors())
            {
                if (next.isBlocked) continue;
                float newCost = costSoFar[current] + 1 + next.cost;
                float dist = Vector3.Distance(next.transform.position, goalNode.transform.position);
                float priority = newCost + dist * dist;

                if (!cameFrom.ContainsKey(next)) // Lo agregamos normalmente
                {
                    frontier.Put(next, priority);
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                }
                else if (costSoFar[next] > newCost) // pero si por alguna razon ya esta y el costo es menor
                {
                    frontier.Put(next, priority);//Lo modificamos en frontier (adentro de prio Queue)
                    costSoFar[next] = newCost; //Le cambiamos el costo que existia en este diccionario 
                    cameFrom[next] = current; //Le decimos que ahora viene por aca (que es mas barato)
                }
            }
        }

    }

}
