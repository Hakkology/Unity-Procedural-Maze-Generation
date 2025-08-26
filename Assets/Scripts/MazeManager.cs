using System.Linq;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [Header("Maze Detail References")]
    public int width = 30;
    public int depth = 30;

    [Header("Maze piece References")]
    public CorridorMaze[] mazes;

    [Header("Straight piece References")]
    public GameObject StraightManHoleLadder;
    public GameObject StraightManHoleUp;
    public GameObject DeadendManHoleLadder;
    public GameObject DeadendManHoleUp;


    void Start()
    {
        int level = 0;
        foreach (CorridorMaze maze in mazes)
        {
            maze.width = width;
            maze.depth = depth;
            maze.level = level++;
            maze.Build();
        }

        for (int mazelvl = 0; mazelvl < mazes.Length-1; mazelvl++)
            for (int z = 0; z < depth; z++)
                for (int x = 0; x < width; x++)
                {

                    if (mazes[mazelvl].piecePlaces[x, z]._piece == mazes[mazelvl + 1].piecePlaces[x, z]._piece)
                    {
                        if (mazes[mazelvl].piecePlaces[x, z]._piece == PieceType.Vertical_Straight)
                        {
                            Destroy(mazes[mazelvl].piecePlaces[x, z]._model);
                            Destroy(mazes[mazelvl + 1].piecePlaces[x, z]._model);

                            Vector3 downPosition = new Vector3(mazes[mazelvl].scale * x, mazes[mazelvl].scale * mazes[mazelvl].level * 2, mazes[mazelvl].scale * z);
                            Vector3 upPosition = new Vector3(mazes[mazelvl+1].scale * x, mazes[mazelvl+1].scale * mazes[mazelvl+1].level * 2, mazes[mazelvl+1].scale * z);

                            var upModel = Instantiate(StraightManHoleLadder, upPosition, Quaternion.identity);
                            var ladderModel = Instantiate(StraightManHoleUp, downPosition, Quaternion.identity);

                            mazes[mazelvl].piecePlaces[x, z]._model = upModel;
                            mazes[mazelvl].piecePlaces[x, z]._piece = PieceType.Ladder;
                            mazes[mazelvl+1].piecePlaces[x, z]._model = ladderModel;
                            mazes[mazelvl+1].piecePlaces[x, z]._piece = PieceType.Ladder;
                        }
                        else if (mazes[mazelvl].piecePlaces[x, z]._piece == PieceType.DeadEnd)
                        {
                            Destroy(mazes[mazelvl].piecePlaces[x, z]._model);
                            Destroy(mazes[mazelvl+1].piecePlaces[x, z]._model);

                            Vector3 downPosition = new Vector3(mazes[mazelvl].scale * x, mazes[mazelvl].scale * mazes[mazelvl].level * 2, mazes[mazelvl].scale * z);
                            Vector3 upPosition = new Vector3(mazes[mazelvl+1].scale * x, mazes[mazelvl+1].scale * mazes[mazelvl+1].level * 2, mazes[mazelvl+1].scale * z);

                            var upModel = Instantiate(DeadendManHoleLadder, upPosition, Quaternion.identity);
                            var ladderModel = Instantiate(DeadendManHoleUp, downPosition, Quaternion.identity);

                            mazes[mazelvl].piecePlaces[x, z]._model = upModel;
                            mazes[mazelvl].piecePlaces[x, z]._piece = PieceType.Ladder;
                            mazes[mazelvl+1].piecePlaces[x, z]._model = ladderModel;
                            mazes[mazelvl+1].piecePlaces[x, z]._piece = PieceType.Ladder;
                        }
                    }
                }
        
    }
    
    void Update()
    {
        
    }
}