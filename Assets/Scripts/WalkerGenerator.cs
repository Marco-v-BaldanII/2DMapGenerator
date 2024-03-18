using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{

    public enum Grid
    {
        FLOOR,
        WALL,
        EMPTY
    }


    public Grid[,] gridHandler;

    public List<WalkerObject> Walkers;

    public Tilemap tileMap;
    public Tile Floor;
    public Tile Wall;
    public int mapWidth;
    public int mapHeight;

    public int maxWalkers = 10;
    public int TileCount = default;

    public float FillPercentage = 0.4f;

    public float WaitTime = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        Walkers = new List<WalkerObject>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridHandler = new Grid[mapWidth,mapHeight];

        for(int i = 0; i < mapWidth; ++i)
        {
            for(int j = 0; j < mapHeight; ++j)
            {

                gridHandler[i, j] = Grid.EMPTY;

            }
        }

        Vector2Int TileCenter = new Vector2Int(mapWidth / 2, mapHeight/ 2);

        // Create the first walker in the center
        WalkerObject MrWalker = new WalkerObject(TileCenter, GetDirection(), 0.5f);

   

        // Place in its position the first floor tile
        gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;

        tileMap.SetTile(new Vector3Int( TileCenter.x, TileCenter.y, 0), Floor);
        Walkers.Add(MrWalker);

        TileCount++;
        StartCoroutine(CreateFloors());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Generate a random direction
    Vector2 GetDirection()
    {
        int choice = Random.Range(1, 4);
        switch (choice)
        {
            case 1:
                return Vector2.up;
                break;
            case 2:
                return Vector2.down;
                break;
            case 3:
                return Vector2.left;
                break;
            case 4:
                return Vector2.right;
                break;
            default:
                return Vector2.zero;
        }
    }

    private IEnumerator CreateFloors()
    {
        yield return new WaitForSeconds(0.2f);

        while( (float)TileCount/(float)gridHandler.Length < FillPercentage)
        {
            bool hasCreatedFloor = false;
            foreach (WalkerObject curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker._position.x, (int)curWalker._position.y, 0);

                // If walker has reached a empty tile , place a floot tile
                if(gridHandler[curPos.x, curPos.y] != Grid.FLOOR)
                {
                    tileMap.SetTile(curPos, Floor);
                    TileCount++;
                    gridHandler[curPos.x, curPos.y] = Grid.FLOOR;
                    hasCreatedFloor = true;
                }
            }

            // Walker chance methods
            ChanceToRemove();

            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            // pause a bit after placing each tile for visual representation
            if (hasCreatedFloor)
            {
                yield return new WaitForSeconds(WaitTime);
            }



        }

        StartCoroutine(CreateWalls());
    }

    // Does a chance check to see if the walker is destroyed
    void ChanceToRemove()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < Walkers[i]._ChanceToChange && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                break;
            }
        }
    }

    // Does a chance check to see if the walker changes position
    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (UnityEngine.Random.value < Walkers[i]._ChanceToChange)
            {
                WalkerObject curWalker = Walkers[i];
                curWalker._direction = GetDirection();
                Walkers[i] = curWalker;
            }
        }
    }

    // does a chance check to see if the walker creates another walker
    void ChanceToCreate()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < Walkers[i]._ChanceToChange && Walkers.Count < maxWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = Walkers[i]._position;

                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.5f);
                Walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {
        for(int i = 0; i < Walkers.Count; ++i)
        {
            WalkerObject curWalker = Walkers[i];
            curWalker._position += curWalker._direction;

            // ensures that the walker doesn't exit the range of the grid (max -2 to account for wall space)
            curWalker._position.x = Mathf.Clamp(curWalker._position.x, 1, gridHandler.GetLength(0) - 2);
            curWalker._position.y = Mathf.Clamp(curWalker._position.y, 1, gridHandler.GetLength(0) - 2);

            Walkers[i] = curWalker;
        }
    }

    private IEnumerator CreateWalls()
    {
        for(int i = 0; i < gridHandler.GetLength(0)-1; ++i)
        {
            for(int j = 0; j < gridHandler.GetLength(1)-1; ++j)
            {
                // Find the floor tile and check if it's neighbours are empty to place walls
                if(gridHandler[i,j] == Grid.FLOOR)
                {

                    bool hasCreatedWall = false;

                    if(gridHandler[i+1,j] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(i + 1, j, 0), Wall);
                        gridHandler[i + 1, j] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (gridHandler[i , j+1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(i , j+1, 0), Wall);
                        gridHandler[i, j+1] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (gridHandler[i -1, j] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(i - 1, j, 0), Wall);
                        gridHandler[i -1, j] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (gridHandler[i , j-1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(i ,j-1, 0), Wall);
                        gridHandler[i , j-1] = Grid.WALL;
                        hasCreatedWall = true;
                    }

                    if (hasCreatedWall)
                    {
                        yield return new WaitForSeconds(WaitTime);
                    }
                }

            }
        }

        Debug.Log("Random map of" + TileCount + "tiles created");
    }

}
