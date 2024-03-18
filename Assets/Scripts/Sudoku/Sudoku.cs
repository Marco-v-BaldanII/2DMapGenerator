using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEngine.UI;

public class Sudoku : MonoBehaviour
{


    public Tilemap tileMap;
    public Tile[] tile;

    int sizeXY = 9;

    private Button myButton;

    public SudokuCell[] myCells;

    public int[,] values;

    public Transform mapPosition;


    // Start is called before the first frame update
    void Start()
    {

        values = new int[9, 9];
        //myCells = new SudokuCell[9];

        //int i = 0;
        //foreach(Transform son in this.transform)
        //{
        //    SudokuCell cell = son.GetComponent<SudokuCell>();
        //    if(cell != null)
        //    {
        //        myCells[i] = cell;
        //        ++i;
        //    }
        //}
        myButton = GetComponentInChildren<Button>();

        myButton.onClick.AddListener(GenerateMatrix);


    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            // rEAD ALL INOUTS







        }

        

    }


    private void ActivateMap()
    {
        bool complete = true;
        for(int i = 0; i < 9; ++i)
        {
            if (!myCells[i].filled)
            {
                complete = false;
            }
        }
        if (complete)
        {
            for (int y = 0; y < sizeXY; ++y)
            {
                for (int x = 0; x < sizeXY; ++x)
                {

                    tileMap.SetTile(new Vector3Int(x, y, 0), tile[0]);
                }
            }


        }
    }


    void GenerateMatrix()
    {
        bool finished = true;
        for(int i =0;i < 9; ++i)
        {
           if(!myCells[i].filled)
            {
                finished = false;
            }
        }

        if (finished)
        {
            for(int i=0;i <9; ++i)
            {


                for(int x=0;x<9; ++x)
                {
                    int index = int.Parse(myCells[i].inputField[x].text) -1;
                    tileMap.SetTile(new Vector3Int(- myCells[i].positions[x].x +10,- myCells[i].positions[x].y +10, 0), tile[index]);




                }


            }




        }

        //tileMap.transform.position = mapPosition.position;
        tileMap.transform.localScale *= 2;

        this.gameObject.SetActive(false);

    }

}
