
using UnityEngine;

public class Recursive : Maze
{
  public override void Generate()
  {
     Generate(Random.Range(1, width), Random.Range(1, depth)); 

  }

  public void Generate(int x, int z)
  {
      if (CountSquareNeighbours(x, z) >= 2) return;
      map[x,z] = 0;
    
  }
}
