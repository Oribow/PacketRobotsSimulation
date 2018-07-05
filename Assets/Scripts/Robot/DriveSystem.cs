using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DriveSystem : IRobotActorInfo, ISensorInfo
{

    public enum State
    {
        Idle,
        DriveNorth,
        TurnToTarget,
        DriveWest,
        DriveEast,
        DriveToTarget,
        DriveBackEastWest,
        Unload,
    }

    private IRobotActors actors;
    public State state;
    public State updatedState;
    public TaskProcessing robo;

    private Position goal;
    private SensorData data;
    private bool updatedStateIsNew = false;

    public DriveSystem(IRobotActors actors, TaskProcessing robo)
    {
        this.actors = actors;
        this.robo = robo;
    }

    public void SensorEvent(SensorData data)
    {
        this.data = data;
        if (updatedStateIsNew)
        {
            updatedStateIsNew = false;
            state = updatedState;
        }

        switch (state)
        {
            case State.DriveToTarget:
                Position delta = data.Pos() - goal;
                Direction dd;
                if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1)
                {
                    dd = TargetDirection(data.Pos(), goal);
                    if (dd == Direction.AHEAD)
                    {
                        actors.DriveForward();
                    }
                    else if (dd == Direction.LEFT)
                    {
                        actors.TurnLeft();
                        actors.DriveForward();
                    }
                    else if (dd == Direction.RIGHT)
                    {
                        actors.TurnRight();
                        actors.DriveForward();
                    }
                    else
                    {
                        actors.TurnRight();
                        actors.TurnRight();
                        actors.DriveForward();
                    }
                    state = State.Idle;
                    robo.Arrived();
                    break;
                }

                Orientation o = TargetOrientation(data.Pos(), goal);
                dd = TargetDirection(data.Pos(), goal);
                if (o == Orientation.NORTH)
                {
                    state = State.DriveNorth;
                    if (dd != Direction.AHEAD)
                    {
                        if (dd == Direction.LEFT)
                        {
                            actors.TurnLeft();
                        }
                        else if (dd == Direction.RIGHT)
                        {
                            actors.TurnRight();
                        }
                        else
                        {
                            actors.TurnRight();
                            actors.TurnRight();
                        }
                    }
                }
                else
                {
                    if (dd == Direction.BEHIND)
                    {
                        actors.TurnLeft();
                        actors.DriveForward();
                        actors.TurnLeft();
                    }
                    state = State.DriveBackEastWest;
                }
                break;
            case State.TurnToTarget:
                Direction d = TargetDirection(data.Pos(), goal);
                if (d == Direction.LEFT)
                {
                    actors.DriveForward();
                    actors.TurnLeft();
                    actors.DriveForward();
                    state = State.DriveWest;
                }
                else
                {
                    actors.TurnRight();
                    state = State.DriveEast;
                }
                break;

            case State.DriveNorth:
                d = TargetDirection(data.Pos(), goal);
                if (d != Direction.AHEAD)
                {
                    state = State.TurnToTarget;
                }
                else
                {
                    actors.DriveForward();
                }
                break;

            case State.DriveWest:
                if (TargetReached(data.Pos() - Position.Forward, goal))
                {
                    actors.TurnLeft();
                    actors.DriveForward();
                    actors.TurnLeft();
                    state = State.Idle;
                    robo.Arrived();
                }
                else
                {
                    actors.DriveForward();
                }
                break;

            case State.DriveEast:
                if (TargetReached(data.Pos(), goal))
                {
                    state = State.Idle;
                    robo.Arrived();
                }
                else
                {
                    actors.DriveForward();
                }
                break;

            case State.DriveBackEastWest:
                d = TargetDirection(data.Pos(), goal);
                if (d != Direction.AHEAD)
                {
                    if (d == Direction.LEFT)
                        actors.TurnLeft();
                    else
                        actors.TurnRight();
                    state = State.DriveEast;
                }
                else
                {
                    actors.DriveForward();
                }
                break;

            case State.Unload:
                actors.StartUnload();
                break;
        }
    }

    public void DriveTo(Position p)
    {
        goal = p;
        updatedState = State.DriveToTarget;
        updatedStateIsNew = true;
    }

    public void Stop()
    {
        updatedState = State.Idle;
        updatedStateIsNew = true;
    }

    public void Unloaded()
    {
        updatedState = State.Idle;
        updatedStateIsNew = true;
        robo.Unloaded();
    }

    public void Unload()
    {
        updatedState = State.Unload;
        updatedStateIsNew = true;
    }

    private bool TargetReached(Position current, Position target)
    {
        return current == target;
    }

    private Direction TargetDirection(Position current, Position target)
    {
        Position delta = target - current;
        Orientation o;
        if (delta.y > 0)
            o = Orientation.NORTH;
        else if (delta.x > 0)
            o = Orientation.EAST;
        else if (delta.x < 0)
            o = Orientation.WEST;
        else
            o = Orientation.SOUTH;
        int i = (int)o + 4;
        int k = (int)data.PosOrientation();
        return (Direction)((i - k) % 4);
    }

    private Orientation TargetOrientation(Position current, Position target)
    {
        Position delta = target - current;
        Orientation o;
        if (delta.y > 0)
            o = Orientation.NORTH;
        else if (delta.x > 0)
            o = Orientation.EAST;
        else if (delta.x < 0)
            o = Orientation.WEST;
        else
            o = Orientation.SOUTH;
        return o;
    }
}
