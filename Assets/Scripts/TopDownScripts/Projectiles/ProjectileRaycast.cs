using UnityEngine;
using System.Collections;
using System.Threading;

[RequireComponent (typeof(LineRenderer))]

public class ProjectileRaycast : ProjectileBasic {
    
    public float laserWidth = 0.5f;
    public float noise = 1.0f;
    public float maxLength = 20.0f; // WE MIGHT be alittle to high here.. but in camera we are at 19.
    public Color color1 = Color.cyan;
	public Color color2 = Color.cyan;

	// This controls if we are firing or not.
	public bool firing = false;

    public LineRenderer lineRenderer;
    public int length;
    public Vector3[] position;
    //Cache any transforms here
    public Transform myTransform;
    public Transform endEffectTransform;

    //The particle system, in this case sparks which will be created by the Laser
    public ParticleSystem endEffect;
    Vector3 offset;

    // Use this for initialization
    protected virtual new void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(laserWidth, laserWidth);
        myTransform = transform;
        offset = new Vector3(0,0,0);
        endEffect = GetComponentInChildren<ParticleSystem>();

        if(endEffect)
            endEffectTransform = endEffect.transform;

        // Auto Firing
        FireLaser();
        Invoke("DestroySelf", lifeTime);
    }

    // The title says it.. It fires laser
	void FireLaser (){

		firing = true;
	}

	// Stops the lazer from rendering
	void StopLaser (){
        //lineRenderer.SetVertexCount(0);
        DisableLineRenderer();
        firing = false;
	}

    protected virtual new void DestroySelf()
    {
        //Debug.Log("[LaserBeams DestroySelf] Destroy gameObject");
        StopLaser();
        Destroy(gameObject);
    }

    private void DisableLineRenderer()
    {
        lineRenderer.SetVertexCount(0);
    }

    // Update is called once per frame
    protected virtual new void Update()
    {
		if(firing){         
            RenderLaser(); 
        }
        else{
            Invoke("DisableLineRenderer", 0.5f);
        }
    }

    void FixedUpdate(){
       
    }
    

    void RenderLaser(){
		
        //Shoot our laserbeam forwards!
        UpdateLength(); 
		  //lineRenderer.material = new Material( Shader.Find("Mobile/Particles/Additive") );
        lineRenderer.SetColors(color1 ,color2);

        //Move through the Array
        for(int i = 0; i<length; i++){
            //Set the position here to the current location and project it in the forward direction of the object it is attached to
            offset.x = myTransform.position.x+i*myTransform.forward.x+Random.Range(-noise,noise);
            offset.z = i*myTransform.forward.z+Random.Range(-noise,noise)+myTransform.position.z;
            position[i] = offset;
            position[0] = myTransform.position;
            
            lineRenderer.SetPosition(i, position[i]);           
        } 
    }

    

    void UpdateLength(){
        //Raycast from the location of the cube forwards
        RaycastHit[] hit;
        hit = Physics.RaycastAll(myTransform.position, myTransform.forward, maxLength);
        int i = 0;
        while(i < hit.Length){
            //Check to make sure we aren't hitting triggers but colliders
            if(!hit[i].collider.isTrigger && hit[i].transform.tag == "Enemies" || hit[i].transform.name == "EnemyType1")
            {
                length = (int)Mathf.Round(hit[i].distance)+2;
                position = new Vector3[length];
                //Move our End Effect particle system to the hit point and start playing it
                if(endEffect){
                endEffectTransform.position = hit[i].point;

                if(!endEffect.isPlaying)
                    endEffect.Play();
						
                }

                //Debug.Log("[LaserBeams: Collided] : Name: "+ hit[i].collider.name);
                // DAMAGE hostiles in the beam.
                //if(hit[i].transform.tag == "Enemies" || hit[i].transform.name == "EnemyType1")
                
                //GameObject go = hit[i];
                //Debug.Log("[LaserBeams: ProcDamage]");
                //hit[i].collider.GetComponent<EnemyUnits>().Health -= damage;

                lineRenderer.SetVertexCount(length);
                StopLaser();
                break;
            }

            
            i++;
		}

        //If we're not hitting anything, don't play the particle effects
        if(endEffect){
        if(endEffect.isPlaying)
            endEffect.Stop();
        }
			

        length = (int)maxLength;
        position = new Vector3[length];
        lineRenderer.SetVertexCount(length); 
    }
		
		
}