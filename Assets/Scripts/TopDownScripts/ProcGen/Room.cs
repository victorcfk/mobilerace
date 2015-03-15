using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FUNITURE_TEMPLATE
{
    CORRIDOR,
    OPEN,
    CENTRAL,
    SCATTERED,
}

public class Room : MonoBehaviour {

    public List<Wall> walls = new List<Wall>();
    public List<Room> adjRooms = new List<Room>();

    public Wall rightWall;
    public Wall leftWall;
    public Wall topWall;
    public Wall btmWall;

    public Room rightRoom;
    public Room leftRoom;
    public Room topRoom;
    public Room btmRoom;

    public FUNITURE_TEMPLATE template;

    public Wall getRightMostWall()
    {
        if (rightWall != null)
            return rightWall;

        rightWall = walls[0];

        foreach (Wall w in walls)
        {
            if (w.middle.x > rightWall.middle.x)
                rightWall = w;
        }

        return rightWall;
    }

    public Wall getLeftMostWall()
    {
        if (leftWall != null)
            return leftWall;

        leftWall = walls[0];

        foreach (Wall w in walls)
        {
            if (w.middle.x < leftWall.middle.x)
                leftWall = w;
        }

        return leftWall;
    }

    public Wall getTopMostWall()
    {

        if (topWall != null)
            return topWall;

        topWall = walls[0];

        foreach (Wall w in walls)
        {
            if (w.middle.y > topWall.middle.y)
                topWall = w;
        }

        return topWall;
    }

    public Wall getBtmMostWall()
    {
        if (btmWall != null)
            return btmWall;

        btmWall = walls[0];

        foreach (Wall w in walls)
        {
            if (w.middle.y < btmWall.middle.y)
                btmWall = w;
        }

        return btmWall;
    }
}
