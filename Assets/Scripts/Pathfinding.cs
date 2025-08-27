using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public PathfindingMaze maze;
    public Material closedMaterial;
    public Material openMaterial;

    List<PathMarker> open = new();
    List<PathMarker> closed = new();

    public GameObject start;
    public GameObject end;
    public GameObject pathMarker;

    PathMarker goalNode;
    PathMarker startNode;
    PathMarker lastPosition;

    bool done = false;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) BeginSearch();
    }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        List<MapLocation> locations = new List<MapLocation>();
        for (var z = 1; z < maze.depth; z++)
            for (var x = 1; x < maze.width; x++)
            {
                if (maze.map[x, z] != 1)
                    locations.Add(new MapLocation(x, z));
            }

        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0, Instantiate(start, startLocation, Quaternion.identity), null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0, Instantiate(end, goalLocation, Quaternion.identity), null);

        open.Clear();
        closed.Clear();

        open.Add(startNode);
        lastPosition = startNode;
    }

    void Search(PathMarker thisNode)
    {
        if (thisNode.Equals(goalNode)) { done = true; return; }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = dir + thisNode.Location;

            if (!neighbour.IsWithinMaze(maze.width, maze.depth) || maze.map[neighbour.x, neighbour.z] == 1) continue;
            

        }
    }

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (var m in markers)
            Destroy(m);
    }
}