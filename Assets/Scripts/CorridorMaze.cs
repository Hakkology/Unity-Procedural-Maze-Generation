using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorMaze : MonoBehaviour
{
    [Header("Player Reference")]
    public GameObject Player;

    public List<MapLocation> directions = new List<MapLocation>() {
                                            new MapLocation(1,0),
                                            new MapLocation(0,1),
                                            new MapLocation(-1,0),
                                            new MapLocation(0,-1) };
    public int width = 30; //x length
    public int depth = 30; //z length
    public byte[,] map;
    public int scale = 6;

    [Header("Straight piece References")]
    public Module VerticalStraight;
    public Module HorizontalStraight;

    [Header("Crossroad piece References")]
    public Module Crossroad;

    [Header("Corner piece References")]
    public Module RightUpCorner;
    public Module RightDownCorner;
    public Module LeftUpCorner;
    public Module LeftDownCorner;

    [Header("Intersection piece References")]
    public Module TIntersection;
    public Module TIntersectionUpsideDown;
    public Module TIntersectionLeft;
    public Module TIntersectionRight;

    [Header("End piece References")]
    public Module Endpiece;
    public Module EndpieceUpsideDown;
    public Module EndpieceRight;
    public Module EndpieceLeft;


    // Start is called before the first frame update
    void Start()
    {
        InitialiseMap();
        GenerateMap();
        DrawMap();
        PlaceFPC();
    }

    void InitialiseMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;     //1 = wall  0 = corridor
            }
    }

    public virtual void GenerateMap()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                    map[x, z] = 0;     //1 = wall  0 = corridor
            }
    }

    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * scale, 0, z * scale);
                if (map[x, z] == 1)
                {
                    //Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    //GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //wall.transform.localScale = new Vector3(scale, scale, scale);
                    // wall.transform.position = pos;
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 5, 1, 5 })) //horizontal end piece -|
                {
                    // GameObject block = Instantiate(endpiece);
                    GameObject block = SpawnPiece(EndpieceLeft, pos);
                    // block.transform.position = pos;
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 })) //horizontal end piece |-
                {
                    // GameObject block = Instantiate(endpiece);
                    GameObject block = SpawnPiece(EndpieceRight, pos);
                    // block.transform.position = new Vector3(x * scale, 0, z * scale);
                    // block.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 })) //vertical end piece T
                {
                    GameObject block = SpawnPiece(Endpiece, pos);
                    // block.transform.position = new Vector3(x * scale, 0, z * scale);
                    // block.transform.Rotate(0, -90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 1, 5 })) //vertical end piece upside downT
                {
                    GameObject block = SpawnPiece(EndpieceUpsideDown, pos);
                    // block.transform.position = new Vector3(x * scale, 0, z * scale);
                    // block.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 0, 5 })) //vertical straight
                {
                    GameObject go = SpawnPiece(VerticalStraight, pos);
                    // Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    // Instantiate(straight, pos, Quaternion.identity);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 })) //horizontal straight
                {
                    // Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject go = SpawnPiece(HorizontalStraight, pos);
                    // go.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })) //crossroad
                {
                    GameObject go = SpawnPiece(Crossroad, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 })) //upper left corner
                {
                    GameObject go = SpawnPiece(LeftUpCorner, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                    // go.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 })) //upper right corner
                {
                    GameObject go = SpawnPiece(RightUpCorner, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                    // go.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 })) //lower right corner
                {
                    GameObject go = SpawnPiece(LeftDownCorner, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 })) //lower left corner
                {
                    GameObject go = SpawnPiece(LeftDownCorner, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                    // go.transform.Rotate(0, -90, 0);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 5, 1, 5 })) //tjunc  upsidedown T
                {
                    GameObject go = SpawnPiece(TIntersectionUpsideDown, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                    // go.transform.Rotate(0, -90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 1, 0, 1 })) //tjunc  T
                {
                    GameObject go = SpawnPiece(TIntersection, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                    // go.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 0, 0, 1, 1, 0, 5 })) //tjunc  -|
                {
                    GameObject go = SpawnPiece(TIntersectionLeft, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                    // go.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 0, 1 })) //tjunc  |-
                {
                    GameObject go = SpawnPiece(TIntersectionRight, pos);
                    // go.transform.position = new Vector3(x * scale, 0, z * scale);
                }


            }
    }

    public virtual void PlaceFPC()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 0)
                {
                    Player.transform.position = new Vector3(x * scale, 1f, z * scale);
                    return;
                }
            }
    }

    bool Search2D(int c, int r, int[] pattern)
    {
        int count = 0;
        int pos = 0;
        for (int z = 1; z > -2; z--)
        {
            for (int x = -1; x < 2; x++)
            {
                if (pattern[pos] == map[c + x, r + z] || pattern[pos] == 5)
                    count++;
                pos++;
            }
        }
        return (count == 9);
    }

    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    public int CountAllNeighbours(int x, int z)
    {
        return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
    }
    
    private GameObject SpawnPiece(Module module, Vector3 position)
    {
        return Instantiate(module.prefab, position, Quaternion.Euler(module.rotation));
    }
}
