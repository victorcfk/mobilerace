using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GridGenerator : MonoBehaviour {

    public Vector3 btmLeftRoomCent;
    
    public float defRoomHeight;
    public float defRoomWidth;
    
    int roomHeightCount;
    int roomWidthCount;
    
    public GameObject[] funiture_list;
    public GameObject CornerObj;
    public GameObject CenterObj;
    
    //public Room RoomParent;
    //public Wall LeftWallParent;
    //public Wall RightWallParent;
    //public Wall TopWallParent;
    //public Wall BtmWallParent;
    
    Vector3 roomCent;
    
    Vector3 btmLeft;
    Vector3 topLeft;
    
    Vector3 btmRight;
    Vector3 topRight;
    
    public List<Room> rooms;
    
    public int rowCount = 5;
    public int colCount = 2;
    public Room[][] roomArr;// = new Room[][];
    //Template type object.
    
    // Use this for initialization
    void Start()
    {
        GenerateRoomGrid(rowCount, colCount);
        
    }
    
    void GenerateRoomGrid(int rowCount, int colCount)
    {
        roomArr = new Room[rowCount][];
        
        //Vector3 roomCent = Vector3.zero;
        for (int i = 0; i < rowCount; i++)
        {
            roomArr[i] = new Room[colCount];
            
            for (int j = 0; j < colCount; j++)
            {
                Room temp;
                float usedHeight;
                float usedWidth;
                
                usedHeight = defRoomHeight;// +Random.Range(-25, 25);
                usedWidth = defRoomWidth;// +Random.Range(-25, 25);
                
                WALL_TEMPLATE? leftRestrict = null;
                WALL_TEMPLATE? topRestrict = null;
                WALL_TEMPLATE? btmRestrict = null;
                WALL_TEMPLATE? rightRestrict = null;
                
                
                if (i <= 0) leftRestrict = WALL_TEMPLATE.BLOCKED;
                if (i >= (rowCount-1)) rightRestrict = WALL_TEMPLATE.BLOCKED;
                
                if (j <= 0) btmRestrict = WALL_TEMPLATE.BLOCKED;
                if (j >= (colCount - 1)) topRestrict = WALL_TEMPLATE.BLOCKED;
                
                if((i-1)>=0){
                    leftRestrict = roomArr[i - 1][j].rightWall.template;
                }
                
                if ((j - 1) >= 0)
                {
                    btmRestrict = roomArr[i][j-1].topWall.template;
                }
                
                
                temp = GenerateRoom(
                    btmLeftRoomCent + new Vector3(i*usedWidth, j*usedHeight, 0),
                    usedWidth,usedHeight,
                    topRestrict,leftRestrict,rightRestrict,btmRestrict);
                
                rooms.Add(temp);
                
                roomArr[i][j] = temp;// rooms[0];
            }
        }
        
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                if (i + 1 <= rowCount-1) roomArr[i][j].rightRoom = roomArr[i + 1][j];
                
                if (i - 1 >= 0) roomArr[i][j].leftRoom = roomArr[i - 1][j];
                
                if (j + 1 <= colCount - 1) roomArr[i][j].topRoom = roomArr[i][j + 1];
                
                if (j - 1 >= 0) roomArr[i][j].btmRoom = roomArr[i][j - 1];
            }
        }
        
//        foreach (Room rm in rooms)
//        {
//            if (rm.topRoom != null)
//            {
//                Corridor corr = (new GameObject("VertCorridor")).AddComponent<Corridor>();
//                corr.transform.position = rm.transform.position;
//                
//                ConnectWallsViaCorridor(rm.getTopMostWall(), rm.topRoom.getBtmMostWall(),corr);
//            }
//            
//            
//            if (rm.rightRoom != null)
//            {
//                Corridor corr = (new GameObject("HoriCorridor")).AddComponent<Corridor>();
//                corr.transform.position = rm.transform.position;
//                
//                ConnectWallsViaCorridor(rm.getRightMostWall(), rm.rightRoom.getLeftMostWall(),corr);
//            }
//        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawLine(btmLeft, topLeft, Color.red);
        //Debug.DrawLine(topLeft, topRight, Color.white);
        //Debug.DrawLine(topRight, btmRight, Color.blue);
        //Debug.DrawLine(btmRight, btmLeft, Color.green);
        
        //foreach (Room rm in rooms)
        //{
        
        //    Debug.DrawLine(rm.transform.position, rm.getRightMostWall().middle, Color.red);
        //    Debug.DrawLine(rm.transform.position, rm.getLeftMostWall().middle, Color.white);
        //    Debug.DrawLine(rm.transform.position, rm.getBtmMostWall().middle, Color.blue);
        //    Debug.DrawLine(rm.transform.position, rm.getTopMostWall().middle, Color.green);
        
        //}
        
    }
    
    Room GenerateRoom(Vector3 roomCenter,
                      float roomWidth,
                      float roomHeight,
                      WALL_TEMPLATE? topWallRestriction = null,
                      WALL_TEMPLATE? leftWallRestriction = null,
                      WALL_TEMPLATE? rightWallRestriction = null,
                      WALL_TEMPLATE? btmWallRestriction = null)
    {
        Room roomParent;
        Wall leftWall;
        Wall rightWall;
        Wall topWall;
        Wall btmWall;
        
        
        roomParent = (new GameObject("Room")).AddComponent<Room>();
        roomParent.transform.position = roomCenter;
        
        
        leftWall = (new GameObject("LeftWallParent")).AddComponent<Wall>();
        leftWall.transform.position = roomParent.transform.position;
        leftWall.transform.parent = roomParent.transform;
        
        rightWall = (new GameObject("RightWallParent")).AddComponent<Wall>();
        rightWall.transform.position = roomParent.transform.position;
        rightWall.transform.parent = roomParent.transform;
        
        
        topWall = (new GameObject("TopWallParent")).AddComponent<Wall>();
        topWall.transform.position = roomParent.transform.position;
        topWall.transform.parent = roomParent.transform;
        
        btmWall = (new GameObject("BtmWallParent")).AddComponent<Wall>();
        btmWall.transform.position = roomParent.transform.position;
        btmWall.transform.parent = roomParent.transform;
        
        
        DefineBoundaries(roomCenter, roomWidth, roomHeight);
        
        if(leftWallRestriction != null)
            GenerateWall(btmLeft, topLeft, CornerObj, (WALL_TEMPLATE)leftWallRestriction, 5, leftWall);
//        else
//            GenerateWall(btmLeft, topLeft, CornerObj, (WALL_TEMPLATE)EnumRand(typeof(WALL_TEMPLATE)), 5, leftWall);
        
        if (topWallRestriction != null)
            GenerateWall(topLeft, topRight, CornerObj, (WALL_TEMPLATE)topWallRestriction, 5, topWall);
//        else
//            GenerateWall(topLeft, topRight, CornerObj, (WALL_TEMPLATE)EnumRand(typeof(WALL_TEMPLATE)), 5, topWall);
        
        
        if (rightWallRestriction != null)
            GenerateWall(topRight, btmRight, CornerObj, (WALL_TEMPLATE)rightWallRestriction, 5, rightWall);
//        else
//            GenerateWall(topRight, btmRight, CornerObj, (WALL_TEMPLATE)EnumRand(typeof(WALL_TEMPLATE)), 5, rightWall);
        
        
        if (btmWallRestriction != null)
            GenerateWall(btmRight, btmLeft, CornerObj, (WALL_TEMPLATE)btmWallRestriction, 5, btmWall);
//        else
//            GenerateWall(btmRight, btmLeft, CornerObj, (WALL_TEMPLATE)EnumRand(typeof(WALL_TEMPLATE)), 5, btmWall);
        
        
        roomParent.walls.Add(leftWall);
        roomParent.leftWall = leftWall;
        leftWall.adjRooms.Add(roomParent);
        
        roomParent.walls.Add(topWall);
        roomParent.topWall = topWall;
        topWall.adjRooms.Add(roomParent);
        
        roomParent.walls.Add(rightWall);
        roomParent.rightWall = rightWall;
        rightWall.adjRooms.Add(roomParent);
        
        roomParent.walls.Add(btmWall);
        roomParent.btmWall = btmWall;
        btmWall.adjRooms.Add(roomParent);
        
        FillRoom(btmLeft, topRight, (FUNITURE_TEMPLATE.CENTRAL), funiture_list, roomParent);
        
        return roomParent;
    }
    
    
    /// <summary>
    /// Assume room is at 0,0, then shift?
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void DefineBoundaries(Vector3 roomCent,float roomWidth, float roomHeight)
    {
        btmLeft = roomCent + new Vector3(-roomWidth / 2, -roomHeight / 2, 0);
        topLeft = roomCent + new Vector3(-roomWidth / 2, roomHeight / 2, 0);
        
        btmRight = roomCent + new Vector3(roomWidth / 2, -roomHeight / 2, 0);
        topRight = roomCent + new Vector3(roomWidth / 2, roomHeight / 2, 0);
        
        //print(roomCent);
        //print(btmLeft);
        //print(topLeft);
        //print(btmRight);
        //print(topRight);
        
        //GameObject.Instantiate(CenterObj, roomCent, CenterObj.transform.rotation);
        
        //GameObject.Instantiate(CornerObj, btmLeft, CenterObj.transform.rotation);    //bottom Left
        //GameObject.Instantiate(CornerObj, topLeft, CenterObj.transform.rotation);      //top left
        
        //GameObject.Instantiate(CornerObj, btmRight, CenterObj.transform.rotation);      //bottom right
        //GameObject.Instantiate(CornerObj, topRight, CenterObj.transform.rotation);      //top right
        
    }
    
    void GenerateWall(Vector3 start, Vector3 end, GameObject WallBrick, WALL_TEMPLATE wall_temp, int openingBrickSize = 0, Wall Parent = null)
    {
        
        float PrefabRadius = 1;// = Vector3.Distance(WallBrick.collider.bounds.max , WallBrick.collider.bounds.min);
        Parent.middle = (start + end) / 2;
        Parent.transform.position = Parent.middle;
        Parent.template = wall_temp;
        
        switch (wall_temp)
        {
            case WALL_TEMPLATE.BLOCKED:
                GenerateLineOfPrefabs(start, end, WallBrick, Parent.gameObject, Parent.wallPieces);
                Parent.leftMostDoorEdge = start;
                Parent.rightMostDoorEdge = end;
                break;
                
            case WALL_TEMPLATE.NO_WALL:
                //GenerateLineOfPrefabs(start, end, WallBrick, Parent);
                //do nothing
                Parent.leftMostDoorEdge = start;
                Parent.rightMostDoorEdge = end;
                break;
                
            case WALL_TEMPLATE.TWO_OPENINGS:
            case WALL_TEMPLATE.ONE_OPENINGS:
                
                Vector3 middleStart = (end + start) / 2 - (end - start).normalized * openingBrickSize * PrefabRadius;
                Vector3 middleEnd = (end + start) / 2 + (end - start).normalized * openingBrickSize * PrefabRadius;
                
                Parent.leftMostDoorEdge = middleStart;
                Parent.rightMostDoorEdge = middleEnd;
                
                GenerateLineOfPrefabs(start, middleStart, WallBrick, Parent.gameObject, Parent.wallPieces);
                GenerateLineOfPrefabs(end, middleEnd, WallBrick, Parent.gameObject, Parent.wallPieces);
                break;
                
            default:
                break;
                //case WALL_TEMPLATE.BLOCKED:
                //    GenerateLineOfPrefabs(start, end, WallBrick, Parent);
                //    break;
        }
        
        
        
    }
    
    void FillRoom(Vector3 btmLeft, Vector3 topRight, FUNITURE_TEMPLATE fun_temp, GameObject[] funiture_list, Room Parent = null)
    {
        GameObject tempObj;
        switch (fun_temp)
        {
            case FUNITURE_TEMPLATE.OPEN:
                //do nothing
                break;
            case FUNITURE_TEMPLATE.CENTRAL:
                
                tempObj = funiture_list[Random.Range(0, funiture_list.Length)];
                //PlaceObjectWithinBoxBoundary((btmLeft + topRight) / 2, (btmLeft + topRight) / 2, tempObj, Parent.gameObject);
                
                //PlaceObjectWithinBoxBoundary((btmLeft + topRight) / 2, (btmLeft + topRight) / 2, tempObj, Parent);
                
                if (Parent == null)
                    Instantiate(tempObj, (btmLeft + topRight) / 2, tempObj.transform.rotation);
                else
                    ((GameObject)Instantiate(tempObj, (btmLeft + topRight) / 2, tempObj.transform.rotation)).transform.parent = Parent.transform;
                
                
                break;
                
            case FUNITURE_TEMPLATE.CORRIDOR:
            case FUNITURE_TEMPLATE.SCATTERED:
                
                int num = Random.Range(10, 20);
                for (int i = 0; i < num; i++)
                {
                    tempObj = funiture_list[Random.Range(0, funiture_list.Length)];
                    
                    PlaceObjectWithinBoxBoundary(btmLeft, topRight, tempObj, Parent.gameObject);
                }
                //Instantiate(tempObj, (btmLeft + topRight) / 2, tempObj.transform.rotation);
                break;
                
                
                //case FUNITURE_TEMPLATE.CORRIDOR:
                //    int num = Random.Range(10, 20);
                //    for (int i = 0; i < num; i++)
                //    {
                //        tempObj = funiture_list[Random.Range(0, funiture_list.Length)];
                
                //        PlaceObjectWithinBoxBoundary(btmLeft, topRight, tempObj, Parent);
                //    }
                //    //Instantiate(tempObj, (btmLeft + topRight) / 2, tempObj.transform.rotation);
                //    break;
            default:
                break;
        }
    }
    
    void ConnectWallsViaReplacement(Wall a, Wall b)
    {
        b.adjRooms[0].walls.Add(a);
        b.adjRooms[0].walls.Remove(b);
        
        Destroy(b.gameObject);
    }
    
    void ConnectWallsViaCorridor(Wall a, Wall b, Corridor c = null)
    {
        if (a == null || b == null)
            return;
        
        if (a.template == WALL_TEMPLATE.BLOCKED || b.template == WALL_TEMPLATE.BLOCKED)
            return;
        
        //GenerateLineOfPrefabs(a.middle, b.middle, CornerObj);
        if (c != null)
        {
            c.walls.Add(a);
            c.walls.Add(b);
        }
        
        GenerateLineOfPrefabs(a.leftMostDoorEdge, b.rightMostDoorEdge, CornerObj,c.gameObject);
        GenerateLineOfPrefabs(a.rightMostDoorEdge, b.leftMostDoorEdge, CornerObj,c.gameObject);
    }
    
    
    void GenerateLineOfPrefabs(Vector3 start, Vector3 end, GameObject WallBrick, GameObject Parent = null, List<GameObject> ReferenceList = null)
    {
        float PrefabRadius;// = Vector3.Distance(WallBrick.collider.bounds.max , WallBrick.collider.bounds.min);
        
        //print("PrefabRadius: " + PrefabRadius);
        
        //print(WallBrick.GetComponent<MeshFilter>().mesh.bounds.center);
        //print(WallBrick.GetComponent<MeshFilter>().mesh.bounds.extents);
        //print(WallBrick.GetComponent<MeshFilter>().mesh.bounds.max);
        //print(WallBrick.GetComponent<MeshFilter>().mesh.bounds.min);
        
        PrefabRadius = 1;
        //print("PrefabRadius: " + PrefabRadius);
        
        int numOfPrefabsToGen = (int)(Vector3.Distance(start, end) / PrefabRadius);
        
        //print("numOfPrefabsToGen: " + numOfPrefabsToGen);
        
        Vector3 DirOfGen = (end - start).normalized;
        Vector3 pointer = start;
        
        GameObject temp;
        for (int i = 0; i < numOfPrefabsToGen; i++)
        {
            
            temp = (GameObject)Instantiate(WallBrick, pointer, WallBrick.transform.rotation);
            
            if (Parent != null)
                temp.transform.parent = Parent.transform;
            
            if (ReferenceList != null)
                ReferenceList.Add(temp);
            
            pointer += (DirOfGen * PrefabRadius);
        }
        
    }
    
    /// <summary>
    /// Place objects within a AABB, still requires defensive prog work
    /// </summary>
    /// <param name="btmLeft"></param>
    /// <param name="topRight"></param>
    /// <param name="obj"></param>
    void PlaceObjectWithinBoxBoundary(Vector3 btmLeft, Vector3 topRight, GameObject obj, GameObject Parent = null)
    {
        float PrefabRadius = 2;
        
        float CorrectedTopRightX = topRight.x - PrefabRadius;
        float CorrectedTopRightY = topRight.y - PrefabRadius;
        
        float CorrectedBtmLeftX = btmLeft.x + PrefabRadius;
        float CorrectedBtmLeftY = btmLeft.y + PrefabRadius;
        
        if (CorrectedTopRightX < CorrectedBtmLeftX)
            CorrectedTopRightX = CorrectedBtmLeftX = (topRight.x + topLeft.x) / 2;
        
        if (CorrectedTopRightY < CorrectedBtmLeftY)
            CorrectedTopRightY = CorrectedBtmLeftY = (topRight.y + topLeft.y) / 2;
        
        float xPos = Random.Range(CorrectedBtmLeftX, CorrectedTopRightX);
        float yPos = Random.Range(CorrectedBtmLeftY, CorrectedTopRightY);
        
        //GameObject.Instantiate(obj, new Vector3(xPos, yPos, 0), obj.transform.rotation);
        
        if (Parent == null)
            Instantiate(obj, new Vector3(xPos, yPos, 0), obj.transform.rotation);
        else
            ((GameObject)Instantiate(obj, new Vector3(xPos, yPos, 0), obj.transform.rotation)).transform.parent = Parent.transform;
        
    }
    
    /// <summary>
    /// Place objects within a circle lying on the xy plane
    /// </summary>
    /// <param name="center">Center Of Circle</param>
    /// <param name="radius">Radius Of Circle</param>
    /// <param name="obj">the object</param>
    void PlaceObjectWithinRadius(Vector3 center, float radius, GameObject obj, GameObject Parent = null)
    {
        if (Parent == null)
            Instantiate(obj, Vec2to3(Random.insideUnitCircle * radius) + center, obj.transform.rotation);
        else
            ((GameObject)Instantiate(obj, Vec2to3(Random.insideUnitCircle * radius) + center, obj.transform.rotation)).transform.parent = Parent.transform;
    }
    
    static Vector3 Vec2to3(Vector2 val)
    {
        return new Vector3(val.x, val.y);
    }
    
    static int EnumRand(System.Type e)
    {
        return Random.Range(0,
                            System.Enum.GetNames(e).Length);
        
        //return new Vector3(val.x, val.y);
    }
}
