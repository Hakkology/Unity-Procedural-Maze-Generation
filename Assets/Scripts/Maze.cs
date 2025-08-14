using System.Collections;
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
                wallLocations.Add(new MapLocation(x, z)); // Add all as wall locations
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
                    corridorLocations.Add(new MapLocation(x, z)); // Add to corridor list
                    wallLocations.RemoveAll(loc => loc.x == x && loc.z == z); // Remove from wall list
                }
            }
    }

    void DrawMap()
    {
        foreach (var wallLocation in wallLocations)
        {
            Vector3 pos = new Vector3(wallLocation.x * scale, 0, wallLocation.z * scale);
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(scale, scale, scale);
            wall.transform.position = pos;
        }
    }
}