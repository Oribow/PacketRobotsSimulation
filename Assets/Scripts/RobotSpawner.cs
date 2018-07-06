using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    public int offsetX = 1;
    public int offsetY = 2;
    public bool spawn = false;
    public int count = 10;
    public GameObject robot;

    private Grid grid;

    // Use this for initialization
    void Start()
    {
        grid = GridLoader.grid;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            spawn = false;

            for (int i = 0; i < count; i++)
            {
                if (grid.IsTileFree(new Position(offsetX + i * 3, offsetY)))
                    Instantiate(robot, new Vector3(offsetX + 3 * i, 0, offsetY), Quaternion.identity);
            }
        }
    }
}
