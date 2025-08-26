using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("Maze Detail References")]
    public int width = 30;
    public int depth = 30;

    [Header("Maze piece References")]
    public DungeonMaze[] mazes;

    [Header("Ladder piece References")]
    public GameObject Stairwell;

    private List<MapLocation> level1ends = new();
    private List<MapLocation> level2ends = new();


    void Start()
    {
        int level = 0;
        foreach (DungeonMaze maze in mazes)
        {
            maze.width = width;
            maze.depth = depth;
            maze.level = level++;
            maze.levelDistance = 1.5f;
            maze.Build();
        }

        for (int mazelvl = 0; mazelvl < mazes.Length - 1; mazelvl++)
        {
            level1ends.Clear();
            level2ends.Clear();
            for (int z = 0; z < depth; z++)
                for (int x = 0; x < width; x++)
                {
                    // if (mazes[mazelvl].piecePlaces[x, z]._piece == PieceType.DeadToLeft && mazes[mazelvl +1].piecePlaces[x, z]._piece == PieceType.DeadToright)
                    // {
                    //     Destroy(mazes[mazelvl].piecePlaces[x, z]._model);
                    //     Destroy(mazes[mazelvl+1].piecePlaces[x, z]._model);
                    //     Vector3 stairPosition = new Vector3(mazes[mazelvl].scale * x, mazes[mazelvl].scale * mazes[mazelvl].level * mazes[mazelvl].levelDistance, mazes[mazelvl].scale * z);
                    //     mazes[mazelvl].piecePlaces[x, z]._model = Instantiate(Stairwell, stairPosition, Quaternion.identity);
                    //     mazes[mazelvl].piecePlaces[x, z]._piece = PieceType.Ladder;
                    //     mazes[mazelvl+1].piecePlaces[x, z]._piece = PieceType.Ladder;
                    // }

                    if (mazes[mazelvl].piecePlaces[x, z]._piece == PieceType.DeadToLeft)
                    {
                        level1ends.Add(new MapLocation(x, z));
                    }

                    if (mazes[mazelvl + 1].piecePlaces[x, z]._piece == PieceType.DeadToright)
                    {
                        level2ends.Add(new MapLocation(x, z));
                    }
                }

            if (!level1ends.Any() && !level2ends.Any()) break;

            MapLocation bottomOfStairs = level1ends[Random.Range(0, level1ends.Count)];
            MapLocation topOfStairs = level2ends[Random.Range(0, level2ends.Count)];

            mazes[mazelvl + 1].xOffset = bottomOfStairs.x - topOfStairs.x + mazes[mazelvl].xOffset;
            mazes[mazelvl + 1].zOffset = bottomOfStairs.z - topOfStairs.z + mazes[mazelvl].zOffset;

            Vector3 stairBottomPosition = new Vector3(mazes[mazelvl].scale * bottomOfStairs.x,
                                                        mazes[mazelvl].scale * mazes[mazelvl].level * mazes[mazelvl].levelDistance,
                                                        mazes[mazelvl].scale * bottomOfStairs.z);
            Vector3 stairTopPosition = new Vector3(mazes[mazelvl].scale * topOfStairs.x,
                                                    mazes[mazelvl].scale * mazes[mazelvl].level * mazes[mazelvl].levelDistance,
                                                    mazes[mazelvl].scale * topOfStairs.z);

            Destroy(mazes[mazelvl].piecePlaces[bottomOfStairs.x, bottomOfStairs.z]._model);
            Destroy(mazes[mazelvl + 1].piecePlaces[topOfStairs.x, topOfStairs.z]._model);

            GameObject Stairs = Instantiate(Stairwell, stairBottomPosition, Quaternion.identity, mazes[mazelvl].transform);
            mazes[mazelvl].piecePlaces[bottomOfStairs.x, bottomOfStairs.z]._model = Stairs;
            mazes[mazelvl].piecePlaces[bottomOfStairs.x, bottomOfStairs.z]._piece = PieceType.Ladder;
            mazes[mazelvl + 1].piecePlaces[topOfStairs.x, topOfStairs.z]._model = null;
            mazes[mazelvl + 1].piecePlaces[topOfStairs.x, topOfStairs.z]._piece = PieceType.Ladder;



        }

        for (int mazeIndex = 0; mazeIndex < mazes.Length-1; mazeIndex++)
        {
            mazes[mazeIndex + 1].gameObject.transform.Translate(
                mazes[mazeIndex + 1].xOffset * mazes[mazeIndex + 1].scale,
                0,
                mazes[mazeIndex + 1].zOffset * mazes[mazeIndex + 1].scale);
        }
    }
    


}