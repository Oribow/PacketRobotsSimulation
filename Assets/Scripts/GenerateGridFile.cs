using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenerateGridFile : MonoBehaviour
{
    public const string GridFilePath = "gridFile.grd";

    public static void WriteGridFile(int gridWidth, int gridHeight)
    {
        string url = Path.Combine(Application.dataPath, GridFilePath);
        using (StreamWriter writer = new StreamWriter(File.OpenWrite(url)))
        {
            writer.WriteLine(gridWidth);
            writer.WriteLine(gridHeight);

            char[] freeLane = new char[gridWidth];
            for (int i = 1; i <= gridWidth; i++)
            {
                if (i % 3 == 0)
                    freeLane[i - 1] = Grid.WaypointTile;
                else
                    freeLane[i - 1] = Grid.CrossRoadTile;
            }

            char[] blockedLane = new char[gridWidth];
            for (int i = 1; i <= gridWidth; i++)
            {
                if (i % 3 == 0)
                    blockedLane[i - 1] = Grid.DeliveryTile;
                else
                    blockedLane[i - 1] = Grid.WaypointTile;
            }

            for (int i = 1; i <= gridHeight; i++)
            {
                if (i % 3 == 0)
                {
                    writer.WriteLine(blockedLane);
                }
                else
                {
                    writer.WriteLine(freeLane);
                }
            }
        }


    }

    public static Grid ParseGridFile()
    {
        string url = Path.Combine(Application.dataPath, GridFilePath);
        using (StreamReader reader = File.OpenText(url))
        {
            int gridWidth = int.Parse(reader.ReadLine());
            int gridHeight = int.Parse(reader.ReadLine());
            char[,] grid = new char[gridHeight, gridWidth];
            for (int i = 0; i < gridHeight; i++)
            {
                string line = reader.ReadLine();
                for (int k = 0; k < gridWidth; k++)
                {
                    grid[i, k] = line[k];
                }
            }
            return new Grid(grid);
        }
    }
}
