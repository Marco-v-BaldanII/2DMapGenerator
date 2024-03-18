using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimensions;
    public mTile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    public Vector2Int startpos;

    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = startpos.y; y < dimensions +startpos.y; y++)
        {
            for (int x = startpos.x; x < dimensions + startpos.x; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
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
            if (tempGrid[i].tileOptions.Length < min && tempGrid[i].tileOptions.Length!= 0 && tempGrid[i].collapsed == false)
            {
                min = tempGrid[i].tileOptions.Length;
                lowestEntropyCell = tempGrid[i];
            }


        }

        yield return new WaitForSeconds(0.004f);
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
        if(cellToCollapse.tileOptions.Length <= 0)
        {
            Debug.Log("not?");
        }
        mTile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
        cellToCollapse.tileOptions = new mTile[] { selectedTile };

        mTile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;
                if (gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<mTile> options = new List<mTile>();
                    foreach (mTile t in tileObjects)
                    {
                        // Filled with all possible tiles
                        options.Add(t);
                    }

                    //update above
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<mTile> validOptions = new List<mTile>();

                        foreach (mTile possibleOptions in up.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].upNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //update right
                    if (x < dimensions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimensions];
                        List<mTile> validOptions = new List<mTile>();

                        foreach (mTile possibleOptions in right.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].leftNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look down
                    if (y < dimensions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimensions];
                        List<mTile> validOptions = new List<mTile>();

                        foreach (mTile possibleOptions in down.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].downNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimensions];
                        List<mTile> validOptions = new List<mTile>();

                        foreach (mTile possibleOptions in left.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].rightNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    mTile[] newTileList = new mTile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iterations++;

        if (iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }

    // Removes from the option list the tiles not present int the "validOptions"
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