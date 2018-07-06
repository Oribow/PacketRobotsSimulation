using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour {

    public static int numRoboter = 0;
    public static int numPackagesDelivered;

    public static void PackageDeliverd()
    {
        numPackagesDelivered += 1;
    }

    private void OnGUI()
    {
        GUILayout.Label(string.Format("Number of Robots: {0}", numRoboter));
        if (numPackagesDelivered != 0)
        GUILayout.Label(string.Format("Avg. Packages Delivered Per Second: {0:0.00}", numPackagesDelivered / Time.time));
    }
}
