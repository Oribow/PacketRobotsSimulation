using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class RobotMotor : MonoBehaviour
{
    public Position StartPos { get; private set; }
    public Position Pos { get; private set; }
    public Orientation Orientation { get; private set; }

    // Unity stuff
    public float speed;
    public float turnSpeed;
    private new Animation animation;

    // movement stuff
    private enum State
    {
        Idle,
        Turn,
        MoveForward,
        Unload,
        Load,
    }

    private State state;
    private Position target;
    private Orientation targetDir;
    private float timePast;
    private System.Action callback;

    private Grid grid;

    private void Awake()
    {
        animation = GetComponent<Animation>();
        Pos = new Position(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        StartPos = Pos;
    }

    private void Start()
    {
        grid = GridLoader.grid;
        grid.PlaceRobot(Pos);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.MoveForward:
                timePast += Time.deltaTime;
                transform.position = Vector3.Lerp(new Vector3(Pos.x, 0, Pos.y), new Vector3(target.x, 0, target.y), timePast * speed);
                if (timePast * speed >= 0.99f)
                {
                    Pos = target;
                    transform.position = new Vector3(Pos.x, 0, Pos.y);
                    grid.RemoveRobot(Pos);
                    state = State.Idle;
                    callback.Invoke();
                }
                break;
            case State.Turn:
                timePast += Time.deltaTime;
                Vector3 dir = new Vector3(0, targetDir.ToDegrees(), 0);
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, Orientation.ToDegrees(), 0), Quaternion.Euler(dir), timePast * turnSpeed);

                if (timePast * turnSpeed >= 0.99f)
                {
                    Orientation = targetDir;
                    transform.rotation = Quaternion.Euler(dir);
                    state = State.Idle;
                    callback.Invoke();
                }
                break;
            case State.Unload:
                animation.Play("ParcelUnload");
                state = State.Idle;
                break;

            case State.Load:
                animation.Play("ParcelLoad");
                state = State.Idle;
                break;
        }
    }

    public void DriveForward(System.Action callback)
    {
        target = Pos + Position.Forward * Orientation;
        state = State.MoveForward;
        timePast = 0;
        grid.PlaceRobot(target);
        this.callback = callback;
    }

    public void TurnLeft(System.Action callback)
    {
        targetDir = (Orientation)(((int)Orientation + 3) % 4);
        state = State.Turn;
        timePast = 0;
        this.callback = callback;
    }

    public void TurnRight(System.Action callback)
    {
        targetDir = (Orientation)(((int)Orientation + 1) % 4);
        state = State.Turn;
        timePast = 0;
        this.callback = callback;
    }

    public void StartUnload(System.Action callback)
    {
        this.callback = callback;
        state = State.Unload;
    }

    public void UnloadFinished()
    {
        this.callback.Invoke();
    }

    public void LoadParcel()
    {
        state = State.Load;
    }
}
