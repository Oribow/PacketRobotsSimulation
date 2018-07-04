using UnityEngine;
using System.Collections;

public enum Orientation
{
    NORTH = 0,
    EAST = 1,
    SOUTH = 2,
    WEST = 3,
}

public static class OrientationExtensions{


    public static float ToDegrees(this Orientation o)
    {
        return (int)o * 90;
    }
}
