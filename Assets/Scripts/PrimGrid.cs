using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrimGrid : MonoBehaviour
{
    public Transform CellPrefab;
    public Vector3 size;

    public float waitTime = 0.01f;

    private Vector2Int gridCenter;
    private Transform[,] Grid;
    private PrimCell[,] cellGrid;

    private PrimCell cellScript;

    public List<Transform> visitedCells = new List<Transform>();

    public List<PrimCell> frontier = new List<PrimCell>();

    public GameObject playerPref;

    private void Start()
    {

        Grid = new Transform[(int)size.x,(int) size.y];

        cellGrid = new PrimCell[(int)size.x , (int)size.y];


        gridCenter = new Vector2Int((int)((int)transform.position.x - (size.x / 2)), (int)((int)transform.position.y - (size.y / 2)));

        CreateGrid();
        SetRandomNumbers();
        SetNeighbours();

        StartCoroutine("Prim");
    }


    void CreateGrid()
    {
        int i = 0; int j = 0;
        for(int x = gridCenter.x; x < size.x + gridCenter.x; ++x)
        {
            for(int y = gridCenter.y; y < size.y + gridCenter.y; ++y)
            {
                Transform newCell = Instantiate(CellPrefab, new Vector3(x, y, 0), Quaternion.identity,this.transform);
                string s = "(" + x + "," + y + ")";
                newCell.name = s;

                Grid[i, j] = newCell;

                PrimCell cell;
                if(cell = newCell.GetComponent<PrimCell>())
                {
                    cell.Initialize(new Vector2(x, y), 0);
                    cellGrid[i, j] = cell;
                }



                j++;
            }
            j = 0;
            i++;
        }

    }


    void SetRandomNumbers()
    {
        foreach (Transform child in this.transform)
        {
            int weight = Random.Range(1, 9);
            TextMeshPro text = child.gameObject.GetComponent<TextMeshPro>();
            if(text != null)
            {
                text.text = weight.ToString();
            }

            PrimCell cell;
            if (cell = child.GetComponent<PrimCell>())
            {
                cell.weight = weight;
            }

        }
    }


    void SetNeighbours()
    {
        for(int i = 0; i < size.x; ++i)
        {
            for(int j = 0; j < size.y; ++j)
            {
                Transform cell = Grid[i, j];

                cellScript = cell.gameObject.GetComponent<PrimCell>();
                if(cellScript != null)
                {
                    if(i -2 >= 0)
                    {
                        cellScript.neighbours.Add(Grid[i - 2, j].GetComponent<PrimCell>());
                    }
                    if (i+2 < size.x)
                    {
                        cellScript.neighbours.Add(Grid[i +2, j].GetComponent<PrimCell>());
                    }
                    if (j -2 >= 0)
                    {
                        cellScript.neighbours.Add(Grid[i , j-2].GetComponent<PrimCell>());
                    }
                    if (j +2 < size.y)
                    {
                        cellScript.neighbours.Add(Grid[i , j+2].GetComponent<PrimCell>());
                    }

                    cellScript.neighbours.Sort(SortByLowestWeight);

                }

            }
        }


    }

    int SortByLowestWeight(PrimCell c1, PrimCell c2)
    {
        int a = c1.weight;
        int b = c2.weight;

        return a.CompareTo(b); // returns the lowest
    }

    IEnumerator Prim()
    {
        // Pick a random cell to start
        Transform startCell = Grid[Random.Range(0, (int)size.x), Random.Range(0, (int)size.y)];
        visitedCells.Add(startCell);
        SpriteRenderer sprite = startCell.gameObject.GetComponentInChildren<SpriteRenderer>();
        sprite.color = Color.blue;

        BoxCollider2D col = startCell.GetComponent<BoxCollider2D>();
        col.enabled = false;

        // Add neightbours of start to the frontier
        cellScript = startCell.GetComponent<PrimCell>();
        cellScript.isStart = true;
        foreach(PrimCell neighbour in cellScript.neighbours)
        {
            if (!frontier.Contains(neighbour))
            {
                frontier.Add(neighbour);
                neighbour.parentCell = cellScript;
                neighbour.inFrontier = true;
                SpriteRenderer sprited = neighbour.gameObject.GetComponentInChildren<SpriteRenderer>();
                sprited.color = Color.red;
            }
        }

        Transform nearestCell = default;

        while (frontier.Count > 0)
        {

            ExpandFrontier();

            // Find the cell with the lowest weight
            nearestCell = FindNearestCell();

            // Connect the nearest cell to the current tree
            ConnectCells(nearestCell);

           
            
            // Place the ending point of the labyrinth
            if(visitedCells.Count >= size.x * size.y)
            {
                SpriteRenderer sp = nearestCell.gameObject.GetComponentInChildren<SpriteRenderer>();
                sp.color = Color.red;
            }

            yield return new WaitForSeconds(waitTime);
        }

        cellScript = nearestCell.GetComponent<PrimCell>();
        cellScript.isEnd = true;

        Debug.Log("Finished");
        // Place the starting point of the labyrinth
        Instantiate(playerPref, startCell.transform.position, Quaternion.identity);

    }

    Transform FindNearestCell()
    {
        Transform nearestCell = null;
        int minWeight = int.MaxValue;

            foreach (PrimCell neighbor in frontier)
            {
                if (!visitedCells.Contains(neighbor.transform) && neighbor.weight < minWeight)
                {
                    minWeight = neighbor.weight;
                    nearestCell = neighbor.transform;
                }
            }
        // Remove from frontier

        cellScript = nearestCell.GetComponent<PrimCell>();

        cellScript.visited = true;
        cellScript.inFrontier = false;
        frontier.Remove(cellScript);

        return nearestCell;
    }

    void ExpandFrontier()
    {
        foreach (Transform visited in visitedCells)
        {
            cellScript = visited.GetComponent<PrimCell>();

            foreach(PrimCell neighbour in cellScript.neighbours)
            {
                if (!neighbour.visited && !cellScript.isBrother && !neighbour.isStart)
                {
                    if (!frontier.Contains(neighbour))
                    {
                        frontier.Add(neighbour);
                        neighbour.inFrontier = true;
                        neighbour.parentCell = cellScript;
                    }
                }
            }
        }
    }

    void ConnectCells(Transform cell)
    {
        cellScript = cell.GetComponent<PrimCell>();

        Vector2 parentIndex = default;  Vector2 myIndex = default;

        Vector2 brotherIndex = default;

        for(int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; ++j)
            {
                if(cellGrid[i,j].transform == cell)
                {
                    myIndex = new Vector2(i, j);
                }
                if(cellGrid[i,j] == cellScript.parentCell)
                {
                    parentIndex = new Vector2(i, j);
                }
            }
        }
        // we have the indexes of parent and child, now let's find the brother
        // by comparing the positions of the child and father we can find the cell that bridges them

        if(myIndex.x == parentIndex.x - 2)
        {
            brotherIndex = new Vector2(parentIndex.x - 1, parentIndex.y);
        }
        if (myIndex.x == parentIndex.x +2)
        {
            brotherIndex = new Vector2(parentIndex.x + 1, parentIndex.y);
        }
        if (myIndex.y == parentIndex.y + 2)
        {
            brotherIndex = new Vector2(parentIndex.x , parentIndex.y +1);
        }
        if (myIndex.y == parentIndex.y - 2)
        {
            brotherIndex = new Vector2(parentIndex.x, parentIndex.y - 1);
        }


        cellGrid[(int) brotherIndex.x,(int) brotherIndex.y].visited = true;
        cellGrid[(int)brotherIndex.x, (int)brotherIndex.y].isBrother = true; // Won't be able to have neighbours

        cellGrid[(int)myIndex.x, (int)myIndex.y].visited = true;

        visitedCells.Add(cellGrid[(int)brotherIndex.x, (int)brotherIndex.y].transform);
        visitedCells.Add(cellGrid[(int)myIndex.x, (int)myIndex.y].transform);
        //SpriteRenderer sprite = cell.gameObject.GetComponentInChildren<SpriteRenderer>();
        //sprite.color = Color.white;

        //PrimCell cellScript = cell.gameObject.GetComponent<PrimCell>();
        //cellScript.visited = true;



        //visitedCells.Add(cell);
    }

}
