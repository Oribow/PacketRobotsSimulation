using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DriveSystem : IRobotActorInfo, ISensorInfo
{

    public enum State
    {
        Idle,
        DriveToTarget,
        Unload,
    }

    private IRobotActors actors;
    public State state;
    public State updatedState;
    public TaskProcessing robo;

    private Position goal;
    private SensorData data;

    public DriveSystem(IRobotActors actors, TaskProcessing robo)
    {
        this.actors = actors;
        this.robo = robo;
    }

    public void SensorEvent(SensorData data)
    {
        this.data = data;
        state = updatedState;

        switch (state)
        {
            case State.DriveToTarget:
                if (TargetReached(data.Pos(), goal))
                {
                    robo.Arrived();
                    state = State.Idle;
                }
                else
                {
                    Direction d = TargetDirection(data.Pos(), goal);
                    if (d == Direction.LEFT)
                    {
                        actors.TurnLeft();
                    }
                    else if (d == Direction.RIGHT || d == Direction.BEHIND)
                    {
                        actors.TurnRight();
                    }
                    else
                    {
                        actors.DriveForward();
                    }
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
    }

    public void Stop()
    {
        updatedState = State.Idle;
    }

    public void Unloaded()
    {
        updatedState = State.Idle;
        robo.Unloaded();
    }

    public void Unload()
    {
        updatedState = State.Unload;
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
}
