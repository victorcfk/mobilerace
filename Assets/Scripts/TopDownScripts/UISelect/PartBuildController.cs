using UnityEngine;
using System.Collections.Generic;

public class PartBuildController : MonoBehaviour {

    //public ShipCoreInfoStore ShipCore;
    public List<BasicShipPart> PossibleParts {get {return ShipCoreInfoStore.instance.listOfPartPrefabs;}}   //The number of parts that can take up this point
    public BasicShipPart CurrentPart;
    protected Quaternion CurrentPartRotation;

    protected int currPartPtr = 0;
    protected RaycastHit hitInfo;

    public PartInfo partInfo;

    public float scrollSpeed = 0.1f;
    protected float offset;
    protected WeaponBasic weaponComponent;
    protected ShieldBasic shieldComponent;

	// Use this for initialization
	void Start () {
        CurrentPartRotation = this.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

        offset = Time.time * scrollSpeed;
		renderer.material.SetTextureOffset ("_MainTex", new Vector2(offset,0));

        //left clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider == this.collider)
            {
                GetNextPart();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider == this.collider)
            {
                RotateCurrentPart();
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider == this.collider)
            {
                WeaponBasic temp = CurrentPart.GetComponent<WeaponBasic>();

                if(temp)
                {

                    print(name + " A pressed");

                    if(temp.BelongsToWeaponGroup[0])
                        temp.BelongsToWeaponGroup[0] = false;
                    else
                        temp.BelongsToWeaponGroup[0] = true;

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider == this.collider)
            {
                WeaponBasic temp = CurrentPart.GetComponent<WeaponBasic>();

                if(temp)
                {
                    print(name + " B pressed");

                    if(temp.BelongsToWeaponGroup[1])
                        temp.BelongsToWeaponGroup[1] = false;
                    else
                        temp.BelongsToWeaponGroup[1] = true;

                }
            }
        }
	}
    
    public void GetNextPart()
    {
        //No part present, create one
        if (CurrentPart == null)
        {            
            AssignPart(
                Instantiate
                       (PossibleParts[currPartPtr], 
                        this.transform.position, 
                        this.transform.rotation) as BasicShipPart);
        }
        else
        {
            Destroy(CurrentPart.gameObject);
            
            if (currPartPtr < PossibleParts.Count - 1)
                currPartPtr++;
            else
                currPartPtr = 0;

            AssignPart(
                    Instantiate
                    (PossibleParts[currPartPtr],
                     this.transform.position,
                     CurrentPartRotation) as BasicShipPart);
        }
        
    }

    public void AssignPart(BasicShipPart part)
    {
        CurrentPart = part;

        weaponComponent = part.GetComponent<WeaponBasic>();
        shieldComponent = part.GetComponent<ShieldBasic>();

        if(!weaponComponent && !shieldComponent)
            part.transform.rotation = this.transform.localRotation;
        
        part.transform.parent = this.transform.parent;
    }

    public void RotateCurrentPart()
    {
        if (CurrentPart != null && (weaponComponent|| shieldComponent))
        {
            CurrentPart.transform.Rotate(0, 30, 0);
            CurrentPartRotation = CurrentPart.transform.rotation;
        }
    }
}
