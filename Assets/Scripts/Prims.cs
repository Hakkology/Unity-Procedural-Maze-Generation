using UnityEngine;

public class Prims : Maze 
{
    public override void Generate()
    {
        int x = 2;
        int z = 2;

        map[x, z] = 0;

        wallLocations.Add(new MapLocation(x + 1, z));
        wallLocations.Add(new MapLocation(x - 1, z));
        wallLocations.Add(new MapLocation(x, z + 1));
        wallLocations.Add(new MapLocation(x, z - 1));

        int countloops = 0;
        while (wallLocations.Count > 0 && countloops < 5000)
        {
            int rwall = Random.Range(0, wallLocations.Count);

            x = wallLocations[rwall.x];
            z = wallLocations[rwall.z];

            
            countloops++;
        }
    }

}