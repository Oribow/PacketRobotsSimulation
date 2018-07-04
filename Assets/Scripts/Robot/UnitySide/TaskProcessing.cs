using UnityEngine;
using System.Collections;
using System.Threading;

[RequireComponent(typeof(RobotMotor))]
public class TaskProcessing : MonoBehaviour
{
    public RobotMotor motor;

    Sensor sensor;
    Actors actors;
    DriveSystem driveSystem;

    private SensorData data;
    private volatile bool runSensors = true;
    private bool nextMoveHome = false;
    private System.Random random;

    private void Start()
    {
        actors = new Actors(motor);
        sensor = new Sensor(motor);
        driveSystem = new DriveSystem(actors, this);
        actors.aInfs = driveSystem;
        data = sensor.Sense();
        random = new System.Random();

        Thread t = new Thread(SensorEventSender);
        t.Start();
        Arrived();
    }

    void SensorEventSender()
    {
        while (runSensors)
        {
            driveSystem.SensorEvent(data);
            Thread.Sleep(100);
        }
    }

    public void Arrived()
    {
        if (nextMoveHome)
        {
            driveSystem.Unload();
        }
        else
        {
            motor.LoadParcel();
            nextMoveHome = true;
            Position p;
            do
            {
                p.x = random.Next(0, GridLoader.grid.Width / 3 - 3);
                p.y = random.Next(0, GridLoader.grid.Height / 3);
                p.x = 5 + p.x * 3;
                p.y = 3 + p.y * 3;
            } while (!GridLoader.grid.IsTileFree(p));
            driveSystem.DriveTo(p);
        }
    }

    public void Unloaded()
    {
        driveSystem.DriveTo(motor.StartPos);
        nextMoveHome = false;
    }

    private void Update()
    {
        data = sensor.Sense();
    }

    private void OnApplicationQuit()
    {
        runSensors = false;
    }
    private void OnDestroy()
    {
        runSensors = false;
    }
}
