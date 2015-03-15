using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

    private static ObjectManager _instance;

    public GameObject objHolder;
    //public Texture2D skyboxMat;

    Component holder;
    //This is the public reference that other classes will use
    public static ObjectManager instance
    {
        get
        {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<ObjectManager>();
            
            return _instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        ObjectManager._instance = this;
        
    }

	// Use this for initialization
	void Start () {

        if (objHolder == null)
            objHolder = new GameObject("ObjectHolder");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        //T holder;

        holder = (T)GameObject.Instantiate(prefab, position, rotation);
        holder.transform.parent = objHolder.transform;
        return (T)holder;
    }

    public void Recycle(GameObject des)
    {
        Destroy(des);
    }

}
