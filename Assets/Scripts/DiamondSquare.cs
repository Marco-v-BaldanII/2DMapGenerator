using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

// Fractal map generation

public class DiamondSquare : MonoBehaviour
{
    float[,] map;

    public int sizeXY;

    public Tilemap tileMap;
    public Tile[] tiles;

    public float waitTime = 0.01f;

    Texture2D heightmapTexture;
    public string heightmapFilename = "heightmap.png";

    public float[] levels;

    public int InValues;

    public GameObject cellPrefab;

    // Recursive function
    void DimondSquareAlgorithm(int size)
    {

        int half = size / 2;
        if (half < 1) return; // Base case


        // theese loops are executed more as the recursive function goes on
        int times = 0;
        for(int y = half; y < sizeXY; y+= size)
        {
            for(int x= half; x < sizeXY; x+= size)
            {
                DimondStep(x, y, half);
                times++;
            }
        }
        Debug.Log("The diamond step was called " + times + " times");
        DimondSquareAlgorithm(half);
    }

    private void Start()
    {
        map = new float[sizeXY, sizeXY];
        heightmapTexture = new Texture2D(sizeXY, sizeXY, TextureFormat.RGBA32, false);


        // Setting the corner values
        map[0, 0] = InValues;
        map[sizeXY - 1, sizeXY - 1] = InValues;
        map[0, sizeXY - 1] = InValues;
        map[sizeXY - 1, 0] = InValues;

        GenerateHeightmap();
        GenerateTilemap();
        SaveHeightmapAsPNG();
    }

    void GenerateHeightmap()
    {
        float maxValue = 0;
        DimondSquareAlgorithm(sizeXY);

        // The values generated are random but for a height map they should be between 0 - 1
        for (int y = 0; y < sizeXY; ++y)
        {
            for (int x = 0; x < sizeXY; ++x)
            {
                if (map[x, y] > maxValue) maxValue = map[x, y];
            }
        }

        // Assign heights to texture
        for (int y = 0; y < sizeXY; ++y)
        {
            for (int x = 0; x < sizeXY; ++x)
            {
                float normalizedHeight = map[x, y] / maxValue;
                Color color = new Color(normalizedHeight, normalizedHeight, normalizedHeight);
                heightmapTexture.SetPixel(x, y, color);
            }
        }

        heightmapTexture.Apply();
    }

    void SaveHeightmapAsPNG()
    {
        byte[] pngBytes = heightmapTexture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath , heightmapFilename);
        File.WriteAllBytes(filePath, pngBytes);
        Debug.Log("Heightmap saved as PNG: " + filePath);
    }


    void GenerateTilemap() { 


         float maxValue = 0;


        for(int y = 0; y<sizeXY; ++y)
        {
            for(int x=0; x<sizeXY; ++x)
            {
                if (map[x, y] > maxValue) maxValue = map[x, y];
            }
        }
    
        for (int y = 0; y < sizeXY; ++y)
        {
            for (int x = 0; x < sizeXY; ++x)
            {
                float normalizedHeight = map[x, y] / maxValue;
                tileMap.SetTile(new Vector3Int(x, y, 0), SelectTile(normalizedHeight));
            }
        }
    }

    void GenMap()
    {
        float maxValue = 0;
        DimondSquareAlgorithm(sizeXY);

        // The values generated are random but for a heigh map they should be between 0 -1

        for(int y = 0; y <sizeXY; ++y)
        {
            for(int x=0; x < sizeXY; ++x)
            {
                if (map[x, y] > maxValue) maxValue = map[x, y];
            }
        }

        for (int y = 0; y < sizeXY; ++y)
        {
            for (int x = 0; x < sizeXY; ++x)
            {
                float v = 0;
                if(map[x,y] > 0)
                {
                    v = map[x, y] / maxValue;

                    tileMap.SetTile(new Vector3Int(x, y, 0), SelectTile(v));
                    
                }
            }
        }
    }

    Tile SelectTile(float p)
    {
        for(int i = 0; i < levels.Length; ++i)
        {
            // approximation where tiles with lower lvl (lower elevation) are assigned the first elements in the tiles array
            if (p <= levels[i]) return tiles[i];
        }
        return null;
    }

    void DimondStep(int x, int y, int half)
    {
        float value = 0.0f;

        value += map[x + half, y - half];
        value += map[x - half, y + half];
        value += map[x + half, y + half];
        value += map[x - half, y - half];

        value += Random.Range(0, half * 2) - half;
        value /= 4;

        map[x, y] = value;

        SquareStep(x - half, y, half);
        SquareStep(x + half, y, half);
        SquareStep(x , y - half, half);
        SquareStep(x , y + half, half);
    }


    void SquareStep(int x, int y, int half)
    {
        float value = 0.0f;
        int count = 0;

        // check if the 4 corners of the diamond exist

        if(x-half >= 0)
        {
            value += map[x - half, y];
            count++;
        }
        if(x + half < sizeXY)
        {
            value += map[x + half, y];
            count++;
        }
        if(y - half >= 0)
        {
            value += map[x, y - half];
            count++;
        }
        if(y + half < sizeXY)
        {
            value += map[x, y + half];
            count++;
        }
        // after adding their values take the average + add random seed

        value += Random.RandomRange(0, half * 2) - half;
        value /= count;

        map[x, y] = value;
    }
}
