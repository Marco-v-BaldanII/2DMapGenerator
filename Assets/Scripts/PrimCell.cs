using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimCell : MonoBehaviour
{


    public List<PrimCell> neighbours;

    public List<PrimCell> closeNeighbours;

    public PrimCell parentCell;

    private SpriteRenderer sprite;

    private BoxCollider2D box;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
    }

    public int weight;

    public bool visited = false;
    public bool inFrontier = false;
    public bool isStart = false; // Starting cell should not be added to the frontier

    public bool isEnd = false;

    public bool isBrother = false; // if it is a berother (passage) it should not have any neighbours
    public void Initialize(Vector2 pos, int weight)
    {
        this.transform.position = pos;
        this.weight = weight;
    }

    private void Update()
    {

        if (inFrontier) {
            sprite.color = Color.red;
        }

        if (visited)
        {
            box.enabled = false;
        }

        if (visited && !isEnd)
        {
            sprite.color = Color.white;
        }

        if (isEnd)
        {
            sprite.color = Color.yellow;
        }
    }

}
