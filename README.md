It's a Unity 2018 project.

**Important files:** RobotDriver (it implements the driving system)

#What is different from the given API:

Uses two int's (named posX, posY for example) to give positions on the grid. (Maybe should have created a position class for that, but now it's to late)

Uses int to give absolute and relative direction, depending on context. (Could have used an enum for that)

0 = North
1 = East
2 = South
3 = West

Only the RobotDriver tries to implement the API, the rest is just how I think it's the easiest

#How does the current implementation in RobotDriver work?

Assumes the robot starts on a Waypoint. 
Gets the target direction.
Choose to either: make a u-turn, cross crossroad forward, left or right.
Follows through with that decision, until it's either at another waypoint(begin from step 1 again) or at the target location.

**It does not currently check for collisions with other robots**
-> also the provided methods to do that from the scanner are not tested yet (but should probably work)

#What outside calls does RobotDriver get?

Think():

Everytime the robot reaches a new tile (is fully on it and on no other tile) Think() is called.
Now the robot has to give instructions (only one call to either driveForward, turnLeft, turnRight allowed) to the motor.

Stop():

Robot should stop now and do nothing.

DriveTo():

Updates the current target of the robot.

**The goal of the RobotDriver is to reach a given target or do nothing!**

#How does loading and unloading of the package handled?

RoboTester does this. No further actions required.

#What does RoboTester do?

Sends the robot to a random target slot (a tile right above the slot). Unloads the package and tells the robot to drive back to it's home tile (the tile the robot started at). Repeat forever.
