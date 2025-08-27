using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
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
    public GameObject pathMarkingObject;

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
        if (Input.GetKeyDown(KeyCode.C) && !done) Search(lastPosition);
        if (Input.GetKeyDown(KeyCode.M)) GetPath();
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
            if (IsClosed(neighbour)) continue;

            float G = Vector2.Distance(thisNode.Location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float H = Vector2.Distance(neighbour.ToVector(), goalNode.Location.ToVector());
            float F = G + H;

            GameObject pathBlock = Instantiate(pathMarkingObject, new Vector3(neighbour.x * maze.scale, 0, neighbour.z * maze.scale), Quaternion.identity);
            UpdateMarkerText(pathBlock, G, H, F);

            if (!UpdateMarker(neighbour, G, H, F, thisNode))
                open.Add(new PathMarker(neighbour, G, H, F, pathBlock, thisNode));
        }

        open = open.OrderBy(p => p.F).ToList<PathMarker>();
        PathMarker pm = (PathMarker)open.ElementAt(0);
        closed.Add(pm);

        open.RemoveAt(0);
        pm.Marker.GetComponent<Renderer>().material = closedMaterial;
        lastPosition = pm;
    }

    private void GetPath()
    {
        RemoveAllMarkers();
        PathMarker begin = lastPosition;

        while (!startNode.Equals(begin) && begin != null)
        {
            Instantiate(pathMarkingObject,
                        new Vector3(begin.Location.x * maze.scale, 0, begin.Location.z * maze.scale),
                        Quaternion.identity);
            begin = begin.Parent;
        }

        Instantiate(pathMarkingObject,
                        new Vector3(startNode.Location.x * maze.scale, 0, startNode.Location.z * maze.scale),
                        Quaternion.identity);
    }


    bool IsClosed(MapLocation marker)
    {
        foreach (PathMarker p in closed)
        {
            if (p.Location.Equals(marker)) return true;
        }
        return false;
    }

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (var m in markers)
            Destroy(m);
    }

    void UpdateMarkerText(GameObject block, float g, float h, float f)
    {
        TextMesh[] values = block.GetComponentsInChildren<TextMesh>();
        values[0].text = "G: " + g.ToString("0.00");
        values[1].text = "H: " + h.ToString("0.00");
        values[2].text = "F: " + f.ToString("0.00");
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker parent)
    {
        foreach (PathMarker p in open)
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