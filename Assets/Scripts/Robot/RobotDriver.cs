using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDriver {

    private static int[] CrossRoadForward = new int[] { 1, 0, 1, 0};
    private static int[] CrossRoadLeft = new int[] { 1, 0, 0, -1, 1, 0 };
    private static int[] CrossRoadRight = new int[] { 0, 1 };
    private static int[] WayPointFlip = new int[] { 0, -1, 0, -1 };

    public enum BrainState {
        Idle,
        FollowPath,
        MoveToTarget,
    }

    private RobotMotor motor;
    private SensorData sensorData;

    public BrainState brainState;
    public int targetX;
    public int targetY;

    private int[] currentPath;
    private int pathOffset;
    private int subTargetX, subTargetY;
    private int prevDir;

    public RobotDriver(RobotMotor motor)
    {
        this.motor = motor;
        sensorData = new SensorData(motor);
    }

	public void Think () {
        ThinkStart:
        switch (brainState)
        {
            case BrainState.Idle:
                break;
            case BrainState.FollowPath:
                if (TargetReached())
                {
                    if (RotateTo(1))
                    {
                        brainState = BrainState.Idle;
                    }
                }
                if (FollowPath())
                {
                    brainState = BrainState.MoveToTarget;
                    goto ThinkStart;
                }
                break;
            case BrainState.MoveToTarget:
                if (TargetReached())
                {
                    if (RotateTo(1))
                    {
                        brainState = BrainState.Idle;
                    }
                } else if (sensorData.PosType() == Grid.TileType.CrossRoad)
                {
                    // select best dir
                    // <- f ->
                    int targetDir = RelTargetDir(TargetDirection());
                    if (targetDir == 0)
                    {
                        currentPath = CrossRoadForward;
                        Debug.Log("Cross Forward");
                    }
                    else if (targetDir == 1)
                    {
                        currentPath = CrossRoadRight;
                        Debug.Log("Cross Right");
                    }
                    else if (targetDir == 3)
                    {
                        currentPath = CrossRoadLeft;
                        Debug.Log("Cross Left");
                    }
                    else
                    {
                        // behind us
                        GotoTarget(targetX, targetY);
                        return;
                    }
                    pathOffset = 0;
                    NextPathNode();
                    brainState = BrainState.FollowPath;

                    goto ThinkStart;
                }
                else {
                    /*if (RelTargetDir(TargetDirection()) == 2)
                    {
                        currentPath = WayPointFlip;
                        pathOffset = 0;
                        NextPathNode();
                        brainState = BrainState.FollowPath;
                        goto ThinkStart;
                    }*/
                    GotoTarget(targetX, targetY, true);
                }
                break;
        }
    }

    public void DriveTo(int x, int y)
    {
        brainState = BrainState.MoveToTarget;
        targetX = x;
        targetY = y;
    }

    public void Stop()
    {
        brainState = BrainState.Idle;
        Debug.Log("Stopping...");
    }

    private bool TargetReached()
    {
        return ReachedTarget(targetX, targetY);
    }

    private int TargetDirection()
    {
        return DirToTarget(targetX, targetY);
    }

    private int DirToTarget(int x, int y)
    {
        int dirX = x - sensorData.PosX();
        int dirY = y - sensorData.PosY();
        if (dirY > 0)
        {
            return 0;
        }
        if (dirX != 0)
        {
            return dirX > 0 ? 1 : 3;
        }
        return 2;
    }


    private void NextPathNode()
    {
        int f = currentPath[pathOffset++];
        int r = currentPath[pathOffset++];

        RelMove(f, r, out subTargetX, out subTargetY);
    }

    private bool FollowPath()
    {
        if (ReachedTarget(subTargetX, subTargetY))
        {
            if (pathOffset == currentPath.Length)
            {
                return true;
            }
            NextPathNode();
        }
        GotoTarget(subTargetX, subTargetY, true);
        return false;
    }

    private void GotoTarget(int x, int y, bool allowRot = false)
    {
        if (allowRot)
        {
            int targetDir = DirToTarget(x, y);
            if (RotateTo(targetDir))
            {
                motor.DriveForward();
            }
        }
        else {
            motor.DriveForward();
        }
    }

    private bool RotateTo(int dir)
    {
        if (dir != sensorData.PosOrientation())
        {
            // turn to
            if ((sensorData.PosOrientation() + 1 % 4) == dir)
                motor.TurnRight();
            else
                motor.TurnLeft();

            return false;
        }
        return true;
    }

    private void RelMove(int stepsForward, int stepsRight, out int x, out int y)
    {
        x = RobotMotor.dirOffsetsX[sensorData.PosOrientation()] * stepsForward + sensorData.PosX();
        x += RobotMotor.dirOffsetsX[(sensorData.PosOrientation() + 1) % 4] * stepsRight;
        y = RobotMotor.dirOffsetsY[(sensorData.PosOrientation() + 1) % 4] * stepsRight + sensorData.PosY();
        y += RobotMotor.dirOffsetsY[sensorData.PosOrientation()] * stepsForward;
    }

    private int RelTargetDir(int dir)
    {
        return (4 + dir - sensorData.PosOrientation()) % 4;
    }

    private bool ReachedTarget(int x, int y)
    {
        return x == sensorData.PosX() && y == sensorData.PosY();
    }
}
