
using System.Collections.Generic;
using UnityEngine;

public class Wilsons : Maze
{
    List<MapLocation> directions = new List<MapLocation>()
    {
        new MapLocation(1, 0),
        new MapLocation(-1, 0),
        new MapLocation(0, 1),
        new MapLocation(0, -1),
    };

    List<MapLocation> notUsed = new List<MapLocation>();
    
    public override void Generate()
    {
        int x = Random.Range(2, width - 1);
        int z = Random.Range(2, depth - 1);
        map[x, z] = 2;

        int runWalkAttempts = width * depth / 3;

        while(GetAvailableCells() > 1 && runWalkAttempts > 0)
        {
            RandomWalk();
            runWalkAttempts--;
        }
    }

    private int CountSquareMazeNeighbours(int x, int z)
    {
        int count = 0;

        for (int d = 0; d < directions.Count; d++)
        {
            int neighboursx = x + directions[d].x;
            int neighboursz = z + directions[d].z;
            if (map[neighboursx, neighboursz] == 2)
            {
                count++;
            }
        }
        return count;
    }

    private int GetAvailableCells()
    {
      notUsed.Clear();
      for (int z = 1; z < depth - 1; z++)
      {
         for (int x = 1; x < width -1; x++)
         {
           if (CountSquareMazeNeighbours(x, z) == 0)
           {
               notUsed.Add(new MapLocation(x, z));
           }
         } 
      }
      return notUsed.Count;
    }

    private void RandomWalk()
    {
        List<MapLocation> inWalk = new List<MapLocation>();

        int rStartIndex = Random.Range(0, notUsed.Count);
        int currentx = notUsed[rStartIndex].x;
        int currentz = notUsed[rStartIndex].z;

        inWalk.Add(new MapLocation(currentx, currentz));

        int loopCount = 0;
        bool validPath = false;
        while (currentx > 0 && currentx < width - 1 && currentz > 0 && currentz < depth - 1 && loopCount < (width * depth) && !validPath)
        {
            loopCount++; // loop check

            map[currentx, currentz] = 0;
            if (CountSquareMazeNeighbours(currentx, currentz) > 1)
                break;

            int randomDirection = Random.Range(0, directions.Count);
            int neighboursx = currentx + directions[randomDirection].x;
            int neighboursz = currentz + directions[randomDirection].z;

            if (CountSquareNeighbours(neighboursx, neighboursz) < 2)
            {
                currentx = neighboursx;
                currentz = neighboursz;
                inWalk.Add(new MapLocation(currentx, currentz));
            }
            validPath = CountSquareMazeNeighbours(currentx, currentz) == 1;
            // currentx += directions[randomDirection].x;
            // currentz += directions[randomDirection].z;
        }

        if (validPath)
        {
            map[currentx, currentz] = 0;
            Debug.Log("PathFound");

            foreach (MapLocation mlocation in inWalk)
                map[mlocation.x, mlocation.z] = 2;
            inWalk.Clear();
        }
        else
        {
            foreach (MapLocation mlocation in inWalk)
                map[mlocation.x, mlocation.z] = 1;
            inWalk.Clear();
        }
    }
}
