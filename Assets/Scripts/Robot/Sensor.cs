using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor
{
    private Grid grid;
    private RobotMotor motor;

    public Sensor(RobotMotor motor)
    {
        this.motor = motor;
        this.grid = GridLoader.grid;
    }

    public SensorData Sense()
    {
        return new SensorData(
            Pos(),
            PosType(),
            PosOrientation(),
            BlockedFront(),
            BlockedLeft(),
            BlockedRight(),
            BlockedWaypointAhead(),
            BlockedWaypointLeft(),
            BlockedWaypointRight(),
            BlockedCrossroadAhead(),
            BlockedCrossroadRight()
            );
    }

    public Position Pos()
    {
        return motor.Pos;
    }

    public PositionType PosType()
    {
        return grid.GetTileType(motor.Pos);
    }

    public Orientation PosOrientation()
    {
        return motor.Orientation;
    }

    public bool BlockedFront()
    {
        return !grid.IsTileFree(motor.Pos + Position.Forward * motor.Orientation);
    }

    public bool BlockedLeft()
    {
        return !grid.IsTileFree(motor.Pos + Position.Left * motor.Orientation);
    }

    public bool BlockedRight()
    {
        return !grid.IsTileFree(motor.Pos + Position.Right * motor.Orientation);
    }

    public bool BlockedWaypointAhead()
    {
        if (PosType() == PositionType.CROSSROADS)
        {
            return !grid.IsTileFree(motor.Pos + Position.Forward * motor.Orientation * 2);
        }
        else
        {
            return !grid.IsTileFree(motor.Pos + new Position(3, -1) * motor.Orientation);
        }
    }

    public bool BlockedWaypointLeft()
    {
        return !grid.IsTileFree(motor.Pos + new Position(1, -2) * motor.Orientation);
    }

    public bool BlockedWaypointRight()
    {
        if (PosType() == PositionType.CROSSROADS)
        {
            return BlockedRight();
        }
        else
        {
            return !grid.IsTileFree(motor.Pos + new Position(2, 1) * motor.Orientation);
        }
    }

    public bool BlockedCrossroadAhead()
    {
        if (PosType() == PositionType.CROSSROADS)
        {
            // check rest of crossroad
            for (int f = -1; f <= 1; f++)
            {
                for (int r = -1; r <= 1; r++)
                {
                    if (f == r && f == 0)
                        continue;

                    Position p = motor.Pos + new Position(f, r) * motor.Orientation;
                    if (grid.GetTileType(p) != PositionType.CROSSROADS)
                        continue;

                    if (!grid.IsTileFree(p))
                        return true;
                }
            }
        }
        else
        {
            // check crossroad ahead
            Position p = motor.Pos + Position.Forward * motor.Orientation;
            if (grid.GetTileType(p) != PositionType.CROSSROADS)
            {
                return true;
            }

            if (!grid.IsTileFree(p))
                return true;

            if (!grid.IsTileFree(motor.Pos + new Position(2, 0) * motor.Orientation))
                return true;

            if (!grid.IsTileFree(motor.Pos + new Position(1, -1) * motor.Orientation))
                return true;

            if (!grid.IsTileFree(motor.Pos + new Position(2, -1) * motor.Orientation))
                return true;

        }
        return false;
    }

    public bool BlockedCrossroadRight()
    {
        if (BlockedRight())
            return true;

        if (!grid.IsTileFree(motor.Pos + new Position(0, 2) * motor.Orientation))
            return true;

        if (!grid.IsTileFree(motor.Pos + new Position(1, 1) * motor.Orientation))
            return true;

        if (!grid.IsTileFree(motor.Pos + new Position(1, 2) * motor.Orientation))
            return true;

        return false;
    }
}
