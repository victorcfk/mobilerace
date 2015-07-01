﻿using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum TrackSegmentType
{
    STRAIGHT,
    LEFT,
    RIGHT
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

    [Range (0,50)]
    public int numOfTrackSegments = 20;
    [Range (0,50)]
    public int trackInterval = 24; //must be even
    [Range (0,50)]
    public float crownAngle = -4;

    [HideInInspector]
    public TrackBuildRTrack track;

    [Space (10)]
    [SerializeField]
    public List<TrackSegment> trackSegs;

    List<Vector3> generatedPointList    = new List<Vector3> ();
    public List<float> StraightLeftRight       = new List<float> ();

    Vector3 UpperBounds;
    Vector3 LowerBounds;

    public int Mat;

    public void InitializeTrackPoints (TrackBuildRTrack track)
    {
        //===================================================
        //Decide straight or curved
        //===================================================
        
        int straightleftright = 2;
        int straightTrackUpperLimit = 4;

        trackSegs = new List<TrackSegment>();

        Vector3 lastDirOnGeneratedTrackSements = Vector3.forward;
        Vector3 lastPointOnGeneratedTrackSegments = Vector3.zero;
        for (int j =0; j <numOfTrackSegments; j++) 
        {
            straightleftright = Random.Range (0, straightTrackUpperLimit);

            if(j==0)    straightleftright =2;
            
            if (straightleftright == 0) 
            {
                straightTrackUpperLimit = 4;

                trackSegs.Add(
                    GenerateRightCurveTrackSegment(lastPointOnGeneratedTrackSegments,lastDirOnGeneratedTrackSements,trackInterval,generatedPointList));


            }
            
            if (straightleftright == 1) 
            {
                straightTrackUpperLimit = 4;

                trackSegs.Add(
                    GenerateLeftCurveTrackSegment(lastPointOnGeneratedTrackSegments,lastDirOnGeneratedTrackSements,trackInterval,generatedPointList));


            }
            
            if (straightleftright >= 2) 
            {
                straightTrackUpperLimit = Mathf.Clamp(straightTrackUpperLimit-1,2,4);

                trackSegs.Add(
                    GenerateStraightTrackSegment(lastPointOnGeneratedTrackSegments,lastDirOnGeneratedTrackSements,trackInterval,generatedPointList));


            }
            
            lastPointOnGeneratedTrackSegments = generatedPointList [generatedPointList.Count - 1];//current last point
            lastDirOnGeneratedTrackSements = (generatedPointList [generatedPointList.Count - 1] - 
                                                generatedPointList [generatedPointList.Count - 2]).normalized;
            
            Debug.DrawRay (lastPointOnGeneratedTrackSegments, lastDirOnGeneratedTrackSements * 50, Color.white, 5);
            Debug.DrawRay (lastPointOnGeneratedTrackSegments, Vector3.up * 50, Color.red, 5);
            
        }

        //===================================================
        DropPointsOnArray (generatedPointList, 0.6f, 0.6f);
        //===================================================

        //Rotation bits
        //=======================================================
        lastDirOnGeneratedTrackSements = Vector3.forward;
        lastPointOnGeneratedTrackSegments = Vector3.zero;
        int temp=0;
        for (int i =0; i <trackSegs.Count; i++)
        {   
            TrackSegment CurrTrackSeg = trackSegs[i];
            List<Vector3> CurrTrackSegTrackpts = CurrTrackSeg.trackPointsPos;
           
            DropPointsOnArray (CurrTrackSegTrackpts, 0.6f, 0.6f,temp*0.6f);

            temp+=CurrTrackSegTrackpts.Count;
            for (int j =0; j <CurrTrackSegTrackpts.Count; j+=4)
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

                //We get the forward control point based on the tiny i intervals for direction
                //=======================================================
                if (j < CurrTrackSegTrackpts.Count - 1) {
                    bp.forwardControlPoint = CurrTrackSegTrackpts [j + 1];
                } else {
                    bp.forwardControlPoint = (CurrTrackSegTrackpts [j] - CurrTrackSegTrackpts [j - 1]) + CurrTrackSegTrackpts [j];
                }
                //=======================================================


                if (CurrTrackSeg.type == TrackSegmentType.LEFT) {
                    
                    //=============================================
                    float angle;
                    Vector3 axis;
                    bp.trackUpQ.ToAngleAxis (out angle, out axis);

                    bp.trackUpQ = Quaternion.AngleAxis (
                        30, 
                        axis) * bp.trackUpQ;

                    bp.type = TrackSegmentType.LEFT;
                    //                    float multi;
                    //                    if (axis.y < 0) multi = -1;
//                    else            multi = 1;

//                    if (j < CurrTrackSegTrackpts.Count/2) { //left turn
//                        //bp.trackUpQ = Quaternion.AngleAxis (angle + StraightLeftRight [j] * multi * 90f, axis);
//                        //bp.position += new Vector3 (0, StraightLeftRight [j] * 35f, 0);
//                        bp.trackUpQ = Quaternion.AngleAxis (
//                                                            Mathf.Lerp(0,90,j/(CurrTrackSegTrackpts.Count/2)), 
//                                                            axis) * bp.trackUpQ;
//
//                        Debug.DrawRay (bp.position, axis * 10, Color.green, 5);
//                    } else {
//                        //bp.trackUpQ = Quaternion.AngleAxis (angle + (1 - StraightLeftRight [j]) * multi * 90f, axis);
//                        // bp.position += new Vector3 (0, (1 - StraightLeftRight [j]) * 35f, 0);
//
//                        bp.trackUpQ = Quaternion.AngleAxis (
//                                                            Mathf.Lerp(90,0,(j- CurrTrackSegTrackpts.Count/2)/(CurrTrackSegTrackpts.Count/2)), 
//                                                            axis) * bp.trackUpQ;
//                        Debug.DrawRay (bp.position, axis * 10, Color.green, 5);
//                    }
                    //=============================================
                } else
                if (CurrTrackSeg.type == TrackSegmentType.RIGHT) { //right turn

                    //=============================================
                    float angle;
                    Vector3 axis;
                    bp.trackUpQ.ToAngleAxis (out angle, out axis);

                    bp.trackUpQ = Quaternion.AngleAxis (
                        -30, 
                        axis) * bp.trackUpQ;

                    bp.type = TrackSegmentType.RIGHT;
//                    if (j < CurrTrackSegTrackpts.Count/2) { //left turn
//                        //bp.trackUpQ = Quaternion.AngleAxis (angle + StraightLeftRight [j] * multi * 90f, axis);
//                        //bp.position += new Vector3 (0, StraightLeftRight [j] * 35f, 0);
//                        bp.trackUpQ = Quaternion.AngleAxis (
//                            -Mathf.Lerp(0,90,j/(CurrTrackSegTrackpts.Count/2)), 
//                            axis) * bp.trackUpQ;
//                        
//                        Debug.DrawRay (bp.position, axis * 10, Color.green, 5);
//                    } else {
//                        //bp.trackUpQ = Quaternion.AngleAxis (angle + (1 - StraightLeftRight [j]) * multi * 90f, axis);
//                        // bp.position += new Vector3 (0, (1 - StraightLeftRight [j]) * 35f, 0);
//                        
//                        bp.trackUpQ = Quaternion.AngleAxis (
//                            -Mathf.Lerp(90,0,(j- CurrTrackSegTrackpts.Count/2)/(CurrTrackSegTrackpts.Count/2)), 
//                            axis) * bp.trackUpQ;
//                        Debug.DrawRay (bp.position, axis * 10, Color.green, 5);
//                    }
                    //=============================================
                } 
                else
                {
                    bp.type = TrackSegmentType.STRAIGHT;
                }

                track.AddPoint (bp);
            }
        }
        
        track.meshResolution = 10;
        track.loop = false; 
        track.includeColliderRoof =false;
        track.trackBumpers = false;
        track.trackColliderWallHeight = 20;

        this.track = track;
    }

    TrackSegment GenerateLeftCurveTrackSegment(Vector3 lastPointAtInterval, Vector3 dirAtEnd, int trackInterval, List<Vector3> generatedPointList)
    {
        List<Vector3> vec3points = GenerateCurve (lastPointAtInterval, dirAtEnd, trackInterval, false, Random.Range (480, 480), Random.Range (0.25f, 0.75f));
        
        TrackSegment tsgt = new TrackSegment();
        tsgt.trackPointsPos = vec3points;
        tsgt.type = TrackSegmentType.LEFT;

        generatedPointList.AddRange (vec3points);

        return tsgt;
    }

    TrackSegment GenerateRightCurveTrackSegment(Vector3 lastPointAtInterval, Vector3 dirAtEnd, int trackInterval, List<Vector3> generatedPointList)
    {
        List<Vector3> vec3points = GenerateCurve (lastPointAtInterval, dirAtEnd, trackInterval, true, Random.Range (480, 480), Random.Range (0.25f, 0.75f));
        
        TrackSegment tsgt = new TrackSegment();
        tsgt.trackPointsPos = vec3points;
        tsgt.type = TrackSegmentType.RIGHT;

        generatedPointList.AddRange (vec3points);

        return tsgt;
    }

    TrackSegment GenerateStraightTrackSegment(Vector3 lastPointAtInterval, Vector3 dirAtEnd, int trackInterval, List<Vector3> generatedPointList)
    {
        List<Vector3> vec3points = GenerateStraight (lastPointAtInterval, dirAtEnd, trackInterval/4, Random.Range (20, 20));
        
        TrackSegment tsgt = new TrackSegment();
        tsgt.trackPointsPos = vec3points;
        tsgt.type = TrackSegmentType.STRAIGHT;
        
        generatedPointList.AddRange (vec3points);

        return tsgt;
    }

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
    
    public Material GetTrackMatToUse ()
    {
        return trackMat;
    }
    
    public Material GetBorderMatToUse ()
    {
        return borderMat;
    }
    
    #region Functions for adding the base points to the track object
    List<Vector3> GenerateStraight (Vector3 startLoc, Vector3 startDir, int numOfPoints, float intervalBtwnPts)
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
                        intervalBtwnPts);
            
            StraightLeftRight.Add (0);
        }
        
        return new List<Vector3>(vecArray);
        
    }

    List<Vector3> GenerateCurve (Vector3 startLoc, Vector3 startDir, int numOfPoints, bool isRight, float intervalBtwnPts, float portionOfCircle = 1)
    {
        Vector3[] vecArray = new Vector3[numOfPoints];
        
        float angle = Vector3.Angle (Vector3.forward, startDir);
        Vector3.Cross (Vector3.forward, startDir);
        
        //Matrix by which to rotate piece by
        Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
                                     Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
                                     new Vector3 (1, 1, 1));
        
        portionOfCircle = Mathf.Clamp (portionOfCircle, 0.1f, 1);

        float sign;
        if (isRight)
            sign = 1;
        else
            sign = -1;

        for (int i=0; i <numOfPoints; i++) {
            float x = (-sign) * Mathf.Cos (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180) + sign;//requires the float so the parameter multiplication works
            float y = startDir.y * i;
            float z = Mathf.Sin (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180);//requires the float so the parameter multiplication works
            
            vecArray [i] =
                startLoc + 
                    g.MultiplyPoint3x4 (
                        new Vector3 (x, y, z) * 
                        intervalBtwnPts);
            
            StraightLeftRight.Add ( (-sign) / (float)numOfPoints);
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
    
    #region Functions for creating the ground Quad and buildings
    
    GameObject ParseTrackBoundsAndCreateQuad (List<Vector3> pointlist)
    {
        UpperBounds = Vector3.zero;
        LowerBounds = Vector3.zero;
        
        for (int i =0; i <pointlist.Count; i++) {
            UpperBounds.x = Mathf.Max (UpperBounds.x, pointlist [i].x);
            UpperBounds.y = Mathf.Max (UpperBounds.y, pointlist [i].y);
            UpperBounds.z = Mathf.Max (UpperBounds.z, pointlist [i].z);
            
            LowerBounds.x = Mathf.Min (LowerBounds.x, pointlist [i].x);
            LowerBounds.y = Mathf.Min (LowerBounds.y, pointlist [i].y);
            LowerBounds.z = Mathf.Min (LowerBounds.z, pointlist [i].z);
        }

        //Expand the bounds
        //================================
        UpperBounds += Vector3.one *50;
        LowerBounds -= Vector3.one *50;
        //================================

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
        ParseTrackBoundsAndCreateQuad (generatedPointList).transform.parent = ParentObj.transform;

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
    

}
