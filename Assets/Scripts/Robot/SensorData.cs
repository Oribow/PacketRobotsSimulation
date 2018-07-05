using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorData
{
    public bool usedFlag;
    Position pos;
    PositionType pType;
    Orientation or;
    bool blockedFront;
    bool blockedLeft;
    bool blockedRight;
    bool blockedWaypointAhead;
    bool blockedWaypointLeft;
    bool blockedWaypointRight;
    bool blockedCrossroadAhead;
    bool blockedCrossroadRight;

    public SensorData(Position pos,
    PositionType pType,
    Orientation or,
    bool blockedFront,
    bool blockedLeft,
    bool blockedRight,
    bool blockedWaypointAhead,
    bool blockedWaypointLeft,
    bool blockedWaypointRight,
    bool blockedCrossroadAhead,
    bool blockedCrossroadRight)
    {
        this.pType = pType;
        this.pos = pos;
        this.or = or;
        this.blockedFront = blockedFront;
        this.blockedLeft = blockedLeft;
        this.blockedRight = blockedRight;
        this.blockedWaypointAhead = blockedWaypointAhead;
        this.blockedWaypointLeft = blockedWaypointLeft;
        this.blockedWaypointRight = blockedWaypointRight;
        this.blockedCrossroadAhead = blockedCrossroadAhead;
        this.blockedCrossroadRight = blockedCrossroadRight;
    }

    public Position Pos()
    {
        return pos;
    }

    public PositionType PosType()
    {
        return pType;
    }

    public Orientation PosOrientation()
    {
        return or;
    }

    public bool BlockedFront()
    {
        return blockedFront;
    }

    public bool BlockedLeft()
    {
        return blockedLeft;
    }

    public bool BlockedRight()
    {
        return blockedRight;
    }

    public bool BlockedWaypointAhead()
    {
        return blockedWaypointAhead;
    }

    public bool BlockedWaypointLeft()
    {
        return blockedWaypointLeft;
    }

    public bool BlockedWaypointRight()
    {
        return blockedWaypointRight;
    }

    public bool BlockedCrossroadAhead()
    {
        return blockedCrossroadAhead;
    }

    public bool BlockedCrossroadRight()
    {
        return blockedCrossroadRight;
    }
}
