using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public GameObject cube;
    public int width = 30;
    public int depth = 30;

    void Start()
    {
        GenerateCubes(40, 25);
    }

    private void GenerateCubes(int width, int depth)
    {
        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < depth; x++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                Instantiate(cube, pos, Quaternion.identity);
            }
        }
    }


}
