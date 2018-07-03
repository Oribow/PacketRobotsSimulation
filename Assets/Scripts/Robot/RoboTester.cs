using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboTester : MonoBehaviour {

    public GameObject robot;
    public int startX;
    public int startY;

    public Transform goal;

    private RobotMotor motor;
    private bool nextMoveHome = false;

	// Use this for initialization
	void Start () {
        motor = Instantiate<GameObject>(robot, new Vector3(startX, 0, startY), Quaternion.identity).GetComponent<RobotMotor>();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawLine(motor.transform.position, new Vector3(motor.robotDriver.targetX, 0, motor.robotDriver.targetY), Color.red);
        if (motor.robotDriver.brainState == RobotDriver.BrainState.Idle)
        {
            if (nextMoveHome)
            {
                if (motor.HasParcel)
                {
                    motor.UnloadLoadParcel();
                }
                else if (motor.UnloadsParcel)
                {

                }
                else {
                    motor.robotDriver.DriveTo(startX, startY);
                    nextMoveHome = false;
                }
            }
            else
            {
                motor.LoadParcel();
                nextMoveHome = true;
                int x, y;
                do
                {
                    x = Random.Range(0, GridLoader.grid.Width / 3 - 3);
                    y = Random.Range(0, GridLoader.grid.Height / 3);
                    x = 5 + x * 3;
                    y = 3 + y * 3;
                } while (!GridLoader.grid.IsTileFree(x, y));
                motor.robotDriver.DriveTo(x, y);
            }
        }
	}
}
