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
        DriveWest,
        DriveForwardTillGoal,
        DriveToTarget,
        DriveBackEast,
        ReachedGoal,
        Unload,

        //util methods
        WaitTillCrossRoadFree,
        SafeDriveForward,
        SafeUTurn,
        TurnToTarget,
        SafeUTurn2,
        SafeCrossRoadForward,
        SafeCrossRoadForward2,
        SafeCrossRoadRight,
        SafeCrossRoadRight2,
        SafeCrossRoadLeft,
        SafeCrossRoadLeft2,
        DriveBackWest,
        DriveWest2,
    }

    private IRobotActors actors;
    public State state;
    public State updatedState;
    public TaskProcessing robo;

    private Position goal;
    private SensorData data;
    private bool updatedStateIsNew = false;
    private State nextState;
    private State nextState2;

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
                        nextState = State.ReachedGoal;
                        state = State.SafeDriveForward;
                    }
                    else if (dd == Direction.LEFT)
                    {
                        actors.TurnLeft();
                        nextState = State.ReachedGoal;
                        state = State.SafeDriveForward;
                    }
                    else if (dd == Direction.RIGHT)
                    {
                        actors.TurnRight();
                        nextState = State.ReachedGoal;
                        state = State.SafeDriveForward;
                    }
                    else
                    {
                        actors.TurnRight();
                        actors.TurnRight();
                        nextState = State.ReachedGoal;
                        state = State.SafeDriveForward;
                    }
                    break;
                }

                Orientation o = TargetOrientation(data.Pos(), goal);
                dd = TargetDirection(data.Pos(), goal);
                if (o == Orientation.NORTH)
                {
                    TurnToTarget();
                    state = State.DriveNorth;
                }
                else
                {
                    if (dd == Direction.BEHIND)
                    {
                        state = State.SafeUTurn;
                        nextState = State.DriveBackWest;
                    }
                    else
                    {
                        state = State.DriveBackEast;
                    }
                }
                break;

            case State.DriveNorth:
                Direction d = TargetDirection(data.Pos() + Position.Forward, goal);
                Direction d2 = TargetDirection(data.Pos() + Position.Forward * 4, goal);
                if (d == Direction.RIGHT)
                {
                    state = State.SafeCrossRoadRight;
                    nextState = State.DriveForwardTillGoal;
                }
                else if (d2 == Direction.LEFT)
                {
                    state = State.SafeCrossRoadLeft;
                    nextState = State.DriveWest;
                }
                else
                {
                    state = State.SafeCrossRoadForward;
                    nextState = State.DriveNorth;
                }
                break;

            case State.DriveWest:
                if (data.Pos().x == goal.x)
                {
                    state = State.SafeCrossRoadRight;
                    nextState = State.DriveWest2;
                }
                else
                {
                    state = State.SafeCrossRoadForward;
                    nextState = State.DriveWest;
                }
                break;

            case State.DriveWest2:
                state = State.SafeCrossRoadRight;
                nextState = State.ReachedGoal;
                break;

            case State.DriveForwardTillGoal:
                if (TargetReached(data.Pos(), goal))
                {
                    state = State.ReachedGoal;
                }
                else
                {
                    state = State.SafeCrossRoadForward;
                    nextState = State.DriveForwardTillGoal;
                }
                break;

            case State.DriveBackEast:
                d = TargetDirection(data.Pos() + Position.Right, goal);
                if (d != Direction.AHEAD)
                {
                    state = State.SafeCrossRoadRight;
                    nextState = State.DriveForwardTillGoal;
                }
                else
                {
                    state = State.SafeCrossRoadForward;
                    nextState = State.DriveBackEast;
                }
                break;

            case State.DriveBackWest:
                d = TargetDirection(data.Pos() + Position.Left * 2, goal);
                if (d != Direction.AHEAD)
                {
                    state = State.SafeCrossRoadLeft;
                    nextState = State.DriveForwardTillGoal;
                }
                else
                {
                    state = State.SafeCrossRoadForward;
                    nextState = State.DriveBackWest;
                }
                break;


            // Helper states

            case State.SafeDriveForward:
                if (data.BlockedFront())
                    break;
                actors.DriveForward();
                state = nextState;
                break;

            case State.SafeUTurn:
                actors.TurnLeft();
                state = State.SafeDriveForward;
                nextState2 = nextState;
                nextState = State.SafeUTurn2;
                break;

            case State.SafeUTurn2:
                actors.TurnLeft();
                state = nextState2;
                break;

            case State.SafeCrossRoadForward:
                nextState2 = nextState;
                state = State.WaitTillCrossRoadFree;
                nextState = State.SafeCrossRoadForward2;
                break;

            case State.SafeCrossRoadForward2:
                actors.DriveForward();
                actors.DriveForward();
                state = State.SafeDriveForward;
                nextState = nextState2;
                break;

            case State.SafeCrossRoadRight:
                state = State.WaitTillCrossRoadFree;
                nextState2 = nextState;
                nextState = State.SafeCrossRoadRight2;
                break;

            case State.SafeCrossRoadRight2:
                actors.DriveForward();
                actors.TurnRight();
                state = State.SafeDriveForward;
                nextState = nextState2;
                break;

            case State.SafeCrossRoadLeft:
                state = State.WaitTillCrossRoadFree;
                nextState2 = nextState;
                nextState = State.SafeCrossRoadLeft2;
                break;

            case State.SafeCrossRoadLeft2:
                actors.DriveForward();
                actors.DriveForward();
                actors.TurnLeft();
                actors.DriveForward();
                state = State.SafeDriveForward;
                nextState = nextState2;
                break;

            case State.WaitTillCrossRoadFree:
                if (data.BlockedCrossroadAhead())
                    break;

                if (data.BlockedWaypointRight())
                {
                    if (data.BlockedWaypointAhead())
                    {
                        if (data.BlockedWaypointLeft())
                        {
                            if (data.PosOrientation() == Orientation.SOUTH)
                            {
                                state = nextState;
                            }
                        }
                    }
                    break;
                }
                state = nextState;
                break;

            case State.ReachedGoal:
                state = State.Idle;
                robo.Arrived();
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

    private void TurnToTarget()
    {
        Direction d = TargetDirection(data.Pos(), goal);
        if (d != Direction.AHEAD)
        {
            if (d == Direction.LEFT)
            {
                actors.TurnLeft();
            }
            else if (d == Direction.RIGHT)
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
