using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed = false;
    /// <summary>
    /// Possible tiles that can be placed in this cell
    /// </summary>
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
