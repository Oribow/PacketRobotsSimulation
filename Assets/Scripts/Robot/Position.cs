using UnityEngine;
using System.Collections;

public struct Position
{
    public static Position Forward { get { return new Position(0, 1); } }
    public static Position Right { get { return new Position(1, 0); } }
    public static Position Backward { get { return new Position(0, -1); } }
    public static Position Left { get { return new Position(-1, 0); } }

    public int x;
    public int y;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Position operator +(Position a, Position b)
    {
        return new Position(a.x + b.x, a.y + b.y);
    }

    public static Position operator -(Position a, Position b)
    {
        return new Position(a.x - b.x, a.y - b.y);
    }

    public static Position operator *(Position a, int b)
    {
        return new Position(a.x * b, a.y * b);
    }

    public static Position operator *(Position a, Orientation b)
    {
        switch (b)
        {
            case Orientation.NORTH:
                return a;
            case Orientation.EAST:
                return RotateRight(a);
            case Orientation.WEST:
                return RotateLeft(a);
            case Orientation.SOUTH:
                return a * -1;
        }
        throw new System.Exception("Unkown orientation");
    }

    public static Position operator /(Position a, int b)
    {
        return new Position(a.x / b, a.y / b);
    }

    public static Position RotateLeft(Position p)
    {
        return new Position(-p.y, p.x);
    }

    public static Position RotateRight(Position p)
    {
        return new Position(p.y, -p.x);
    }

    public static bool operator== (Position a, Position b)
    {
        return a.x == b.x && b.y == a.y;
    }

    public static bool operator!=(Position a, Position b)
    {
        return a.x != b.x || b.y != a.y;
    }
}
