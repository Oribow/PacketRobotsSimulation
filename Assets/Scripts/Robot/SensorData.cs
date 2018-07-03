using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorData
{
    private Grid grid;
    private RobotMotor motor;

    public SensorData(RobotMotor motor)
    {
        this.motor = motor;
        this.grid = GridLoader.grid;
    }

    public int PosX()
    {
        return motor.PosX;
    }

    public int PosY()
    {
        return motor.PosY;
    }

    public Grid.TileType PosType()
    {
        return grid.GetTileType(motor.PosX, motor.PosY);
    }

    public int PosOrientation()
    {
        return motor.Orientation;
    }

    public bool BlockedFront()
    {
        int x, y;
        RelMove(1, 0, out x, out y);
        return !grid.IsTileFree(x, y);
    }

    public bool BlockedLeft()
    {
        int x, y;
        RelMove(0, -1, out x, out y);
        return !grid.IsTileFree(x, y);
    }

    public bool BlockedRight()
    {
        int x, y;
        RelMove(0, 1, out x, out y);
        return !grid.IsTileFree(x, y);
    }

    public bool BlockedWaypointAhead()
    {
        int x, y;
        if (PosType() == Grid.TileType.CrossRoad)
        {
            RelMove(2, 0, out x, out y);
            if (!grid.IsTileFree(x, y))
                return true;
        }
        else
        {
            RelMove(3, -1, out x, out y);
            if (!grid.IsTileFree(x, y))
                return true;
        }
        return false;
    }

    public bool BlockedWaypointLeft()
    {
        int x, y;
        RelMove(1, -2, out x, out y);
        if (!grid.IsTileFree(x, y))
            return true;
        return false;
    }

    public bool BlockedWaypointRight()
    {
        int x, y;
        if (PosType() == Grid.TileType.CrossRoad)
        {
            RelMove(0, 1, out x, out y);
            if (!grid.IsTileFree(x, y))
                return true;
        }
        else
        {
            RelMove(2, 1, out x, out y);
            if (!grid.IsTileFree(x, y))
                return true;
        }
        return false;
    }

    public bool BlockedCrossroadAhead()
    {
        int x, y;
        if (PosType() == Grid.TileType.CrossRoad)
        {
            // check rest of crossroad
            for (int f = -1; f <= 1; f++)
            {
                for (int r = -1; r <= 1; r++)
                {
                    if (f == r && f == 0)
                        continue;

                    RelMove(f, r, out x, out y);
                    if (grid.GetTileType(x, y) != Grid.TileType.CrossRoad)
                        continue;

                    if (!grid.IsTileFree(x, y))
                        return true;
                }
            }
        }
        else
        {
            // check crossroad ahead
            RelMove(1, 0, out x, out y);
            if (grid.GetTileType(x, y) != Grid.TileType.CrossRoad)
            {
                Debug.LogWarning("BlockedCrossroadAhead: Not in front of cross!");
                return true;
            }

            if (!grid.IsTileFree(x, y))
                return true;

            RelMove(2, 0, out x, out y);
            if (!grid.IsTileFree(x, y))
                return true;

            RelMove(1, -1, out x, out y);
            if (!grid.IsTileFree(x, y))
                return true;

            RelMove(2, -1, out x, out y);
            if (!grid.IsTileFree(x, y))
                return true;

        }
        return false;
    }

    public bool BlockedCrossroadRight()
    {
        int x, y;

        RelMove(0, 1, out x, out y);
        if (!grid.IsTileFree(x, y))
            return true;

        RelMove(0, 2, out x, out y);
        if (!grid.IsTileFree(x, y))
            return true;

        RelMove(1, 1, out x, out y);
        if (!grid.IsTileFree(x, y))
            return true;

        RelMove(1, 2, out x, out y);
        if (!grid.IsTileFree(x, y))
            return true;

        return false;
    }

    private void RelMove(int stepsForward, int stepsRight, out int x, out int y)
    {
        x = RobotMotor.dirOffsetsX[motor.Orientation] * stepsForward + motor.PosX;
        x += RobotMotor.dirOffsetsX[(motor.Orientation + 1) % 4] * stepsRight;
        y = RobotMotor.dirOffsetsY[(motor.Orientation + 1) % 4] * stepsRight + motor.PosY;
        y += RobotMotor.dirOffsetsY[motor.Orientation] * stepsForward;
    }
}
