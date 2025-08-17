using System.Collections.Generic;
using UnityEngine;

public class MapLocation
{
    public int x;
    public int z;

    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}

public class Maze : MonoBehaviour
{
    public int width = 30; //x length
    public int depth = 30; //z length
    public byte[,] map;
    public int scale = 6;

    public List<MapLocation> wallLocations;
    public List<MapLocation> corridorLocations;

    void Start()
    {
        wallLocations = new List<MapLocation>();
        corridorLocations = new List<MapLocation>();

        InitialiseMap();
        Generate();
        DrawMap();
    }

    void InitialiseMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1; // Initially set all as walls
                // wallLocations.Add(new MapLocation(x, z)); // Add all as wall locations
            }
    }

    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                {
                    map[x, z] = 0; // Convert some walls to corridors
                    // corridorLocations.Add(new MapLocation(x, z)); // Add to corridor list
                    // wallLocations.RemoveAll(loc => loc.x == x && loc.z == z); // Remove from wall list
                }
            }
    }

    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                    wall.transform.position = pos;
                }
            }
    }

    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 1 || x >= width - 2 || z <= 1 || z >= depth - 2) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        if (map[x, z + 1] == 0) count++;

        return count;
    }

    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 1 || x >= width - 2 || z <= 1 || z >= depth - 2) return 5;
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;

        return count;
    }

    public int CountAllNeighbours(int x, int z)
    {
        return CountSquareNeighbours(x, z) + CountSquareNeighbours(x, z);
    }
}
