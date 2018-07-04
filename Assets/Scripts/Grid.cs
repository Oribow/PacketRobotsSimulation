using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public const char WaypointTile = '+';
    public const char CrossRoadTile = 'x';
    public const char DeliveryTile = 'O';

    public int Width { get; private set; }
    public int Height { get; private set; }
    char[,] grid;
    bool[,] isTileUsed;

    public Grid(char[,] grid)
    {
        this.grid = grid;
        Width = grid.GetLength(1);
        Height = grid.GetLength(0);
        isTileUsed = new bool[Height, Width];
    }

    public bool IsTileFree(Position p)
    {
        if (p.x < 0 || p.y < 0 || p.x >= Width || p.y >= Height)
            return false;

        return grid[p.y, p.x] != DeliveryTile && isTileUsed[p.y, p.x] == false;
    }

    public void RemoveRobot(Position p)
    {
        if (!isTileUsed[p.y, p.x])
        {
            Debug.Log("Already removed");
        }
        isTileUsed[p.y, p.x] = false;
    }

    public void PlaceRobot(Position p)
    {
        if (isTileUsed[p.y, p.x])
        {
           // Debug.Log("Collision");
        }
        isTileUsed[p.y, p.x] = true;
    }

    public PositionType GetTileType(Position p)
    {
        if (p.x < 0 ||p.y < 0 || p.x >= Width || p.y >= Height)
            return PositionType.BLOCKED;

        switch (grid[p.y, p.x])
        {
            case CrossRoadTile:
                return PositionType.CROSSROADS;
            case WaypointTile:
                return PositionType.WAYPOINT;
            case DeliveryTile:
                return PositionType.STATION;
        }
        throw new System.Exception(string.Format("Unkown tile type: {0}", grid[p.y, p.x]));
    }
}
