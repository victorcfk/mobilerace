using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum TrackSegmentType
{
    STRAIGHT = 0 ,
    LEFT_SEMI = 1 ,
    RIGHT_SEMI = 2 ,
    LEFT_S = 3 ,
    RIGHT_S = 4 ,
    LEFT_EXPO = 5 ,
    RIGHT_EXPO = 6 ,
}

[System.Serializable]
public enum TrackMatType
{
    STRAIGHT = 0 ,
    LEFT = 1 ,
    RIGHT = 2 ,
    BTM = 3,
}

[System.Serializable]
public class TrackSegment
{
    public int pointCount{  get {   return trackPoints.Length;  }   }

    public TrackPoint[] trackPoints;

    public TrackSegmentType type;
}

[System.Serializable]
public class TrackPoint
{
    TrackBuildRPoint _point;
    public TrackBuildRPoint point
    {
        get{
//            if (_point == null)
//            {
//                _point = TrackManager.instance.gameObject.AddComponent<TrackBuildRPoint>();
//            }

            return _point;
        }

        set
        {
            _point = value;
        }
    }

    Vector3 _position;
    public Vector3 position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;
        }
    }

    float _DistFromStart;
    public float DistFromStart
    {
        get
        {
            return _DistFromStart;
        }
        
        set
        {
            _DistFromStart = value;
        }
    }

}


public class TrackManager : MonoBehaviour {

    static TrackManager _instance;
    
    //This is the public reference that other classes will use
    public static TrackManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<TrackManager> ();
            return _instance;
        }
    }

    [Space (10)]
    
    [SerializeField]
    Material
        fwdTrackMat;

    [SerializeField]
    Material
        btmTrackMat;

    [SerializeField]
    Material
        rightTrackMat;

    [SerializeField]
    Material
        leftTrackMat;

    [SerializeField]
    Material
        borderMat;
    [SerializeField]
    Material
        groundMat;

    [Range (0,200)]
    public int numOfTrackSegments = 20;
    [Range (0,200)]
    public int pointsPerSegment = 24; //must be even
    [Range (-20,20)]
    public float crownAngle = -4;

    [HideInInspector]
    public TrackBuildRTrack track;

    [Space (10)]
    [SerializeField]
    public List<TrackSegment> trackSegs;
    public List<TrackPoint> listOfTrackPoints;

    Vector3 UpperBounds;
    Vector3 LowerBounds;

    [Range (0,90)]
    public float MaxLeftTurnCant = 30;
    [Range (0,90)]
    public float MaxRightTurnCant = 30;

    public AnimationCurve GradualCurve;
    public AnimationCurve ExpoCurve;
    public AnimationCurve SemiCircle;
    public AnimationCurve SCurve;

    public Vector2 textureUnitSize = new Vector2(50, 50);

    public TeleportVehToStart teleporta;

    public TrackMatType trackMatType;

    [Range (0, 200)]
    public int numOfBuildingsToGenerate;
    public GameObject[] Buildings;

    Vector3 firstPointGenerated 
    {
        get 
        { 
            if(listOfTrackPoints != null) 
                return listOfTrackPoints[0].position; 
            else 
                return Vector3.zero;
        }
    }

    Vector3 lastPointGenerated
    {
        get 
        { 
            if(listOfTrackPoints != null) 
                return listOfTrackPoints[listOfTrackPoints.Count-1].position; 
            else 
                return Vector3.zero;
        }
    }

    public List<TrackSegment> GenerateTrackSegments( List<TrackSegment> trackSegments )
    {
        //===================================================
        //Decide straight or curved
        //===================================================

        TrackSegmentType tstToGen = TrackSegmentType.STRAIGHT;

        int straightleftright = 2;
        int straightTrackUpperLimit = 4;
        
        trackSegments = new List<TrackSegment>();

        Vector3 lastDirOnGeneratedTrackSegments = Vector3.forward;
        Vector3 currLastPtOnTrackSegArr = Vector3.zero;
        Vector3 currSecLastPtOnGenTrackSegArr = Vector3.zero;

        for (int j =0; j <numOfTrackSegments; j++) 
        {
            trackSegments.Add( 
                              GenerateFromCurveTrackSegment(
                currLastPtOnTrackSegArr,
                lastDirOnGeneratedTrackSegments,
                pointsPerSegment,
                Random.Range(400,600),tstToGen,listOfTrackPoints));
            
            //=================================================================
            
            switch(Random.Range(0,5))
            {
                case 0: tstToGen = TrackSegmentType.STRAIGHT;       break;
                case 1: tstToGen = TrackSegmentType.LEFT_SEMI;      break;
                case 2: tstToGen = TrackSegmentType.RIGHT_SEMI;     break;
//                case 3: tstToGen = TrackSegmentType.LEFT_S;         break;
//                case 4: tstToGen = TrackSegmentType.RIGHT_S;         break;
                case 3: tstToGen = TrackSegmentType.LEFT_EXPO;      break;
                case 4: tstToGen = TrackSegmentType.RIGHT_EXPO;     break;
            }

            TrackSegment currLastTrackSeg = trackSegments[trackSegments.Count-1];

            int currLastPtOnTrackSegArrPos = currLastTrackSeg.trackPoints.Length-1;
            currLastPtOnTrackSegArr = currLastTrackSeg.trackPoints[currLastPtOnTrackSegArrPos].position;

            int currSecLastPtOnGenTrackSegArrPos = currLastTrackSeg.trackPoints.Length-2;
            currSecLastPtOnGenTrackSegArr = currLastTrackSeg.trackPoints[currSecLastPtOnGenTrackSegArrPos].position;

            lastDirOnGeneratedTrackSegments = (currLastPtOnTrackSegArr - 
                                               currSecLastPtOnGenTrackSegArr).normalized;

            //=================================================================
            Debug.DrawRay (currLastPtOnTrackSegArr, lastDirOnGeneratedTrackSegments * 50, Color.white, 5);
            Debug.DrawRay (currLastPtOnTrackSegArr, Vector3.up * 50, Color.red, 5);
            
        }

        return trackSegments;
    }

    public void UpdateDistanceFromStart()
    {
        listOfTrackPoints[0].DistFromStart = 0;
        for(int i =1; i <listOfTrackPoints.Count; i++)
        {   
            listOfTrackPoints[i].DistFromStart = 
                listOfTrackPoints[i-1].DistFromStart + Vector3.Distance(listOfTrackPoints [i].position,listOfTrackPoints [i-1].position);
        }
    }

    public void DropYPosOnListOfTrackPoints(float totalDropValue)
    {
        float TotalDistanceOfTrack = listOfTrackPoints [listOfTrackPoints.Count - 1].DistFromStart;

        for(int i =0; i <listOfTrackPoints.Count; i++)
        {   
            listOfTrackPoints[i].position = 
                listOfTrackPoints[i].position + 
                    Vector3.down * Mathf.Lerp(0,totalDropValue,listOfTrackPoints[i].DistFromStart/TotalDistanceOfTrack);

            //                listOfTrackPoints[i-1].DistFromStart + Vector3.Distance(listOfTrackPoints [i].position,listOfTrackPoints [i-1].position);
        }
    }

    public void InitializeTrackPoints (TrackBuildRTrack track)
    {
        trackSegs = GenerateTrackSegments(trackSegs);

        UpdateDistanceFromStart();
        
        DropYPosOnListOfTrackPoints(numOfTrackSegments * 25);

        //===================================================
        //Rotation bits
        //=======================================================
        int temp=0;
        for (int i =0; i <trackSegs.Count; i++)
        {   
            TrackSegment CurrTrackSeg = trackSegs[i];
            TrackPoint[] CurrTrackSegTrackpts = CurrTrackSeg.trackPoints;
           
//            DropPointsOnArray (CurrTrackSegTrackpts, 0.75f, 0.75f,temp*0.75f);
//            temp += CurrTrackSegTrackpts.Length;

            //We skip the last point of each segment to compensate for the fact it is repeated in the next segement's first point
            for (int j =0 ; j <CurrTrackSegTrackpts.Length-1; j+=1)
            {
//                Debug.Log("dasdas");
                TrackBuildRPoint bp = track.gameObject.AddComponent<TrackBuildRPoint>();
                CurrTrackSegTrackpts[j].point = bp;

                bp.baseTransform = transform;
                bp.position = CurrTrackSegTrackpts [j].position;
                bp.width = 150;

                bp.colliderSides = true;
                bp.renderBounds = false;
                bp.generateBumpers = false;
                bp.extrudeTrack = true;
                bp.extrudeTrackBottom = true;
                bp.crownAngle = crownAngle;

                float angle2;
                Vector3 axis2;
                bp.trackUpQ.ToAngleAxis (out angle2, out axis2);
//                Debug.DrawRay(bp.position,axis2.normalized *100,Color.yellow,6);

                //We get the forward control point based on the tiny i intervals for direction
                //=======================================================
                if (j < CurrTrackSegTrackpts.Length - 1) 
                {
                    bp.forwardControlPoint = (-CurrTrackSegTrackpts [j].position + CurrTrackSegTrackpts [j + 1].position).normalized*10 + CurrTrackSegTrackpts [j].position;
                }
                else 
                {
                    bp.forwardControlPoint = (CurrTrackSegTrackpts [j].position - CurrTrackSegTrackpts [j - 1].position).normalized*10 + CurrTrackSegTrackpts [j].position;
                }
                //=======================================================

                if (CurrTrackSeg.type == TrackSegmentType.LEFT_SEMI ||
                    CurrTrackSeg.type == TrackSegmentType.LEFT_EXPO 
//                    ||
//                    CurrTrackSeg.type == TrackSegmentType.LEFT_S
                    ) {
                    
                    //=============================================
                    float angle;
                    Vector3 axis;
                    bp.trackUpQ.ToAngleAxis (out angle, out axis);

                    Debug.DrawRay(bp.position,axis.normalized *100,Color.yellow,6);

                    if(axis.y > 0 )
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Length,0, MaxLeftTurnCant,GradualCurve), 
                            axis) * bp.trackUpQ;
                    else
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Length,0,-MaxLeftTurnCant,GradualCurve), 
                            axis) * bp.trackUpQ;

                    bp.position += Vector3.up * GetPosChangeCurveValue(j,CurrTrackSegTrackpts.Length,150,MaxLeftTurnCant,GradualCurve);

                    CurrTrackSegTrackpts [j].position = bp.position;
                    //=============================================
                } else
                    if (CurrTrackSeg.type == TrackSegmentType.RIGHT_SEMI ||
                        CurrTrackSeg.type == TrackSegmentType.RIGHT_EXPO 
//                        ||
//                        CurrTrackSeg.type == TrackSegmentType.RIGHT_S
                       ) { //right turn

                    //=============================================
                    float angle;
                    Vector3 axis;
                    bp.trackUpQ.ToAngleAxis (out angle, out axis);

                    Debug.DrawRay(bp.position,axis.normalized *100,Color.yellow,6);

                    if(axis.y > 0 )
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Length,0,-MaxRightTurnCant,GradualCurve), 
                            axis) * bp.trackUpQ;
                    else
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Length,0,MaxRightTurnCant,GradualCurve), 
                            axis) * bp.trackUpQ;

                    bp.position += Vector3.up * GetPosChangeCurveValue(j,CurrTrackSegTrackpts.Length,150,MaxRightTurnCant,GradualCurve);

                    CurrTrackSegTrackpts [j].position = bp.position;
                    //=============================================
                } 

                bp.type = CurrTrackSeg.type;

                UpperBounds.x = Mathf.Max (UpperBounds.x, bp.position.x);
                UpperBounds.y = Mathf.Max (UpperBounds.y, bp.position.y);
                UpperBounds.z = Mathf.Max (UpperBounds.z, bp.position.z);
                    
                LowerBounds.x = Mathf.Min (LowerBounds.x, bp.position.x);
                LowerBounds.y = Mathf.Min (LowerBounds.y, bp.position.y);
                LowerBounds.z = Mathf.Min (LowerBounds.z, bp.position.z);

                //If it is the first point on the track
                if(j==0)
                {
                    DrawCross(bp.position + Vector3.up,Color.green);
                    Debug.DrawLine(bp.position,bp.forwardControlPoint,Color.green,5);

                }
                 else
                {
                    DrawCross(bp.position + Vector3.up,Color.white);
                    Debug.DrawLine(bp.position,bp.forwardControlPoint,Color.white,5);
                }

                track.AddPoint (bp);
            }
        }
        
        track.meshResolution = 11;
        //track.Texture(0).textureUnitSize
        track.loop = false; 
        track.includeColliderRoof =false;
        track.trackBumpers = false;
        track.trackColliderWallHeight = 20;

        this.track = track;

        //Expand the bounds
        //================================

        //Expand the top and botom
        UpperBounds += Vector3.up *50;
        LowerBounds -= Vector3.up *50;

        //Expand the left and right
        UpperBounds += new Vector3(1,0,1) * 800;
        LowerBounds -= new Vector3(1,0,1) * 800;
        //================================

        DrawCross(firstPointGenerated, Color.magenta);
        DrawCross(lastPointGenerated, Color.magenta);


        Instantiate(teleporta, lastPointGenerated, Quaternion.identity);
    }

    float GetCantAngleCurveValue(float point, float totalCurvePointCount, float initialCantAngle, float maxCantAngle, AnimationCurve cantCurve, int type = 0)
    {

        return 
            Mathf.Lerp(initialCantAngle, maxCantAngle,
                       cantCurve.Evaluate(point / totalCurvePointCount));
    }

    float GetPosChangeCurveValue(float point, float totalCurvePointCount, float trackWidth, float angle, AnimationCurve displacementCurve, int type = 0)
    {
        float x = trackWidth/4;

        if (angle == 0)
            return 0;

        return 
            Mathf.Abs(Mathf.Lerp(0, x * Mathf.Cos(angle*4),
                       displacementCurve.Evaluate(point / totalCurvePointCount)));
    }

    int straightTrackSpacingModifier = 5; //This is used to compensate for the fact that the spacing between the straight track points are too close as compared to the curved tracks, we need to modify them to compensate.

    TrackSegment GenerateFromCurveTrackSegment(
        Vector3 lastPointAtInterval, 
        Vector3 dirAtEnd, 
        int pointsInSegment, 
        float distbetweenPoints, 
        TrackSegmentType type,
        List< TrackPoint> allTrackPoints = null)
    {
        TrackPoint[] trackPoints;

        switch (type)
        {
            case TrackSegmentType.STRAIGHT://15 400 - > 15
                trackPoints = GenerateStraight(lastPointAtInterval, dirAtEnd, pointsInSegment/straightTrackSpacingModifier, distbetweenPoints*straightTrackSpacingModifier * 15/400);        
                break;
                
            case TrackSegmentType.RIGHT_S://400 to 800
                trackPoints = GenerateCurvePointsTowardsRight(lastPointAtInterval, dirAtEnd, pointsInSegment*2, distbetweenPoints*2,SCurve);        
                break;
                
            case TrackSegmentType.LEFT_S:
                trackPoints = GenerateCurvePointsTowardsLeft(lastPointAtInterval, dirAtEnd, pointsInSegment*2, distbetweenPoints*2,SCurve);
                break;
                
            case TrackSegmentType.RIGHT_SEMI:
                trackPoints = GenerateCurvePointsTowardsRight(lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints,SemiCircle);
                break;
                
            case TrackSegmentType.LEFT_SEMI:
                trackPoints = GenerateCurvePointsTowardsLeft(lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints,SemiCircle);
                break;
                
            case TrackSegmentType.RIGHT_EXPO:
                trackPoints = GenerateCurvePointsTowardsRight(lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints,ExpoCurve);
                break;
                
            case TrackSegmentType.LEFT_EXPO:
                trackPoints = GenerateCurvePointsTowardsLeft(lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints,ExpoCurve);
                break;
                
            default:
                trackPoints = GenerateStraight(lastPointAtInterval, dirAtEnd, pointsInSegment/straightTrackSpacingModifier, distbetweenPoints*straightTrackSpacingModifier* 15/400);        
                break;
        }

        TrackSegment tsgt = new TrackSegment();

        tsgt.type = type;
        tsgt.trackPoints = trackPoints;

        if (allTrackPoints != null) allTrackPoints.AddRange(trackPoints);
        
        return tsgt;
    }

    #region Functions for adding the base points to the track object
    TrackPoint[] GenerateCurvePointsTowardsRight (Vector3 startLoc, Vector3 startDir, int numOfPoints, float distBetweenPoints, AnimationCurve trackCurve )
    {
        TrackPoint[] trackPoints = new TrackPoint[numOfPoints];
        Vector3 dircurve = new Vector3(0.01f,0,trackCurve.Evaluate(0.01f)).normalized; //Just sample the initial bit of the curve

        float angle = Vector3.Angle (dircurve, startDir);
        Vector3.Cross (dircurve, startDir);

        Matrix4x4 g = Matrix4x4.TRS (startLoc,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (dircurve, startDir)),
                                     new Vector3 (distBetweenPoints, distBetweenPoints, distBetweenPoints));

        float interpolationPoints = numOfPoints;
        for (int i=0; i <numOfPoints; i++) {
            
            /*
             * Can multiply to use a portion of curve
            float x = (float)(i)/(float)(numOfPoints) * TurnRight;
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/(float)(numOfPoints));
            */
            
            float x = (float)(i)/interpolationPoints;
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/interpolationPoints);

            trackPoints[i] = new TrackPoint();
            trackPoints[i].position = 
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z));
        }
        
        return trackPoints;//new List<Vector3>(vecArray);
    }

    TrackPoint[] GenerateCurvePointsTowardsLeft (Vector3 startLoc, Vector3 startDir, int numOfPoints, float distBetweenPoints, AnimationCurve trackCurve )
    {
        TrackPoint[] trackPoints = new TrackPoint[numOfPoints];
        Vector3 dircurve = new Vector3(-0.01f,0,trackCurve.Evaluate(0.01f)).normalized; //Just sample the initial bit of the curve

        float angle = Vector3.Angle (dircurve, startDir);
        Vector3.Cross (dircurve, startDir);

        //Matrix by which to rotate piece by
        Matrix4x4 g = Matrix4x4.TRS (startLoc,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (dircurve, startDir)),
                                     new Vector3 (distBetweenPoints, distBetweenPoints, distBetweenPoints));

        float interpolationPoints = numOfPoints;
        for (int i=0; i <numOfPoints; i++) {

            /*
             * Can multiply to use a portion of curve
            float x = (float)(i)/(float)(numOfPoints) * TurnRight;
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/(float)(numOfPoints));
            */
            
            float x = -(float)(i)/interpolationPoints;
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/interpolationPoints);

            trackPoints[i] = new TrackPoint();
            trackPoints[i].position =
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z));
        }
        
        return trackPoints;

    }

    TrackPoint[] GenerateStraight (Vector3 startLoc, Vector3 startDir, int numOfPoints, float distBetweenPoints)
    {
        Vector3[] vecArray = new Vector3[numOfPoints]; 
        TrackPoint[] trackPoints = new TrackPoint[numOfPoints];

        float angle = Vector3.Angle (Vector3.forward, startDir);
        Vector3.Cross (Vector3.forward, startDir);
        
        //Matrix by which to rotate piece by
        Matrix4x4 g = Matrix4x4.TRS (startLoc,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
                                     new Vector3 (distBetweenPoints, distBetweenPoints, distBetweenPoints));
        
        for (int i=0; i <numOfPoints; i++) {
            float x = 0;
            float y = startDir.y * i;
            float z = i;
            
            vecArray [i] =
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z) );

            trackPoints[i] = new TrackPoint();
            trackPoints[i].position = vecArray [i];
        }
        
        return trackPoints;
        
    }

    /// <summary>
    /// Lowers all the points in the array by a random amount bounded by the provided ranges.
    /// </summary>
    /// <param name="pointlist">Pointlist.</param>
    /// <param name="dropLowerRange">Drop lower range.</param>
    /// <param name="dropUpperRange">Drop upper range.</param>
    void DropPointsOnArray (List<Vector3> pointlist, float dropLowerRange, float dropUpperRange,  float initDropAmount = 0)
    {
        float dropAmount = 0;
        for (int i =0; i <pointlist.Count; i++) {
            dropAmount += Random.Range (-dropLowerRange, -dropUpperRange);
            
            pointlist [i] += new Vector3 (0, -initDropAmount + dropAmount, 0); //DropPointOnArray(pointlist[i],i,0.1f); 
        }
    }
    
    #endregion

    public Material GetVariedTrackMatToUse ()
    {
        switch (trackMatType)
        {
            case TrackMatType.STRAIGHT:
                return fwdTrackMat;
                break;

            case TrackMatType.LEFT:
                return leftTrackMat;
                break;

            case TrackMatType.RIGHT:
                return rightTrackMat;
                break;

            case TrackMatType.BTM:
            default:
                return btmTrackMat;
                break;

        }
    }

    public Material GetBottomTrackMatToUse ()
    {
        return btmTrackMat;
    }

    public Material GetBorderMatToUse ()
    {
        return borderMat;
    }

    #region Functions for creating the ground Quad and buildings
    
    GameObject ParseTrackBoundsAndCreateQuad ()
    {
        GameObject.Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube), UpperBounds, Quaternion.identity);
        GameObject.Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube), LowerBounds, Quaternion.identity);
        
        Vector3 g = (UpperBounds + LowerBounds) / 2;
        g.y = LowerBounds.y;
        
        float width = UpperBounds.x - LowerBounds.x;
        float height = UpperBounds.z - LowerBounds.z;
        
        Vector2 textureScale = new Vector2 (width / Mathf.Min (width, height), height / Mathf.Min (width, height));
        
        Vector3[] vertices = new Vector3[]
        {
            new Vector3 (UpperBounds.x, g.y, UpperBounds.z),
            new Vector3 (UpperBounds.x, g.y, LowerBounds.z),
            new Vector3 (LowerBounds.x, g.y, UpperBounds.z),
            new Vector3 (LowerBounds.x, g.y, LowerBounds.z),
        };
        
        return GetMeshWithTexture (vertices, textureScale);
    }
    
    /// <summary>
    /// Creates a mesh and binds a texture to it with texturescale
    /// </summary>
    /// <param name="vertices">Vertices.</param>
    /// <param name="textureScale">Texture scale.</param>
    GameObject GetMeshWithTexture (Vector3[] vertices, Vector2 textureScale)
    {  
        // Create object
        Mesh _m1 = CreateMeshFromVertices (vertices);
        var item = (GameObject)new GameObject (
            "GeneratedGround", 
            typeof(MeshRenderer), // Required to render
            typeof(MeshFilter)    // Required to have a mesh
            );
        
        item.GetComponent<MeshFilter> ().mesh = _m1;
        item.GetComponent<MeshRenderer> ().material = (groundMat);
        
        item.GetComponent<MeshRenderer> ().material.SetTextureScale ("_MainTex", textureScale);

        return item;
    }
    
    // Create a quad mesh
    Mesh CreateMeshFromVertices (Vector3[] vertices)
    {
        
        Mesh mesh = new Mesh ();
        
        Vector2[] uv = new Vector2[]
        {
            new Vector2 (1, 1),
            new Vector2 (1, 0),
            new Vector2 (0, 1),
            new Vector2 (0, 0),
        };
        
        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3,
        };
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals ();
        
        return mesh;
    }
    

    public void PopulateEnvironment( GameObject ParentObj)
    {
        ParseTrackBoundsAndCreateQuad ().transform.parent = ParentObj.transform;

        for(int i =0; i<numOfBuildingsToGenerate; i++)
        {
            GameObject temp = SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[Random.Range(0,5)]);

            if(temp)
                temp.transform.parent = ParentObj.transform;
        }
    }

    public void PopulateBoosts( GameObject Boost)
    {

        for (int i=0; i <this.listOfTrackPoints.Count; i++)
        {
            if(Random.Range(0, 10) < 1)
            {
                Instantiate(Boost, listOfTrackPoints[i].position,Quaternion.identity);
            }
        }
       
    }

    GameObject SphereCastWithinBoundaryForRoom (Vector3 UpperBounds, Vector3 LowerBounds, float sphereRadius, GameObject obj)
    {
        Vector3 topCent = (UpperBounds + LowerBounds) / 2;
        topCent.y = UpperBounds.y;
        
        Vector3 btmCent = (UpperBounds + LowerBounds) / 2;
        btmCent.y = LowerBounds.y;
        
//        while (true) 
        {
            float xTestLoc = Random.Range(LowerBounds.x,UpperBounds.x);
            float zTestLoc = Random.Range(LowerBounds.z,UpperBounds.z);
            
            Vector3 rayStart = new Vector3(xTestLoc,UpperBounds.y + sphereRadius*2,zTestLoc);   //want to start below the ground for accuracy, spherecast ignores objects it starts in
            Ray ray = new Ray (rayStart, Vector3.down);   //We start the spherecast someway above the upperbounds, and cast downwards
            
            RaycastHit rch;

            if (
                !Physics.SphereCast (ray,sphereRadius,out rch,(UpperBounds.y - LowerBounds.y) + sphereRadius*2)
                ) 
            {
                //Debug.Log("have place to put");
                Debug.DrawRay (rayStart, Vector3.down*100, Color.cyan, 5);
               
                return Instantiate (obj, new Vector3(rayStart.x,LowerBounds.y,rayStart.z), obj.transform.rotation) as GameObject;
                
//                g.transform.localScale = new Vector3(
//                    
//                    g.transform.localScale.x*Random.Range(1,1.5f),
//                    g.transform.localScale.y*Random.Range(1,1.5f),
//                    g.transform.localScale.z*Random.Range(1,1.5f));
            } 

            return null;
        }
    }
    
    #endregion
    

    void DrawCross(Vector3 pos, Color color = default(Color), float dist = 10, float duration = 5)
    {
        Debug.DrawLine(pos - Vector3.right * dist, pos + Vector3.right * dist, color, duration);
        Debug.DrawLine(pos - Vector3.forward * dist, pos + Vector3.forward * dist, color, duration);
    }

    #region OldDrawCircle
    List<Vector3> GenerateLeftCurvePoints (Vector3 startLoc, Vector3 startDir, int numOfPoints, float distBetweenPoints, float portionOfCircle = 1)
    {
        Vector3[] vecArray = new Vector3[numOfPoints];
        
        float angle = Vector3.Angle (Vector3.forward, startDir);
        Vector3.Cross (Vector3.forward, startDir);
        
        //Matrix by which to rotate piece by
        Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
                                     new Vector3 (1, 1, 1));
        
        float sign = 1;
        
        for (int i=0; i <numOfPoints; i++) {
            float x = (-sign) * Mathf.Cos (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180) + sign;//requires the float so the parameter multiplication works
            float y = startDir.y * i;
            float z = Mathf.Sin (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180);//requires the float so the parameter multiplication works
            
            vecArray [i] =
                startLoc + 
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z) * 
                        distBetweenPoints);
        }
        
        return new List<Vector3>(vecArray);
    }
    
    List<Vector3> GenerateRightCurvePoints (Vector3 startLoc, Vector3 startDir, int numOfPoints, float distBetweenPoints, float portionOfCircle = 1)
    {
        Vector3[] vecArray = new Vector3[numOfPoints];
        
        float angle = Vector3.Angle (Vector3.forward, startDir);
        Vector3.Cross (Vector3.forward, startDir);
        
        //Matrix by which to rotate piece by
        Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
                                     new Vector3 (1, 1, 1));
        
        float sign = -1;
        
        for (int i=0; i <numOfPoints; i++) {
            float x = (-sign) * Mathf.Cos (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180) + sign;//requires the float so the parameter multiplication works
            float y = startDir.y * i;
            float z = Mathf.Sin (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180);//requires the float so the parameter multiplication works
            
            vecArray [i] =
                startLoc + 
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z) * 
                        distBetweenPoints);
        }
        
        return new List<Vector3>(vecArray);
    }
    #endregion
}
