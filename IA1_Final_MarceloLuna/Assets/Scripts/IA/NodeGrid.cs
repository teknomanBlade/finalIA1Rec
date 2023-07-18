using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    Node[,] grid;

    public int gridHeight;
    public int gridWidth;

    public GameObject nodePrefab;
    public float offset = 1;

    // Start is called before the first frame update
    void Awake()
    {
        nodePrefab = Resources.Load<Node>("Node").gameObject;
        GenerateGrid();
        GameManager.instance.AllNodes = FindObjectsOfType<Node>().ToList();
        GameManager.instance.CornerNodes = FindObjectsOfType<Node>().Where(x => x.tag.Contains("Corner")).ToList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateGrid()
    {
        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject obj = Instantiate(nodePrefab);
                obj.name = "Node " + x + "," + y;
                obj.transform.position = new Vector3(x * offset, 0, y * offset);
                Node node = obj.GetComponent<Node>();
                node.Initialize(this, x, y);
                grid[x, y] = node;
            }
        }
    }

    public List<Node> GetNeighborsBasedOnPosition(int x, int y)
    {
        List<Node> neighbors = new List<Node>();

        Node upNode = y + 1 < gridHeight ? grid[x, y + 1] : null;
        Node rightNode = x + 1 < gridWidth ? grid[x + 1, y] : null;
        Node downNode = y - 1 >= 0 ? grid[x, y - 1] : null;
        Node leftNode = x - 1 >= 0 ? grid[x - 1, y] : null;


        if (upNode != null) neighbors.Add(upNode);
        if (rightNode != null) neighbors.Add(rightNode);
        if (downNode != null) neighbors.Add(downNode);
        if (leftNode != null) neighbors.Add(leftNode);

        return neighbors;
    }

    public void ClearColors()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!grid[x, y].isBlocked) 
                    GameManager.instance.ChangeColor(grid[x, y].gameObject, Color.white);
            }
        }
    }
}
