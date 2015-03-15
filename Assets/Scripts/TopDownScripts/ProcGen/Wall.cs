using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WALL_TEMPLATE
{
    NO_WALL,
    BLOCKED,
    ONE_OPENINGS,
    TWO_OPENINGS,
}

public class Wall : MonoBehaviour {

    public List<Gate> gates = new List<Gate>();
    public List<GameObject> wallPieces = new List<GameObject>();

    public List<Room> adjRooms = new List<Room>();

    public Vector3 middle;
    public Vector3 leftMostDoorEdge;
    public Vector3 rightMostDoorEdge;

    public WALL_TEMPLATE template;

    public Corridor c;
}
