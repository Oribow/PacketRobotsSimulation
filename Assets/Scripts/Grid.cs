using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public const char WaypointTile = '+';
    public const char CrossRoadTile = 'x';
    public const char DeliveryTile = 'O';

    public enum TileType {
        CrossRoad,
        Waypoint,
        DeliveryStation,
        OutOfBounds
    }

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

    public bool IsTileFree(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return false;

        return grid[y, x] != DeliveryTile && isTileUsed[y, x] == false;
    }

    public void RemoveRobot(int x, int y)
    {
        if (!isTileUsed[y, x])
        {
            Debug.Log("Already removed");
        }
        isTileUsed[y, x] = false;
    }

    public void PlaceRobot(int x, int y)
    {
        if (isTileUsed[y, x])
        {
           // Debug.Log("Collision");
        }
        isTileUsed[y, x] = true;
    }

    public TileType GetTileType(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return TileType.OutOfBounds;

        switch (grid[y, x])
        {
            case CrossRoadTile:
                return TileType.CrossRoad;
            case WaypointTile:
                return TileType.Waypoint;
            case DeliveryTile:
                return TileType.DeliveryStation;
        }
        throw new System.Exception(string.Format("Unkown tile type: {0}", grid[y, x]));
    }
}
