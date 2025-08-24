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
    [Header("Player Reference")]
    public GameObject Player;

    [Header("Maze piece References")]
    public GameObject straight;
    public GameObject crossroad;
    public GameObject deadend; 
    public GameObject cornerstraight; 
    public GameObject cornercurved; 
    public GameObject tjunction; 
    public GameObject floorpiece;
    public GameObject wallpiece;
    public GameObject ceilingpiece;

    [Header("Maze measurement Details")]
    public int width = 30; //x length
    public int depth = 30; //z length
    public int scale = 6;

    public List<MapLocation> wallLocations;
    public List<MapLocation> corridorLocations;
    public byte[,] map;

    bool top;
    bool bottom;
    bool right;
    bool left;

    void Start()
    {
        wallLocations = new List<MapLocation>();
        corridorLocations = new List<MapLocation>();

        InitialiseMap();
        GenerateMap();
        GenerateRooms(4, 3, 7);
        DrawMap();
        PlaceFPC();
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

    public virtual void GenerateMap()
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

    public virtual void GenerateRooms(int count, int minSize, int maxSize)
    {
        for (int c = 0; c < count; c++)
        {
            int startX = Random.Range(3, width - 3);
            int startZ = Random.Range(3, depth - 3);
            int roomWidth = Random.Range(minSize, maxSize);
            int roomDepth = Random.Range(minSize, maxSize);
            
            for (int x = startX; x < width - 3 && x < startX + roomWidth; x++)
            {
                for (int z = startZ; z < depth - 3 && z < startZ + roomDepth; z++)
                {
                    map[x, z] = 0;
                }
            }
        }
    }

    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * scale, 0, z * scale); // Save pos

                if (map[x, z] == 1)
                {

                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                    wall.transform.position = pos;
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 0, 5 })) // vertical straight
                {
                    // Procedural mace piece add code.

                    GameObject wall = Instantiate(straight, pos, Quaternion.Euler(0, 90, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 })) // horizontal straight
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(straight, pos, Quaternion.identity);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })) // Tjunction
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(crossroad, pos, Quaternion.identity);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 5, 1, 5 })) // horizontal right end
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(deadend, pos, Quaternion.Euler(0, 180, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 })) // horizontal left end
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(deadend, pos, Quaternion.identity);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 })) // vertical up end
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(deadend, pos, Quaternion.Euler(0, 90, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 1, 5 })) // vertical down end
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(deadend, pos, Quaternion.Euler(0, -90, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 }))
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(cornerstraight, pos, Quaternion.Euler(0, 180, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 }))
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(cornerstraight, pos, Quaternion.Euler(0, 90, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 }))
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(cornerstraight, pos, Quaternion.Euler(0, 0, 0));
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 }))
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(cornerstraight, pos, Quaternion.Euler(0, -90, 0));
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 5, 1, 5 }))
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(tjunction, pos, Quaternion.Euler(0, -90, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 1, 0, 1 }))
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(tjunction, pos, Quaternion.Euler(0, 90, 0));
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 0, 0, 1, 1, 0, 5 }))
                {
                    // Procedural mace piece add code.15
                    GameObject wall = Instantiate(tjunction, pos, Quaternion.Euler(0, 180, 0));
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 0, 1 }))
                {
                    // Procedural mace piece add code.
                    GameObject wall = Instantiate(tjunction, pos, Quaternion.Euler(0, 0, 0));
                }
                else if(map[x,z] == 0 && 
                        (CountDiagonalNeighbours(x,z) >= 1 && CountSquareNeighbours(x,z)>1 
                         || CountSquareNeighbours(x,z) >=1 && CountDiagonalNeighbours(x,z)>1))
                {
                    GameObject floor = Instantiate(floorpiece, pos, Quaternion.identity);
                    GameObject ceiling = Instantiate(ceilingpiece, pos, Quaternion.identity);
                }
                else
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.localScale = new Vector3(scale, scale, scale);
                    sphere.transform.position = pos;
                }
            }
    }

    public void LocateWalls(int x, int z)
    {
        top = false;
        bottom = false;
        left = false;
        right = false;

        if (x <= 0 || x >= width-1 || z <= 0 || z >= depth-1) return;
        if(map[x, z+1] == 1) top = true;
        if(map[x, z-1] == 1) bottom = true;

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
              {
                 count++; 
              } 
              pos++;
           } 
        }
        return count == 9;
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
        return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
    }
}
