using UnityEngine;
using System.Collections;

// Place the script in the Camera-Control group in the component menu
[AddComponentMenu("Camera-Control/Smooth FollowCS")]
public class SmoothFollowCS : MonoBehaviour {

	/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/
	
	// The target we are following
	public Transform camFollowTarget;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;

    [SeparatorAttribute]

    [ReadOnlyAttribute]
    public float MaxRotationAngle = 90;

    [ReadOnlyAttribute]
    public float MaxClampHeight = 20;
    [ReadOnlyAttribute]
    public float MinClampHeight = 0;

    float temp = 0;
    float temp2 = 0;

	void LateUpdate () {
		// Early out if we don't have a target
		if (!camFollowTarget)
			return;
		
		// Calculate the current rotation angles
		var wantedRotationAngle = camFollowTarget.eulerAngles.y;
		var wantedHeight = camFollowTarget.position.y + height;
		
		var currentRotationAngle = transform.eulerAngles.y;
		var currentHeight = transform.position.y;
		
		// Damp the rotation around the y-axis
        //currentRotationAngle = Mathf.SmoothDampAngle (currentRotationAngle, wantedRotationAngle, ref temp2 ,rotationDamping * Time.smoothDeltaTime);

        currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.smoothDeltaTime);

		// Damp the height
		currentHeight = Mathf.SmoothDamp (currentHeight, wantedHeight, ref temp ,heightDamping * Time.smoothDeltaTime);

        currentHeight = Mathf.Clamp(currentHeight,camFollowTarget.position.y + MinClampHeight, camFollowTarget.position.y+ MaxClampHeight);

		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);

		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = camFollowTarget.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		// Set the height of the camera
		transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);
		
		// Always look at the target
		transform.LookAt (camFollowTarget);
	}
}