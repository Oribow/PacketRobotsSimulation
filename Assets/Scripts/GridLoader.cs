using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLoader : MonoBehaviour {
    public static Grid grid;

    public bool writeNewFile = false;
    public int gridWidth = 100;
    public int gridHeight = 100;

    private void Awake()
    {
        if (writeNewFile)
            GenerateGridFile.WriteGridFile(gridWidth, gridHeight);

        grid = GenerateGridFile.ParseGridFile();
    }
}
