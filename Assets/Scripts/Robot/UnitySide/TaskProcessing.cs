using UnityEngine;
using System.Collections;
using System.Threading;

[RequireComponent(typeof(RobotMotor))]
public class TaskProcessing : MonoBehaviour
{
    public RobotMotor motor;
    public Transform manualGoal;
    public bool useManualGoal;

    Sensor sensor;
    Actors actors;
    DriveSystem driveSystem;

    public SensorData data;
    private volatile bool runSensors = true;
    private int state = 3;
    private System.Random random;
    private Position currentTarget;
    private Position manualTarget;
    private volatile bool requireNewSensorData;

    private void Start()
    {
        Statistics.numRoboter++;
        actors = new Actors(motor);
        sensor = new Sensor(motor);
        driveSystem = new DriveSystem(actors, this);
        actors.aInfs = driveSystem;
        data = sensor.Sense();
        random = new System.Random(motor.Pos.x * motor.Pos.y);
        if (manualGoal != null)
            manualTarget = new Position(Mathf.RoundToInt(manualGoal.position.x), Mathf.RoundToInt(manualGoal.position.z));

        Thread t = new Thread(SensorEventSender);
        t.Start();
        Arrived();
    }

    void SensorEventSender()
    {
        while (runSensors)
        {
            driveSystem.SensorEvent(data);
            requireNewSensorData = true;
            do
            {
                Thread.Sleep(40);
            } while (requireNewSensorData);
        }
    }

    public void Arrived()
    {
        if (state == 0)
        {
            driveSystem.Unload();
        }
        else if (state == 1)
        {
            driveSystem.DriveTo(motor.StartPos);
            state = 2;
        }
        else
        {
            motor.LoadParcel();
            state = 0;
            Position p;
            if (useManualGoal)
            {
                p = manualTarget;
            }
            else
            {
                do
                {
                    p.x = random.Next(0, GridLoader.grid.Width / 3 - 3);
                    p.y = random.Next(0, GridLoader.grid.Height / 3 - 3);
                    p.x = 5 + p.x * 3;
                    p.y = 6 + p.y * 3;
                } while (!GridLoader.grid.IsTileFree(p));
            }
            currentTarget = p;
            driveSystem.DriveTo(p);
        }
    }

    public void Unloaded()
    {
        driveSystem.DriveTo(motor.StartPos - new Position(1, 0));
        state = 1;
        Statistics.PackageDeliverd();
    }

    public DriveSystem.State prevState;
    private void Update()
    {
        if (requireNewSensorData)
        {
            requireNewSensorData = false;
            data = sensor.Sense();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (prevState != driveSystem.state)
        {
            Debug.Log(driveSystem.state.ToString());
            prevState = driveSystem.state;
        }
        Gizmos.DrawLine(transform.position, new Vector3(currentTarget.x, 0, currentTarget.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(motor.StartPos.x, 0, motor.StartPos.y));
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
