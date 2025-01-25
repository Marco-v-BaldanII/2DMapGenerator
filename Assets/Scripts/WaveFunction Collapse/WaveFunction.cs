using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimensions;
    /// <summary>
    /// The tileset of the room, all possible tiles
    /// </summary>
    public mTile[] tileObjects;
    /// <summary>
    /// All cells placed, which can have tiles
    /// </summary>
    public List<Cell> gridComponents;

    public List<Vector2Int> allowedPositions;

    public Cell cellObj;

    public Vector2Int startpos;

    int iterations = 0;

    int instancedTiles = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = startpos.y; y < dimensions + startpos.y; y++)
        {
            for (int x = startpos.x; x < dimensions + startpos.x; x++)
            {
                // create a grid of CELLS only in allowed positions
                if (allowedPositions.Contains(new Vector2Int(x, y)))
                {
                    Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
                    newCell.CreateCell(false, tileObjects);
                    gridComponents.Add(newCell);
                }
            }
        }

        StartCoroutine(CheckEntropy());
    }

    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        Cell lowestEntropyCell = null;
        int min = 999;

        for (int i = 0; i < tempGrid.Count; ++i)
        {
            if (tempGrid[i].tileOptions.Length < min && tempGrid[i].tileOptions.Length != 0 && tempGrid[i].collapsed == false)
            {
                min = tempGrid[i].tileOptions.Length;
                lowestEntropyCell = tempGrid[i];
            }
        }

        yield return new WaitForSeconds(0.004f);
        // Once the lowest entropy cell has been found, collapse it
        if (lowestEntropyCell != null)
        {
            CollapseCell(lowestEntropyCell);
        }
        else
        {
            Debug.Log("This shouldn't happen");
        }
    }

    void CollapseCell(Cell cell)
    {
        Cell cellToCollapse = cell;

        cellToCollapse.collapsed = true;
        if (cellToCollapse.tileOptions.Length <= 0)
        {
            Debug.Log("not?");
        }
        mTile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
        cellToCollapse.tileOptions = new mTile[] { selectedTile };

        mTile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);
        instancedTiles++;

        if (instancedTiles < allowedPositions.Count)
        {
            print(instancedTiles + " have been created out of the allowed " + allowedPositions.Count);
            UpdateGeneration();
        }
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        foreach (Cell cell in gridComponents)
        {
            Vector2Int pos = new Vector2Int ((int)cell.transform.position.x ,(int) cell.transform.position.y) ;

            // If the cell is in the allowedPositions
            if (allowedPositions.Contains(pos) && !cell.collapsed)
            {
                List<mTile> options = new List<mTile>();

                // Add all tile options by default
                foreach (mTile t in tileObjects)
                {
                    options.Add(t);
                }

                // Check for neighboring cells within allowed positions
                CheckNeighboringCells(options, pos);

                mTile[] newTileList = new mTile[options.Count];
                for (int i = 0; i < options.Count; ++i)
                {
                    newTileList[i] = options[i];
                }

                cell.RecreateCell(newTileList);
            }
        }

        gridComponents = newGenerationCell;
        iterations++;

        if (iterations < allowedPositions.Count)
        {
            StartCoroutine(CheckEntropy());
        }
    }

    void CheckNeighboringCells(List<mTile> options, Vector2Int pos)
    {
        List<mTile> validOptions = new List<mTile>();


        if (allowedPositions.Contains(pos + Vector2Int.up))
        {
            // Find the cell right above the current one in the grid
            Cell up = gridComponents.Find(c => Vector2Int.FloorToInt( (Vector2)c.transform.position  ) == pos + Vector2Int.up);

            // Iterate through each possible tile that may be placed above this one
            foreach (mTile possibleOption in up.tileOptions)
            {
                // get index of tile
                int valOption = Array.FindIndex(tileObjects, obj => obj == possibleOption);

                // Get which tiles can be placed below this one
                mTile[] valid = tileObjects[valOption].downNeighbours;

                // add up valid options to array
                validOptions = validOptions.Concat(valid).ToList();
            }
        }

        if (allowedPositions.Contains(pos + Vector2Int.right))
        {
            // Find the cell to the right of the current one
            Cell right = gridComponents.Find(c => Vector2Int.FloorToInt((Vector2)c.transform.position) == pos + Vector2Int.right);

            // Iterate through each possible tile that may be placed below this one
            foreach (mTile possibleOptions in right.tileOptions)
            {
                // get index of tile
                int valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);

                // Get which tiles can be placed left of this one
                mTile[] valid = tileObjects[valOption].leftNeighbours;

                // add left valid options to array
                validOptions = validOptions.Concat(valid).ToList();
            }
        }


        if (allowedPositions.Contains(pos + Vector2Int.down))
        {
            // Find the cell right below the current one
            Cell down = gridComponents.Find(c => Vector2Int.FloorToInt((Vector2)c.transform.position) == pos + Vector2Int.down);
            foreach (mTile possibleOptions in down.tileOptions)
            {
                //get index of tile
                int valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);

                // Get which tiles can be placed up of this one
                mTile[] valid = tileObjects[valOption].upNeighbours;

                // add up valid options to array
                validOptions = validOptions.Concat(valid).ToList();
            }
        }

        if (allowedPositions.Contains(pos + Vector2Int.left))
        {
            // Find the cell to the left of the current one
            Cell left = gridComponents.Find(c => Vector2Int.FloorToInt((Vector2)c.transform.position) == pos + Vector2Int.left);
            foreach (mTile possibleOptions in left.tileOptions)
            {
                // get index of tile
                int valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);

                // Get which tiles can be placed right of this one
                mTile[] valid = tileObjects[valOption].rightNeighbours;

                // add up valid options to array
                validOptions = validOptions.Concat(valid).ToList();
            }
        }

        // Remove invalid options from the list
        CheckValidity(options, validOptions);
    }

    /// <summary>
    /// Removes from the option list the tiles not present in the "validOptions"
    /// </summary>
    void CheckValidity(List<mTile> optionList, List<mTile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
