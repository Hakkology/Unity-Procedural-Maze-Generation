using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonPathfinding : MonoBehaviour {
    public DungeonMaze maze;

    List<DungeonPathMarker> open = new();
    List<DungeonPathMarker> closed = new();

    public DungeonPathMarker goalNode;
    public DungeonPathMarker startNode;
    DungeonPathMarker lastPosition;

    bool done = false;

    void Start()
    {
    }

    public void Build()
    {
        BeginSearch();
        while (!done)
            Search(lastPosition);

        maze.InitialiseMap();
        MarkPath();
    }

    void BeginSearch()
    {
        done = false;

        List<MapLocation> locations = new List<MapLocation>();
        for (var z = 1; z < maze.depth; z++)
            for (var x = 1; x < maze.width; x++)
            {
                if (maze.map[x, z] != 1)
                    locations.Add(new MapLocation(x, z));
            }

        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        startNode = new DungeonPathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0, null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        goalNode = new DungeonPathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0, null);

        open.Clear();
        closed.Clear();

        open.Add(startNode);
        lastPosition = startNode;
    }

    void Search(DungeonPathMarker thisNode)
    {
        if (thisNode.Equals(goalNode)) { done = true; return; }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = dir + thisNode.Location;

            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (neighbour.x < 1 || neighbour.x >= maze.width || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
            if (IsClosed(neighbour)) continue;

            float G = Vector2.Distance(thisNode.Location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float H = Vector2.Distance(neighbour.ToVector(), goalNode.Location.ToVector());
            float F = G + H;

            if (!UpdateMarker(neighbour, G, H, F, thisNode))
                open.Add(new DungeonPathMarker(neighbour, G, H, F, thisNode));
        }

        open = open.OrderBy(p => p.F).ToList<DungeonPathMarker>();
        DungeonPathMarker pm = (DungeonPathMarker)open.ElementAt(0);
        closed.Add(pm);

        open.RemoveAt(0);
        lastPosition = pm;
    }

    private void MarkPath()
    {
        DungeonPathMarker begin = lastPosition;

        while (!startNode.Equals(begin) && begin != null)
        {
            maze.map[begin.Location.x, begin.Location.z] = 0;
            begin = begin.Parent;
        }
    }

    private void GetPath()
    {
        DungeonPathMarker begin = lastPosition;

        while (!startNode.Equals(begin) && begin != null)
        {
            begin = begin.Parent;
        }
    }


    bool IsClosed(MapLocation marker)
    {
        foreach (DungeonPathMarker p in closed)
        {
            if (p.Location.Equals(marker)) return true;
        }
        return false;
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, DungeonPathMarker parent)
    {
        foreach (DungeonPathMarker p in open)
        {
            if (p.Location.Equals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.Parent = parent;
                return true;
            }
        }
        return false;
    }
}