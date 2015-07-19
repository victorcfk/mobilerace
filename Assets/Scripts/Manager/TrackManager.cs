﻿using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum TrackSegmentType
{
    STRAIGHT,
    LEFT_SEMI,
    RIGHT_SEMI
}

[System.Serializable]
public struct TrackSegment
{
    public int pointCount
    {
        get
        {
            return trackPointsPos.Count;

        }
    }
    public List<Vector3> trackPointsPos;
    public List<Vector3> trackPointsRot;

    public TrackSegmentType type;
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
        trackMat;
    [SerializeField]
    Material
        trackMat2;
    [SerializeField]
    Material
        borderMat;
    [SerializeField]
    Material
        groundMat;

    [Range (0,200)]
    public int numOfTrackSegments = 20;
    [Range (0,200)]
    public int pointsInSegment = 24; //must be even
    [Range (-20,20)]
    public float crownAngle = -4;

    [HideInInspector]
    public TrackBuildRTrack track;

    [Space (10)]
    [SerializeField]
    public List<TrackSegment> trackSegs;

    List<Vector3> GeneratedPointList    = new List<Vector3> ();

    Vector3 UpperBounds;
    Vector3 LowerBounds;

    [Range (0,90)]
    public float MaxLeftTurnCant = 30;
    [Range (0,90)]
    public float MaxRightTurnCant = 30;

    public AnimationCurve GradualCurve;
    public AnimationCurve ExpoCurve;


    public AnimationCurve SemiCircle;

    public Vector2 textureUnitSize = new Vector2(50, 50);

    public int Mat;

    public void InitializeTrackPoints (TrackBuildRTrack track)
    {
        //===================================================
        //Decide straight or curved
        //===================================================
        
        int straightleftright = 2;
        int straightTrackUpperLimit = 4;

        trackSegs = new List<TrackSegment>();

        Vector3 lastDirOnGeneratedTrackSegments = Vector3.forward;
        Vector3 lastPointOnGeneratedTrackSegments = Vector3.zero;
        Vector3 secondLastPointOnGeneratedTrackSegment = Vector3.zero;
              
        for (int j =0; j <numOfTrackSegments; j++) 
        {
            if(j==0)
            {
                straightleftright =2;
            }

            if (straightleftright == 0) 
            {
                straightleftright = 2;
                trackSegs.Add( 
                              GenerateFromCurveTrackSegment(
                    GeneratedPointList,
                    lastPointOnGeneratedTrackSegments,
                    lastDirOnGeneratedTrackSegments,
                    pointsInSegment,
                    Random.Range(300,600),TrackSegmentType.RIGHT_SEMI,getCurveToGenerate()));
            }
            else
            if (straightleftright == 1) 
            {

                straightleftright = 2;
                trackSegs.Add( 
                              GenerateFromCurveTrackSegment(
                    GeneratedPointList,
                    lastPointOnGeneratedTrackSegments,
                    lastDirOnGeneratedTrackSegments,
                    pointsInSegment,
                    Random.Range(300,600),TrackSegmentType.LEFT_SEMI,getCurveToGenerate()));
            }
            else
            if (straightleftright >= 2) 
            {
                straightleftright = Random.Range(0,2);
                trackSegs.Add( 
                              GenerateFromCurveTrackSegment(
                    GeneratedPointList,
                    lastPointOnGeneratedTrackSegments,
                    lastDirOnGeneratedTrackSegments,
                    pointsInSegment,
                    20,TrackSegmentType.STRAIGHT,getCurveToGenerate()));
//                trackSegs.Add( 
//                              GenerateFromCurveTrackSegment(
//                    GeneratedPointList,
//                    lastPointOnGeneratedTrackSegments,
//                    lastDirOnGeneratedTrackSegments,
//                    pointsInSegment,
//                    300,TrackSegmentType.RIGHT));
            }

            TrackSegment lastKnowntrackSeg = trackSegs[trackSegs.Count-1];
            lastPointOnGeneratedTrackSegments = lastKnowntrackSeg.trackPointsPos[lastKnowntrackSeg.trackPointsPos.Count-1];
            secondLastPointOnGeneratedTrackSegment = lastKnowntrackSeg.trackPointsPos[lastKnowntrackSeg.trackPointsPos.Count-2];

//                GeneratedPointList [GeneratedPointList.Count - 1];//current last point
            lastDirOnGeneratedTrackSegments = (lastPointOnGeneratedTrackSegments - 
                                               secondLastPointOnGeneratedTrackSegment).normalized;

            lastPointOnGeneratedTrackSegments += lastDirOnGeneratedTrackSegments * 60;

            Debug.DrawRay (lastPointOnGeneratedTrackSegments, lastDirOnGeneratedTrackSegments * 50, Color.white, 5);
            Debug.DrawRay (lastPointOnGeneratedTrackSegments, Vector3.up * 50, Color.red, 5);
            
        }

        //===================================================
       // DropPointsOnArray (generatedPointList, 0.6f, 0.6f);
        //===================================================

        //float temp = 0;
        //Rotation bits
        //=======================================================
        lastDirOnGeneratedTrackSegments = Vector3.forward;
        lastPointOnGeneratedTrackSegments = Vector3.zero;
        int temp=0;
        for (int i =0; i <trackSegs.Count; i++)
        {   
            TrackSegment CurrTrackSeg = trackSegs[i];
            List<Vector3> CurrTrackSegTrackpts = CurrTrackSeg.trackPointsPos;
           
            DropPointsOnArray (CurrTrackSegTrackpts, 0.75f, 0.75f,temp*0.75f);
            temp+=CurrTrackSegTrackpts.Count;

            for (int j =0 ; j <CurrTrackSegTrackpts.Count; j+=4)
            {
//                Debug.Log("dasdas");
                TrackBuildRPoint bp = track.gameObject.AddComponent<TrackBuildRPoint>();
                
                bp.baseTransform = transform;
                bp.position = CurrTrackSegTrackpts [j];
                bp.width = 100;

                bp.colliderSides = true;
                bp.renderBounds = false;
                bp.generateBumpers = false;
                bp.extrudeTrackBottom = false;
                bp.crownAngle = crownAngle;

                float angle2;
                Vector3 axis2;
                bp.trackUpQ.ToAngleAxis (out angle2, out axis2);
//                Debug.DrawRay(bp.position,axis2.normalized *100,Color.yellow,6);

                //We get the forward control point based on the tiny i intervals for direction
                //=======================================================
                if (j < CurrTrackSegTrackpts.Count - 1) {
                    bp.forwardControlPoint = CurrTrackSegTrackpts [j + 1];
                } else {
                    bp.forwardControlPoint = (CurrTrackSegTrackpts [j] - CurrTrackSegTrackpts [j - 1]) + CurrTrackSegTrackpts [j];
                }

                /*
                 *  if (j == 0 && i> 0) {
                    bp.forwardControlPoint = CurrTrackSegTrackpts [j]-trackSegs[i-1].trackPointsPos[trackSegs.Count-1];
                }
                else
                if (j >= CurrTrackSegTrackpts.Count - 1) {
                    bp.forwardControlPoint = (CurrTrackSegTrackpts [j] - CurrTrackSegTrackpts [j - 1]) + CurrTrackSegTrackpts [j];
                } else {
                    bp.forwardControlPoint = CurrTrackSegTrackpts [j + 1];
                }
                 * */
                //=======================================================

                if (CurrTrackSeg.type == TrackSegmentType.LEFT_SEMI) {
                    
                    //=============================================
                    float angle;
                    Vector3 axis;
                    bp.trackUpQ.ToAngleAxis (out angle, out axis);

                    Debug.DrawRay(bp.position,axis.normalized *100,Color.yellow,6);

                    if(axis.y > 0 )
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Count,0,MaxLeftTurnCant/4), 
                            axis) * bp.trackUpQ;
                    else
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Count,0,-MaxLeftTurnCant/4), 
                            axis) * bp.trackUpQ;

                    bp.position += Vector3.up * GetPosChangeCurveValue(j,CurrTrackSegTrackpts.Count,100,MaxLeftTurnCant);

                    CurrTrackSegTrackpts [j] = bp.position;

                    bp.type = TrackSegmentType.LEFT_SEMI;
                    //=============================================
                } else
                if (CurrTrackSeg.type == TrackSegmentType.RIGHT_SEMI) { //right turn

                    //=============================================
                    float angle;
                    Vector3 axis;
                    bp.trackUpQ.ToAngleAxis (out angle, out axis);

                    Debug.DrawRay(bp.position,axis.normalized *100,Color.yellow,6);

                    if(axis.y > 0 )
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Count,0,-MaxRightTurnCant/4), 
                            axis) * bp.trackUpQ;
                    else
                        bp.trackUpQ = Quaternion.AngleAxis (
                            GetCantAngleCurveValue(j,CurrTrackSegTrackpts.Count,0,MaxRightTurnCant/4), 
                            axis) * bp.trackUpQ;

                    bp.position += Vector3.up * GetPosChangeCurveValue(j,CurrTrackSegTrackpts.Count,100,MaxRightTurnCant);

                    CurrTrackSegTrackpts [j] = bp.position;

                    bp.type = TrackSegmentType.RIGHT_SEMI;
                    //=============================================
                } 
                else
                {
                    bp.type = TrackSegmentType.STRAIGHT;
                }

                UpperBounds.x = Mathf.Max (UpperBounds.x, bp.position.x);
                UpperBounds.y = Mathf.Max (UpperBounds.y, bp.position.y);
                UpperBounds.z = Mathf.Max (UpperBounds.z, bp.position.z);
                    
                LowerBounds.x = Mathf.Min (LowerBounds.x, bp.position.x);
                LowerBounds.y = Mathf.Min (LowerBounds.y, bp.position.y);
                LowerBounds.z = Mathf.Min (LowerBounds.z, bp.position.z);
               
                track.AddPoint (bp);
            }
        }
        
        track.meshResolution = 25;
        //track.Texture(0).textureUnitSize
        track.loop = false; 
        track.includeColliderRoof =false;
        track.trackBumpers = false;
        track.trackColliderWallHeight = 20;

        this.track = track;

        //Expand the bounds
        //================================
        UpperBounds += Vector3.one *50;
        LowerBounds -= Vector3.one *50;
        //================================
    }

    AnimationCurve getCurveToGenerate()
    {
//        //float difficulty
//        if (Random.Range(0, 2) < 1)
//        {
            return SemiCircle;
//        } else
//        {
//
//            return ExpoCurve;
//        }
    }

    float GetCantAngleCurveValue(float point, float totalCurvePointCount, float initialCantAngle, float maxCantAngle, int type = 0)
    {

        return 
            Mathf.Lerp(initialCantAngle, maxCantAngle,
                       GradualCurve.Evaluate(point / totalCurvePointCount));
    }

    float GetPosChangeCurveValue(float point, float totalCurvePointCount, float trackWidth, float angle, int type = 0)
    {
        float x = trackWidth/4;

        if (angle == 0)
            return 0;

        return 
            Mathf.Lerp(0, -x * Mathf.Cos(angle),
                       GradualCurve.Evaluate(point / totalCurvePointCount));
    }

    
    TrackSegment GenerateFromCurveTrackSegment(
        List<Vector3> generatedPointList,
        Vector3 lastPointAtInterval, 
        Vector3 dirAtEnd, 
        int pointsInSegment, 
        float distbetweenPoints, 
        TrackSegmentType type,
        AnimationCurve curveToUse)
    {
        List<Vector3> vec3points;
        if(type == TrackSegmentType.RIGHT_SEMI)
            vec3points = GenerateCurvePointsTowardsRight(lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints,curveToUse);        
        else
            if(type == TrackSegmentType.LEFT_SEMI)
                vec3points = GenerateCurvePointsTowardsLeft(lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints,curveToUse);        
        else
            vec3points = GenerateStraight(lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints);        

        TrackSegment tsgt = new TrackSegment();
        tsgt.trackPointsPos = vec3points;
        if(type == TrackSegmentType.RIGHT_SEMI)
            tsgt.type = TrackSegmentType.RIGHT_SEMI;
        else
            if(type == TrackSegmentType.LEFT_SEMI)
                tsgt.type = TrackSegmentType.LEFT_SEMI;
        else
            tsgt.type = TrackSegmentType.STRAIGHT;


        
        generatedPointList.AddRange (vec3points);
        
        return tsgt;
    }

    TrackSegment GenerateStraightTrackSegment(List<Vector3> generatedPointList, Vector3 lastPointAtInterval, Vector3 dirAtEnd, int pointsInSegment, float distbetweenPoints)
    {
        List<Vector3> vec3points = GenerateStraight (lastPointAtInterval, dirAtEnd, pointsInSegment, distbetweenPoints);
        
        TrackSegment tsgt = new TrackSegment();
        tsgt.trackPointsPos = vec3points;
        tsgt.type = TrackSegmentType.STRAIGHT;
        
        generatedPointList.AddRange (vec3points);
        
        return tsgt;
    }

    List<Vector3> GenerateCurvePointsTowardsRight (Vector3 startLoc, Vector3 startDir, float numOfPoints, float distBetweenPoints, AnimationCurve trackCurve )
    {
        Debug.DrawRay (startLoc, startDir * 100,  Color.cyan,5);
        //Time horzontal
        //value vertical
        Vector3 dircurve = new Vector3(
            2/numOfPoints,
            0,
            trackCurve.Evaluate(2/numOfPoints)
            );
        Debug.Log (dircurve);
        Debug.DrawRay (startLoc, dircurve * 200,  Color.cyan,5);

        Vector3[] vecArray = new Vector3[(int)numOfPoints];
        
        float angle = Vector3.Angle (dircurve, startDir);
        Vector3.Cross (dircurve, startDir);

        Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (dircurve, startDir)),
                                     new Vector3 (1, 1, 1));


        for (int i=0; i <numOfPoints; i++) {
            
            /*
             * Can multiply to use a portion of curve
            float x = (float)(i)/(float)(numOfPoints) * TurnRight;
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/(float)(numOfPoints));
            */
            
            float x = (float)(i)/(float)(numOfPoints);
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/(float)(numOfPoints));
            
            Debug.Log(vecArray [i] + " "+ (float)(i)/(float)(numOfPoints));
            vecArray [i] =
                startLoc + 
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z) * 
                        distBetweenPoints);
            
            //Debug.Log(vecArray [i] + " "+ (float)(i)/(float)(pointsInSegment));
            
            DrawCross(vecArray[i],Color.white);
        }
        
        return new List<Vector3>(vecArray);
    }

    List<Vector3> GenerateCurvePointsTowardsLeft (Vector3 startLoc, Vector3 startDir, float numOfPoints, float distBetweenPoints, AnimationCurve trackCurve )
    {
        Debug.DrawRay (startLoc, startDir * 100,  Color.cyan,5);

        Vector3 dircurve = new Vector3(
            -2/numOfPoints,
            0,
            trackCurve.Evaluate(2/numOfPoints)
            );

        Debug.Log (dircurve);
        Debug.DrawRay (startLoc, dircurve * 200,  Color.cyan,5);

        Vector3[] vecArray = new Vector3[(int)numOfPoints];
        
        float angle = Vector3.Angle (dircurve, startDir);
        Vector3.Cross (dircurve, startDir);
        //Matrix by which to rotate piece by
        Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (dircurve, startDir)),
                                     new Vector3 (1, 1, 1));
        
        for (int i=0; i <numOfPoints; i++) {

            /*
             * Can multiply to use a portion of curve
            float x = (float)(i)/(float)(numOfPoints) * TurnRight;
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/(float)(numOfPoints));
            */
            
            float x = (float)(i)/(float)(numOfPoints) * -1;
            float y = startDir.y * (float)(i);
            float z = trackCurve.Evaluate((float)(i)/(float)(numOfPoints));

            Debug.Log(vecArray [i] + " "+ (float)(i)/(float)(numOfPoints));
            vecArray [i] =
                startLoc + 
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z) * 
                        distBetweenPoints);
            
            //Debug.Log(vecArray [i] + " "+ (float)(i)/(float)(pointsInSegment));
            
            DrawCross(vecArray[i],Color.white);
        }
        
        return new List<Vector3>(vecArray);

    }

    #region Functions for adding the base points to the track object
    List<Vector3> GenerateStraight (Vector3 startLoc, Vector3 startDir, int numOfPoints, float distBetweenPoints)
    {
        Vector3[] vecArray = new Vector3[numOfPoints]; 
        
        float angle = Vector3.Angle (Vector3.forward, startDir);
        Vector3.Cross (Vector3.forward, startDir);
        
        //Matrix by which to rotate piece by
        Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
                                     new Vector3 (1, 1, 1));
        
        for (int i=0; i <numOfPoints; i++) {
            float x = 0;
            float y = startDir.y * i;
            float z = i;
            
            vecArray [i] =
                startLoc + 
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z) * 
                        distBetweenPoints);

//            DrawCross(vecArray[i]);
        }
        
        return new List<Vector3>(vecArray);
        
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
        if(Mat == 0)
        {
            return trackMat;
        }
        else
        {
            return trackMat2;
        }
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
    
    public GameObject[] Buildings;
    public void PopulateEnvironment( GameObject ParentObj)
    {
        ParseTrackBoundsAndCreateQuad ().transform.parent = ParentObj.transform;

        for(int i =0; i<30; i++)
        {
            GameObject temp = SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[Random.Range(0,5)]);

            if(temp)
                temp.transform.parent = ParentObj.transform;
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
    

    void DrawCross(Vector3 pos, Color color)
    {
        Debug.DrawLine(pos - Vector3.right * 10, pos + Vector3.right * 10, color, 5);
        Debug.DrawLine(pos - Vector3.forward * 10, pos + Vector3.forward * 10, color, 5);
    }
}
