using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonMaze : MonoBehaviour
{
    public List<MapLocation> directions = new List<MapLocation>()
    {
        new MapLocation(1, 0),
        new MapLocation(-1, 0),
        new MapLocation(0, 1),
        new MapLocation(0, -1),
    };

    [Header("Player Reference")]
    public GameObject Player;

    [Header("Straight piece References")]
    public Module VerticalStraight;
    public Module HorizontalStraight;

    [Header("Crossroad piece References")]
    public Module Crossroad;

    [Header("Endpiece piece References")]
    public Module Endpiece;
    public Module EndpieceUpsideDown;
    public Module EndpieceRight;
    public Module EndpieceLeft;

    [Header("Corner piece References")]
    public Module RightUpCorner;
    public Module RightDownCorner;
    public Module LeftUpCorner;
    public Module LeftDownCorner;
    // public Module cornercurved;

    [Header("T-Intersection piece References")]
    public Module TIntersection;
    public Module TIntersectionUpsideDown;
    public Module TIntersectionLeft;
    public Module TIntersectionRight;

    [Header("Wall piece References")]
    public Module WallpieceTop;
    public Module WallpieceBottom;
    public Module WallpieceRight;
    public Module WallpieceLeft;

    [Header("Doorway piece References")]
    public Module DoorTop;
    public Module DoorBottom;
    public Module DoorRight;
    public Module DoorLeft;

    [Header("Remaining Room piece References")]
    public Module Floorpiece;
    public Module Ceilingpiece;
    public Module Pillarpiece;

    [Header("Maze measurement Details")]
    public int width = 30; //x length
    public int depth = 30; //z length
    public int scale = 6;
    public int level = 0;
    public float levelDistance = 2.0f;
    public float xOffset = 0;
    public float zOffset = 0;
    public bool IsPathfinding;

    public List<MapLocation> locations = new List<MapLocation>();
    // public List<MapLocation> wallLocations;
    // public List<MapLocation> corridorLocations;
    public List<MapLocation> pillarLocations = new();

    public byte[,] map;
    public Pieces[,] piecePlaces;
    public MapLocation entryPoint;
    public MapLocation exitPoint;

    private DungeonPathfinding dungeonPathfinding;
    bool top;
    bool bottom;
    bool right;
    bool left;

    void Awake() 
    {
        dungeonPathfinding = GetComponent<DungeonPathfinding>();
    }

    void Start()
    {
        // wallLocations = new List<MapLocation>();
        // corridorLocations = new List<MapLocation>();
    }

    public void Build()
    {
        InitialiseMap();
        GenerateMap();
        GenerateRooms(5, 4, 10);

        byte[,] oldmap = map;
        int oldWidth = width;
        int oldDepth = depth;

        width += 6;
        depth += 6;

        map = new byte[width, depth];
        InitialiseMap();

        for (int z = 0; z < oldDepth; z++)
            for (int x = 0; x < oldWidth; x++)
            {
                map[x + 3, z + 3] = oldmap[x, z];
            }

        int xpos;
        int zpos;


        if (dungeonPathfinding != null && IsPathfinding)
        {
            dungeonPathfinding.Build();
            if (dungeonPathfinding.startNode.Location.x < dungeonPathfinding.goalNode.Location.x) //start is left
            {
                xpos = dungeonPathfinding.startNode.Location.x;
                zpos = dungeonPathfinding.startNode.Location.z;

                while (xpos > 1)
                {
                    map[xpos, zpos] = 0;
                    xpos--;
                }

                xpos = dungeonPathfinding.goalNode.Location.x;
                zpos = dungeonPathfinding.goalNode.Location.z;

                while (xpos < width - 2)
                {
                    map[xpos, zpos] = 0;
                    xpos++;
                }
            }
            else
            {
                xpos = dungeonPathfinding.startNode.Location.x;
                zpos = dungeonPathfinding.startNode.Location.z;

                while (xpos < width - 2)
                {
                    map[xpos, zpos] = 0;
                    xpos++;
                }

                xpos = dungeonPathfinding.goalNode.Location.x;
                zpos = dungeonPathfinding.goalNode.Location.z;

                while (xpos > 1)
                {
                    map[xpos, zpos] = 0;
                    xpos--;
                }

            }

        }
        else
        {
            //upper vertical corridor
            xpos = Random.Range(5, width - 5);
            zpos = depth - 2;

            while (map[xpos, zpos] != 0 && zpos > 1)
            {
                map[xpos, zpos] = 0;
                zpos--;
            }

            //lower vertical corridor
            xpos = Random.Range(5, width - 5);
            zpos = 1;

            while (map[xpos, zpos] != 0 && zpos < depth - 2)
            {
                map[xpos, zpos] = 0;
                zpos++;
            }

            //right horizontal corridor
            zpos = Random.Range(5, depth - 5);
            xpos = width - 2;

            while (map[xpos, zpos] != 0 && xpos > 1)
            {
                map[xpos, zpos] = 0;
                xpos--;
            }

            //left horizontal corridor
            zpos = Random.Range(5, depth - 5);
            xpos = 1;

            while (map[xpos, zpos] != 0 && xpos < width - 2)
            {
                map[xpos, zpos] = 0;
                xpos++;
            }
        }

        DrawMap();
        GenerateObjects();

        if (Player != null)
            PlaceFPC();

        SetMapCoordinates();
    }

    private void SetMapCoordinates()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                GameObject go = piecePlaces[x, z]._model;
                if (go != null)
                {
                    go.GetComponent<DungeonMapLocation>().x = x;
                    go.GetComponent<DungeonMapLocation>().z = z;
                }
                if (map[x, z] != 1)
                {
                    locations.Add(new MapLocation(x, z));
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

    public void InitialiseMap()
    {
        map = new byte[width, depth];
        piecePlaces = new Pieces[width, depth];
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

    public virtual void GenerateObjects()
    {
        DungeonPlaceObject[] objectspawners = GetComponents<DungeonPlaceObject>();
        if (objectspawners.Length > 0)
            foreach (var spawner in objectspawners)
                spawner.SpawnObject();
    }

    public void DrawMap()
    {
        int height = (int)(level * scale * levelDistance);
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * scale, height, z * scale); // Save pos

                if (map[x, z] == 1)
                {

                    // GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // wall.transform.localScale = new Vector3(scale, scale, scale);
                    // wall.transform.position = pos;
                    AssignPiece(new MapLocation(x, z), PieceType.Wall, null);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 0, 5 })) // vertical straight
                {
                    // GameObject piece = Instantiate(straight, pos, Quaternion.Euler(0, 90, 0));
                    GameObject piece = SpawnPiece(VerticalStraight, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.Vertical_Straight, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 })) // horizontal straight
                {
                    // GameObject piece = Instantiate(straight, pos, Quaternion.identity);
                    GameObject piece = SpawnPiece(HorizontalStraight, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.Horizontal_Straight, piece);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })) // Crossroads
                {
                    // Procedural mace piece add code.
                    // GameObject piece = Instantiate(crossroad, pos, Quaternion.identity);
                    GameObject piece = SpawnPiece(Crossroad, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.Crossroad, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 5, 1, 5 })) // horizontal right end
                {
                    // GameObject piece = Instantiate(deadend, pos, Quaternion.Euler(0, 180, 0));
                    GameObject piece = SpawnPiece(EndpieceRight, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.DeadToright, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 })) // horizontal left end
                {
                    // GameObject piece = Instantiate(deadend, pos, Quaternion.identity);
                    GameObject piece = SpawnPiece(EndpieceLeft, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.DeadToLeft, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 })) // vertical up end
                {
                    // GameObject piece = Instantiate(deadend, pos, Quaternion.Euler(0, 90, 0));
                    GameObject piece = SpawnPiece(Endpiece, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.DeadEnd, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 1, 5 })) // vertical down end
                {
                    // GameObject piece = Instantiate(deadend, pos, Quaternion.Euler(0, -90, 0));
                    GameObject piece = SpawnPiece(EndpieceUpsideDown, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.DeadUpsideDown, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 })) // upper left corner
                {
                    // GameObject piece = Instantiate(cornerstraight, pos, Quaternion.Euler(0, 180, 0));
                    GameObject piece = SpawnPiece(LeftUpCorner, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.Left_Up_Corner, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 })) // upper right corner
                {
                    // GameObject piece = Instantiate(cornerstraight, pos, Quaternion.Euler(0, 90, 0));
                    GameObject piece = SpawnPiece(RightUpCorner, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.Right_Up_Corner, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 })) // lower right corner
                {
                    // GameObject piece = Instantiate(cornerstraight, pos, Quaternion.Euler(0, 0, 0));
                    GameObject piece = SpawnPiece(RightDownCorner, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.Right_Down_Corner, piece);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 })) // lower left corner
                {
                    // GameObject piece = Instantiate(cornerstraight, pos, Quaternion.Euler(0, -90, 0));
                    GameObject piece = SpawnPiece(LeftDownCorner, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.Left_Down_Corner, piece);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 5, 1, 5 })) // tjunction reverse
                {
                    // GameObject piece = Instantiate(tjunction, pos, Quaternion.Euler(0, -90, 0));
                    GameObject piece = SpawnPiece(TIntersectionUpsideDown, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.TUpsideDown, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 1, 0, 1 })) // Tjunction
                {
                    // GameObject piece = Instantiate(tjunction, pos, Quaternion.Euler(0, 90, 0));
                    GameObject piece = SpawnPiece(TIntersection, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.T_Junction, piece);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 0, 0, 1, 1, 0, 5 })) // tjunction right
                {
                    // GameObject piece = Instantiate(tjunction, pos, Quaternion.Euler(0, 180, 0));
                    GameObject piece = SpawnPiece(TIntersectionRight, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.TToRight, piece);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 0, 1 })) // tjunction left
                {
                    // GameObject piece = Instantiate(tjunction, pos, Quaternion.Euler(0, 0, 0));
                    GameObject piece = SpawnPiece(TIntersectionLeft, pos);
                    AssignPiece(new MapLocation(x, z), PieceType.TToLeft, piece);
                }
                else if (map[x, z] == 0 &&
                        (CountDiagonalNeighbours(x, z) >= 1 && CountSquareNeighbours(x, z) > 1
                         || CountSquareNeighbours(x, z) >= 1 && CountDiagonalNeighbours(x, z) > 1))
                {
                    GameObject floor = Instantiate(Floorpiece.prefab, pos, Quaternion.identity, this.transform);
                    GameObject ceiling = Instantiate(Ceilingpiece.prefab, pos, Quaternion.identity, this.transform);

                    AssignPiece(new MapLocation(x, z), PieceType.Room, floor);

                    GameObject pillarCorner;
                    Vector3 enlargingPillarScale = new Vector3(1.01f, 1, 1.01f);
                    LocateWalls(x, z);
                    if (top)
                    {
                        // GameObject walltop = Instantiate(wallpiece, pos, Quaternion.Euler(0, 0, 0));
                        GameObject walltop = SpawnPiece(WallpieceTop, pos);

                        if (map[x + 1, z] == 0 && map[x + 1, z + 1] == 0 && !pillarLocations.Contains(new MapLocation(x, z)))
                        {
                            pillarCorner = Instantiate(Pillarpiece.prefab, pos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x, z));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }

                        if (map[x - 1, z] == 0 && map[x - 1, z + 1] == 0 && !pillarLocations.Contains(new MapLocation(x - 1, z)))
                        {
                            var newPos = pos + new Vector3(-1 * scale, 0, 0);
                            pillarCorner = Instantiate(Pillarpiece.prefab, newPos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x - 1, z));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }
                    }

                    if (bottom)
                    {
                        // GameObject wallbottom = Instantiate(wallpiece, pos, Quaternion.Euler(0, 180, 0));
                        GameObject wallbottom = SpawnPiece(WallpieceBottom, pos);

                        if (map[x + 1, z] == 0 && map[x + 1, z - 1] == 0 && !pillarLocations.Contains(new MapLocation(x, z - 1)))
                        {
                            var newPos = pos + new Vector3(0, 0, -1 * scale);
                            pillarCorner = Instantiate(Pillarpiece.prefab, newPos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x, z - 1));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }

                        if (map[x - 1, z] == 0 && map[x - 1, z - 1] == 0 && !pillarLocations.Contains(new MapLocation(x - 1, z - 1)))
                        {
                            var newPos = pos + new Vector3(-1 * scale, 0, -1 * scale);
                            pillarCorner = Instantiate(Pillarpiece.prefab, newPos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x - 1, z - 1));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }
                    }

                    if (right)
                    {
                        // GameObject wallright = Instantiate(wallpiece, pos, Quaternion.Euler(0, 90, 0));
                        GameObject wallright = SpawnPiece(WallpieceRight, pos);
                        if (map[x + 1, z + 1] == 0 && map[x, z + 1] == 0 && !pillarLocations.Contains(new MapLocation(x, z - 1)))
                        {
                            var newPos = pos + new Vector3(0, 0, -1 * scale);
                            pillarCorner = Instantiate(Pillarpiece.prefab, newPos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x, z - 1));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }

                        if (map[x, z - 1] == 0 && map[x + 1, z - 1] == 0 && !pillarLocations.Contains(new MapLocation(x - 1, z - 1)))
                        {
                            var newPos = pos + new Vector3(1 * scale, 0, -1 * scale);
                            pillarCorner = Instantiate(Pillarpiece.prefab, newPos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x - 1, z - 1));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }
                    }

                    if (left)
                    {
                        // GameObject wallleft = Instantiate(wallpiece, pos, Quaternion.Euler(0, -90, 0));
                        GameObject wallleft = SpawnPiece(WallpieceLeft, pos);

                        if (map[x - 1, z + 1] == 0 && map[x, z + 1] == 0 && !pillarLocations.Contains(new MapLocation(x - 1, z)))
                        {
                            var newPos = pos + new Vector3(-1 * scale, 0, 0);
                            pillarCorner = Instantiate(Pillarpiece.prefab, newPos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x - 1, z));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }

                        if (map[x - 1, z - 1] == 0 && map[x, z - 1] == 0 && !pillarLocations.Contains(new MapLocation(x - 1, z - 1)))
                        {
                            var newPos = pos + new Vector3(-1 * scale, 0, -1 * scale);
                            pillarCorner = Instantiate(Pillarpiece.prefab, newPos, Quaternion.identity, this.transform);
                            pillarLocations.Add(new MapLocation(x - 1, z - 1));
                            pillarCorner.transform.localScale = enlargingPillarScale;
                        }
                    }
                }
                else
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.localScale = new Vector3(scale, scale, scale);
                    sphere.transform.position = pos;
                }
            }

        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (piecePlaces[x, z]._piece != PieceType.Room) continue;

                Vector3 pos = new Vector3(x * scale, height, z * scale); // Save pos
                GameObject doorway;
                LocateDoors(x, z);

                Vector3 newDoorwayPos = pos + new Vector3(0, 0, 0.01f);
                if (top)
                {
                    // doorway = Instantiate(doorwaypiece, newDoorwayPos, Quaternion.Euler(0, 180, 0));
                    doorway = SpawnPiece(DoorTop, newDoorwayPos);
                }

                if (bottom)
                {
                    // doorway = Instantiate(doorwaypiece, newDoorwayPos, Quaternion.Euler(0, 0, 0));
                    doorway = SpawnPiece(DoorBottom, newDoorwayPos);
                }

                if (left)
                {
                    // doorway = Instantiate(doorwaypiece, newDoorwayPos, Quaternion.Euler(0, 90, 0));
                    doorway = SpawnPiece(DoorLeft, newDoorwayPos);
                }

                if (right)
                {
                    // doorway = Instantiate(doorwaypiece, newDoorwayPos, Quaternion.Euler(0, 270, 0));
                    doorway = SpawnPiece(DoorRight, newDoorwayPos);
                }
            }
    }

    public void LocateWalls(int x, int z)
    {
        top = false;
        bottom = false;
        left = false;
        right = false;

        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return;
        if (map[x, z + 1] == 1) top = true;
        if (map[x, z - 1] == 1) bottom = true;
        if (map[x + 1, z] == 1) right = true;
        if (map[x - 1, z] == 1) left = true;
    }

    public void LocateDoors(int x, int z)
    {
        top = false;
        bottom = false;
        left = false;
        right = false;

        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return;
        if (piecePlaces[x, z + 1]._piece != PieceType.Room && piecePlaces[x, z + 1]._piece != PieceType.Wall) top = true;
        // if (map[x, z + 1] == 0 && map[x - 1, z + 1] == 1 && map[x + 1, z + 1] == 1) top = true;
        if (piecePlaces[x, z - 1]._piece != PieceType.Room && piecePlaces[x, z - 1]._piece != PieceType.Wall) bottom = true;
        // if (map[x, z - 1] == 0 && map[x - 1, z - 1] == 1 && map[x + 1, z - 1] == 1) bottom = true;
        if (piecePlaces[x + 1, z]._piece != PieceType.Room && piecePlaces[x + 1, z]._piece != PieceType.Wall) right = true;
        // if (map[x + 1, z] == 0 && map[x + 1, z + 1] == 1 && map[x + 1, z - 1] == 1) right = true;
        if (piecePlaces[x - 1, z]._piece != PieceType.Room && piecePlaces[x - 1, z]._piece != PieceType.Wall) left = true;
        // if (map[x - 1, z] == 0 && map[x - 1, z + 1] == 1 && map[x - 1, z - 1] == 1) left = true;
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

    private void AssignPiece(MapLocation loc, PieceType type, GameObject model)
    {
        piecePlaces[loc.x, loc.z]._piece = type;
        piecePlaces[loc.x, loc.z]._model = model;
        // piecePlaces[loc.x, loc.z]._model.GetComponent<DungeonMapLocation>().x = loc.x;
        // piecePlaces[loc.x, loc.z]._model.GetComponent<DungeonMapLocation>().z = loc.z;
    }

    private GameObject SpawnPiece(Module module, Vector3 position)
    {
        return Instantiate(module.prefab, position, Quaternion.Euler(module.rotation), this.transform);
    }
}
