using System.Collections.Generic;
using UnityEngine;

public class DungeonPlaceObject : MonoBehaviour
{
    public GameObject prefab;
    public int chanceOfAppearence;
    public List<PieceType> possibleLocations = new List<PieceType>();

    DungeonMaze attachedMaze;

    void Awake()
    {
        attachedMaze = GetComponent<DungeonMaze>();
    }

    public void SpawnObject()
    {
        if (attachedMaze == null || possibleLocations == null || possibleLocations.Count == 0) return;
        
        for (int z = 0; z < attachedMaze.depth; z++)
            for (int x = 0; x < attachedMaze.width; x++)
            {
                var cell = attachedMaze.piecePlaces[x, z];

                if (cell._model == null) continue;
                if (!possibleLocations.Contains(cell._piece)) continue;
                if (Random.Range(0, 100) >= chanceOfAppearence) continue;

                Instantiate(prefab, cell._model.transform);
            }
        
    }
}