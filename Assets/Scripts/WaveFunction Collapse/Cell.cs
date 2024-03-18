using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed = false;
    public mTile[] tileOptions;

 

    public void CreateCell(bool collapseState, mTile[] tiles)
    {
        collapsed = collapseState;
        tileOptions = tiles;
    }

    public void RecreateCell(mTile[] tiles)
    {
        tileOptions = tiles;
    }

    private void Update()
    {
       


    }
}
