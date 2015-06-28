// Track BuildR
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://support.jasperstocker.com
// For support contact email@jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Xml;
#endif
using UnityEngine;

public class TrackBuildRuntime : MonoBehaviour
{
	public static float VERSION_NUMBER = 1.27f;
	public float version = VERSION_NUMBER;
	public TrackBuildRTrack track;
	public TrackBuildRGenerator generator;
	public GameObject trackEditorPreview = null;
	
	//CUSTOM EDITOR VALUES
	public enum modes
	{
		track,
		boundary,
		bumpers,
		textures,
		terrain,
		stunt,
		diagram,
		options,
		export
	}
	
	public enum pointModes
	{
		transform,
		controlpoint,
		trackup,
		trackpoint,
		add,
		remove
	}
	
	public enum boundaryModes
	{
		transform,
		controlpoint
	}
	
	public enum textureModes
	{
		track,
		boundary,
		offroad,
		bumpers
	}
	
	public enum terrainModes
	{
		mergeTerrain,
		conformTrack
	}
	
	public enum stuntModes
	{
		loop,
		jump,
		//twist, TODO: twist creation
		jumptwist
	}
	
	public modes mode = modes.track;
	public pointModes pointMode = pointModes.transform;
	public boundaryModes boundaryMode = boundaryModes.transform;
	public textureModes textureMode = textureModes.track;
	public stuntModes stuntMode = stuntModes.loop;
	public terrainModes terrainMode = terrainModes.mergeTerrain;
	
	//export
	public enum fileTypes
	{
		Fbx,
		Obj
	}
	public fileTypes fileType = fileTypes.Fbx;
	public string exportFilename = "exportedTrack";
	public bool copyTexturesIntoExportFolder = true;
	public bool exportCollider = true;
	public bool createPrefabOnExport = true;
	public bool includeTangents = false;
	
	//preview
	public float previewPercentage = 0;
	public bool previewForward = true;
	public float previewStartPoint = 0.0f;
	
	public bool tangentsGenerated {get {return track.tangentsGenerated;}}
	public bool lightmapGenerated {get {return track.lightmapGenerated;}}
	public bool optimised {get {return track.optimised;}}


	public Material useMaterial;


	public void Init()
	{
		track = gameObject.AddComponent<TrackBuildRTrack>();
		track.InitTextures();
		track.baseTransform = transform;
		
        TrackManager.instance.InitializeTrackPoints(track);

		generator = gameObject.AddComponent<TrackBuildRGenerator>();
		
		ForceFullRecalculation();
		
		track.diagramMesh = new Mesh();
		track.diagramMesh.vertices = new [] { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1)};
		track.diagramMesh.uv = new [] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0,1), new Vector2(1,1)};
		track.diagramMesh.triangles = new []{1,0,2,1,2,3};
		
		track.diagramGO = new GameObject("Diagram");
		track.diagramGO.transform.parent = transform;
		track.diagramGO.transform.localPosition = Vector3.zero;
		track.diagramGO.AddComponent<MeshFilter>().mesh = track.diagramMesh;
		track.diagramMaterial = new Material(Shader.Find("Unlit/Texture"));
		track.diagramGO.AddComponent<MeshRenderer>().material = track.diagramMaterial;
		track.diagramGO.AddComponent<MeshCollider>().sharedMesh = track.diagramMesh;
	}
	
	public void UpdateRender()
	{
		generator.track = track;
		generator.UpdateRender();
		
		foreach(Transform child in GetComponentsInChildren<Transform>())
		{
			child.gameObject.isStatic = gameObject.isStatic;
		}
	}
	
	public void ForceFullRecalculation()
	{
		int numberOfPoints = track.realNumberOfPoints;
		for (int i = 0; i < numberOfPoints; i++)
			track[i].isDirty = true;
		track.RecalculateCurves();
		UpdateRender();
	}
	
	public void GenerateSecondaryUVSet()
	{
		track.GenerateSecondaryUVSet();
	}
	
	public void GenerateTangents()
	{
		track.SolveTangents();
	}
	
	public void OptimizeMeshes()
	{
		track.OptimizeMeshes();
	}
	
	void OnDestroy()
	{
		for(int i=0; i<track.transform.childCount; i++)
		{
			Destroy(track.transform.GetChild(i).gameObject);
		}
		
		track.Clear();
		
	}
	
	void OnDrawGizmos()
	{
		if(track != null)
		{
			int numberOfPoints = track.numberOfCurves;

			if (numberOfPoints < 1)
				return;
		}
	}
	
	public void Clear()
	{
		track.Clear();
	}
	
	//    void OnEnable()
	//    {
	//        if(track != null)
	//            if (track.diagramGO != null)
	//                track.diagramGO.SetActive(!Application.isPlaying&&track.showDiagram);
	//    }
	
	void Awake()
	{
		if (!Application.isPlaying)
		{
			//only update the models when we're editing.
			UpgradeData();
		}
	}
	
	private void UpgradeData()
	{
		float currentVersion = VERSION_NUMBER;
		float dataVersion = version;
		
		if (currentVersion == dataVersion)
		{
			//The data matches the current version of Track Buildr - do nothing.
			return;
		}
		
		if (currentVersion < dataVersion)
		{
			Debug.LogError("Track BuildR v." + currentVersion + ": Great scot! This data is from the future! (version:" + dataVersion + ") - need to avoid contact to ensure the survival of the universe...");
			return;//don't touch ANYTHING!
		}
		
		if (dataVersion < 1.1f)
		{
			#if UNITY_EDITOR
			if(UnityEditor.EditorUtility.DisplayDialog("WARNING", "Track BuildR 1.1 does not directly support upgrading from 1.0.\nContact me at email@jasperstocker.com for instructions on how to do this.\nIt's not hard to upgrade and\nremember to backup your project!\nJasper","Ok, I'll email you"))
				Application.OpenURL("mailto:email@jasperstocker.com");
			#endif
			Debug.LogWarning("Warning - There is no upgrade path to this version - sorry.");
		}
		
		version = currentVersion;
	}
	
	#if UNITY_EDITOR
	/// <summary>
	/// Convert this camera path into an xml string for export
	/// </summary>
	/// <returns>A generated XML string</returns>
	public string ToXML()
	{
		StringBuilder sb = new StringBuilder();
		
		sb.AppendLine("<?xml version='1.0' encoding='ISO-8859-15'?>");
		sb.AppendLine("<!-- Unity3D Asset Track BuildR XML Exporter http://trackbuildr.jasperstocker.com -->");
		sb.AppendLine("<trackbuildr>");
		sb.AppendLine("<version>"+version+"</version>");
		sb.AppendLine("<name>"+name+"</name>");
		sb.AppendLine("<fileType>"+fileType+"</fileType>");
		sb.AppendLine("<exportFilename>"+exportFilename+"</exportFilename>");
		sb.AppendLine("<copyTexturesIntoExportFolder>"+copyTexturesIntoExportFolder+"</copyTexturesIntoExportFolder>");
		sb.AppendLine("<exportCollider>"+exportCollider+"</exportCollider>");
		sb.AppendLine("<createPrefabOnExport>"+createPrefabOnExport+"</createPrefabOnExport>");
		sb.AppendLine("<includeTangents>"+includeTangents+"</includeTangents>");
		
		sb.Append(track.ToXML());
		
		sb.AppendLine("</trackbuildr>");
		
		return sb.ToString();
	}
	
	/// <summary>
	/// Import XML data into this camera path overwriting the current data
	/// </summary>
	/// <param name="XMLPath">An XML file path</param>
	public void FromXML(string XMLPath)
	{
		Debug.Log("Import Track BuildR Track XML " + XMLPath);
		Clear();
		XmlDocument xml = new XmlDocument();
		using (StreamReader sr = new StreamReader(XMLPath))
		{
			xml.LoadXml(sr.ReadToEnd());
		}
		
		XmlNode trackNode = xml.SelectNodes("trackbuildr")[0];
		version = float.Parse(trackNode["version"].FirstChild.Value);
		
		if (trackNode["name"] != null)
			name = trackNode["name"].FirstChild.Value;
		
		if(trackNode["fileType"] != null)
		{
			fileType = (fileTypes)System.Enum.Parse(typeof(fileTypes), trackNode["fileType"].FirstChild.Value);
			exportFilename = trackNode["exportFilename"].FirstChild.Value;
			copyTexturesIntoExportFolder = bool.Parse(trackNode["copyTexturesIntoExportFolder"].FirstChild.Value);
			exportCollider = bool.Parse(trackNode["exportCollider"].FirstChild.Value);
			createPrefabOnExport = bool.Parse(trackNode["createPrefabOnExport"].FirstChild.Value);
			includeTangents = bool.Parse(trackNode["includeTangents"].FirstChild.Value);
		}
		//send data to track
		track.FromXML(trackNode.SelectSingleNode("track"));
	}
	
	/// <summary>
	/// Import XML data into this camera path overwriting the current data
	/// </summary>
	/// <param name="KMLPath">An Google Earth KML file path</param>
	public void FromKML(string KMLPath)
	{
		Debug.Log("Import Google Earth KML " + KMLPath);
		Clear();
		XmlDocument xml = new XmlDocument();
		using (StreamReader sr = new StreamReader(KMLPath))
		{
			xml.LoadXml(sr.ReadToEnd());
		}
		name = xml["kml"]["Document"]["Placemark"]["name"].FirstChild.Value;
		
		int nodes = xml["kml"]["Document"].ChildNodes.Count;
		for(int i = 0; i < nodes; i++)
		{
			XmlNode node = xml["kml"]["Document"].ChildNodes[i];
			if(node.Name == "Placemark")
			{
				if(node["LineString"] != null)
				{
					if(node["LineString"]["coordinates"] != null)
					{
						track.FromKML(node["LineString"]["coordinates"].FirstChild.Value);
						return;
					}
				}
			}
		}
		Debug.LogError("Track BuildR could not convert KML file, contact email@jasperstocker.com and I'll sort this out. Thanks");
	}
	#endif
}
