using UnityEngine;
using System.Collections;

public enum ShipPartType
{
    CORE,
    ENGINE,
    SHIELD,
    MACHINE_GUN,
    SHOT_GUN,
    BURST_GUN,
    LASER
}

[System.Serializable]
public class PartInfo {

    public ShipPartType partType;

    public Vector3 partLocalPosition;
    public Quaternion partLocalRotation;

    //public BasicShipPart shipPartInst;
    public PartBuildController partBuildControllerInst;

    public PartInfo(BasicShipPart shipPart)
    {
        savePart(shipPart);
    }

    /// <summary>
    /// We create a part from the stored data.
    /// </summary>
    /// <returns>The part.</returns>
    /// <param name="parentShip">If there is a Parent ship, attach the created part as well.</param>
    /// <param name="createController">If set to <c>true</c> create ctrler as well.</param>
    public BasicShipPart loadPart(GameObject parentShip = null,bool createController = false)
    {
        BasicShipPart tempPart = null;

        for (int j = 0; j < ShipCoreInfoStore.instance.listOfPartPrefabs.Count; j++)
        {
            if (ShipCoreInfoStore.instance.listOfPartPrefabs[j].partType == this.partType)
            {
                
                //=========================================================
                
                if(parentShip ==null)
                {
                    tempPart = MonoBehaviour.Instantiate(ShipCoreInfoStore.instance.listOfPartPrefabs[j],partLocalPosition,partLocalRotation) as BasicShipPart;    //Create a part
                    //shipPartInst = tempPart;
                }
                else
                {
                    tempPart = MonoBehaviour.Instantiate(ShipCoreInfoStore.instance.listOfPartPrefabs[j]) as BasicShipPart;    //Create a part
                    
                    tempPart.transform.parent = parentShip.transform;
                    tempPart.transform.localPosition = partLocalPosition;
                    tempPart.transform.localRotation = partLocalRotation;
                    //shipPartInst = tempPart;
                }

                //=========================================================

                //Create a corresponding controller as well?
                if(createController)
                {
                    if(parentShip == null)
                    {
                        partBuildControllerInst = 
                            MonoBehaviour.Instantiate(
                                ShipCoreInfoStore.instance.PieceControlPrefab,
                                partLocalPosition,
                                ShipCoreInfoStore.instance.PieceControlPrefab.transform.rotation) as PartBuildController;//Create a part
                    }
                    else
                    {
                        partBuildControllerInst = 
                            MonoBehaviour.Instantiate(ShipCoreInfoStore.instance.PieceControlPrefab) as PartBuildController;//Create a part
                        
                        partBuildControllerInst.transform.parent = parentShip.transform;
                        partBuildControllerInst.transform.localPosition = partLocalPosition;
                        partBuildControllerInst.transform.localRotation = Quaternion.identity;

                    }
                    
                    partBuildControllerInst.partInfo = this;
                    partBuildControllerInst.AssignPart(tempPart);
                    
                }

                //=========================================================

            }
        }

        return tempPart;
    }

    public void savePart(BasicShipPart shipPart)
    {
        partType = (shipPart.partType);    //save the type on record
     
        partLocalPosition = (shipPart.transform.localPosition);
        partLocalRotation = (shipPart.transform.localRotation);  //save the transform for later.
    }

    public void savePart(PartBuildController pieceCntrlPart)
    {
        partType = pieceCntrlPart.CurrentPart.partType;
            
        partLocalPosition = (pieceCntrlPart.CurrentPart.transform.localPosition);
        partLocalRotation = (pieceCntrlPart.CurrentPart.transform.localRotation);  //save the transform for later.
    }


}
