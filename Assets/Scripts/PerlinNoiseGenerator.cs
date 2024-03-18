using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinNoiseGenerator : MonoBehaviour
{
    Dictionary<int, Tile> tileset;
    Dictionary<int, Tile> tile_groups;

    public Tile prefab_plains;
    public Tile prefab_forest;
    public Tile prefab_hill;
    public Tile prefab_mountain;

    int map_width = 160;
    int map_height = 90;

    public float waitSpeed = 0;

    List<List<int>> noise_grid = new List<List<int>>();

    //List<List<GameObject>> tile_grid = new List<List<GameObject>>();
    public Tilemap tileMap;

    public float magnification = 7.0f; // this is the frequency

    public int x_offset = 0;
    public int y_offset = 0;


    private void Start()
    { 
        CreateTileSet();
        x_offset = Random.Range(0, 1000);
        y_offset = Random.Range(0, 1000);
        GenerateMap();
    }



    void CreateTileSet()
    {

        tileset = new Dictionary<int, Tile>();

        // addd id   and list item for the dictionary
        tileset.Add(0, prefab_plains);
        tileset.Add(1, prefab_forest);
        tileset.Add(2, prefab_hill);
        tileset.Add(3, prefab_mountain);

        // this dictionary is just a list to access the different tile prefabs using their id

    }

    void CreateTileGroups()
    {

    }


    void GenerateMap()
    {
        for(int x = 0; x < map_width; ++x)
        {
            noise_grid.Add(new List<int>());
            
            for(int y = 0; y < map_height; ++y)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[x].Add(tile_id);
                PlaceTile(tile_id, x, y);
                // yield return new WaitForSeconds(waitSpeed);
                
            }

        }

    }

    int GetIdUsingPerlin(int x, int y)
    {

        // float perlin = MAthf.PerlinNoise(x , y);
        // the PerlinNoise method internally uses a grid to ensure that neighbouring points transition smoothly

        float perlin = Mathf.PerlinNoise((x - x_offset) / magnification, (y - y_offset) / magnification);

        perlin = Mathf.Clamp(perlin, 0, 1);

        perlin *= tileset.Count;

        if(perlin == tileset.Count) { perlin -= 1; }


        return Mathf.FloorToInt(perlin);


    }

    void PlaceTile(int key, int x, int y)
    {
        Tile actualTile = tileset[key];

        tileMap.SetTile(new Vector3Int(x, y, 0), actualTile);

        
    }

}

