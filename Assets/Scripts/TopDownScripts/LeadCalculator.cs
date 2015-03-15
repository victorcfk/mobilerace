using UnityEngine;
using System.Collections;

public class LeadCalculator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static Quaternion FirstOrderInterceptQuaternion
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity,
        Vector3 shooterUpVector
    )
    {

        return Quaternion.LookRotation(
            FirstOrderInterceptDirection(
            shooterPosition,
            shooterVelocity,
            shotSpeed,
            targetPosition,
            targetVelocity), shooterUpVector);
    }


    public static Quaternion FirstOrderInterceptQuaternion
    (
        GameObject shooter,
        float shotSpeed,
        GameObject target,
        Vector3 shooterUpVector
    )
    {

        return Quaternion.LookRotation(
            FirstOrderInterceptDirection(
            shooter.transform.position,
            shooter.rigidbody.velocity,
            shotSpeed,
            target.transform.position,
            target.rigidbody.velocity), shooterUpVector);
    }

    public static float FirstOrderInterceptAngle
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        return Vector3.Angle(Vector3.up,
            FirstOrderInterceptDirection(
            shooterPosition,
            shooterVelocity,
            shotSpeed,
            targetPosition,
            targetVelocity));
    }


    public static float FirstOrderInterceptAngle
    (
        GameObject shooter,
        float shotSpeed,
        GameObject target
    )
    {
        return Vector3.Angle(Vector3.up,
            FirstOrderInterceptDirection(
            shooter.transform.position,
            shooter.rigidbody.velocity,
            shotSpeed,
            target.transform.position,
            target.rigidbody.velocity));
    }

    public static Vector3 FirstOrderInterceptDirection
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        return FirstOrderInterceptPosition(
            shooterPosition,
            shooterVelocity,
            shotSpeed,
            targetPosition,
            targetVelocity) - shooterPosition;
    }


    public static Vector3 FirstOrderInterceptDirection
    (
        GameObject shooter,
        float shotSpeed,
        GameObject target
    )
    {
        return FirstOrderInterceptPosition(
            shooter.transform.position,
            shooter.rigidbody.velocity,
            shotSpeed,
            target.transform.position,
            target.rigidbody.velocity) - shooter.transform.position;
    }


    //first-order intercept using absolute target position
    public static Vector3 FirstOrderInterceptPosition
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t * (targetRelativeVelocity);
    }

    //first-order intercept using absolute target position
    public static Vector3 FirstOrderInterceptPosition
    (
        GameObject shooter,
        float shotSpeed,
        GameObject target
    )
    {
        Vector3 targetRelativePosition = target.transform.position - shooter.transform.position;
        Vector3 targetRelativeVelocity = target.rigidbody.velocity - shooter.rigidbody.velocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return target.transform.position + t * (targetRelativeVelocity);
    }

    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
}
