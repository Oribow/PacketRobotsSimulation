using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour {

    public GameObject deliveryStation;
    public GameObject waypointTile;
    public GameObject crossRoadTile;
    public Transform tileRoot;

    private Grid grid;

    private void Start()
    {
        this.grid = GridLoader.grid;
        VisualizeWorld();
    }

    private void VisualizeWorld()
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var tile = grid.GetTileType(new Position(x, y));
                switch (tile)
                {
                    case PositionType.STATION:
                        Instantiate<GameObject>(deliveryStation, new Vector3(x, 0, y), Quaternion.identity, tileRoot);
                        break;
                    case PositionType.WAYPOINT:
                        Instantiate<GameObject>(waypointTile, new Vector3(x, 0, y), Quaternion.identity, tileRoot);
                        break;
                    case PositionType.CROSSROADS:
                        Instantiate<GameObject>(crossRoadTile, new Vector3(x, 0, y), Quaternion.identity, tileRoot);
                        break;
                }
               
            }
        }
    }
}
