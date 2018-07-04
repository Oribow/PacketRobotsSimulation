using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Actors : IRobotActors {
    private ManualResetEvent FinishedAction = new ManualResetEvent(false);
    private RobotMotor motor;
    public IRobotActorInfo aInfs;

    public Actors(RobotMotor motor)
    {
        this.motor = motor;
    }

    public void DriveForward()
    {
        FinishedAction.Reset();
        motor.DriveForward(()=> { FinishedAction.Set(); });
        FinishedAction.WaitOne();
    }

    public void TurnLeft()
    {
        FinishedAction.Reset();
        motor.TurnLeft(() => { FinishedAction.Set(); });
        FinishedAction.WaitOne();
    }

    public void TurnRight()
    {
        FinishedAction.Reset();
        motor.TurnRight(() => { FinishedAction.Set(); });
        FinishedAction.WaitOne();
    }

    public void StartUnload()
    {
        FinishedAction.Reset();
        motor.StartUnload(() => { FinishedAction.Set(); });
        FinishedAction.WaitOne();
        aInfs.Unloaded();
    }
}
