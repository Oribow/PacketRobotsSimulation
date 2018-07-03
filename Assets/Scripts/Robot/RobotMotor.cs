using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMotor : MonoBehaviour {

    public static int[] dirOffsetsX = new int[] { 0, 1, 0, -1};
    public static int[] dirOffsetsY = new int[] { 1, 0, -1, 0 };
    public static int[] dirRotations = new int[] { 0, 90, -180, 270 };

    public int PosX { get; private set; }
    public int PosY { get; private set; }
    public int Orientation { get; private set; }
    public bool HasParcel { get; private set; }
    public bool UnloadsParcel { get; private set; }

    public float speed;
    public float turnSpeed;

    private Animation animation;

    private bool shouldMove = false;
    private bool shouldTurn = false;
    private int targetX = 0;
    private int targetY = 0;
    private int targetDir = 0;
    private float timePast;

    public RobotDriver robotDriver;
    private Grid grid;

    private void Awake()
    {
        animation = GetComponent<Animation>();
    }

    private void Start()
    {
        PosX = Mathf.RoundToInt(transform.position.x);
        PosY = Mathf.RoundToInt(transform.position.z);
        robotDriver = new RobotDriver(this);
        grid = GridLoader.grid;
        grid.PlaceRobot(PosX, PosY);
    }

    //   0 
    // 3   1
    //   2 
    public void DriveForward()
    {
        targetX = dirOffsetsX[Orientation] + PosX;
        targetY = dirOffsetsY[Orientation] + PosY;
        shouldMove = true;
        timePast = 0;
        grid.PlaceRobot(targetX, targetY);
    }

    public void TurnLeft()
    {
        targetDir = (targetDir + 3) % 4;
        shouldTurn = true;
        timePast = 0;
    }

    public void TurnRight()
    {
        targetDir = (targetDir + 1) % 4;
        shouldTurn = true;
        timePast = 0;
    }

    public void UnloadLoadParcel()
    {
        UnloadsParcel = true;
        animation.Play("ParcelUnload");
        HasParcel = false;
    }

    public void LoadParcel()
    {
        animation.Play("ParcelLoad");
        HasParcel = true;
    }

    public void ParcelUnloadingFinished()
    {
        UnloadsParcel = false;
    }

    // Update is called once per frame
    void Update () {
        if (shouldMove)
        {
            timePast += Time.deltaTime;
            transform.position = Vector3.Lerp(new Vector3(PosX, 0, PosY), new Vector3(targetX, 0, targetY), timePast * turnSpeed);
            if (timePast * turnSpeed >= 0.99f)
            {
                transform.position = new Vector3(targetX, 0, targetY);
                // reached goal
                shouldMove = false;
                grid.RemoveRobot(PosX, PosY);
                PosX = targetX;
                PosY = targetY;

                // ask for further instructions
                robotDriver.Think();
            }
        }
        else if (shouldTurn)
        {
            timePast += Time.deltaTime;
            Vector3 dir = new Vector3(0, dirRotations[targetDir], 0);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, dirRotations[Orientation], 0), Quaternion.Euler(dir), timePast * turnSpeed);

            if (timePast * turnSpeed >= 0.99f)
            {
                transform.rotation = Quaternion.Euler(dir);
                // reached goal
                shouldTurn = false;
                Orientation = targetDir;

                // ask for further instructions
                robotDriver.Think();
            }
        }
        else
        {
            robotDriver.Think();
        }
    }


}
