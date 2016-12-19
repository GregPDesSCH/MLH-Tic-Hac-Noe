/* 
    MLH Tic-Hac-Noe
    Token

    This script gives the tokens some movement to make it look more 3D.S

    Programmed by Gregory Desrosiers 
    Candidate for Bachelor of Software Engineering (University of Waterloo)

    Programming Date: December 3, 2016
    Comments Added and Additional Fixes: December 18, 2016

    File Name: Token.cs
*/

using UnityEngine;
using System.Collections;

public class Token : MonoBehaviour {

    private Vector3 rotationVector;

	void Start ()
    {
        // Create the rotation vectors based on token shape (explained by layer name)
        if (gameObject.layer == LayerMask.NameToLayer("Cube Token"))
            rotationVector = new Vector3(RandomDegreeStep(false), RandomDegreeStep(false), RandomDegreeStep(false));
        else
            rotationVector = new Vector3(RandomDegreeStep(false), RandomDegreeStep(false), RandomDegreeStep(true));
    }

    void FixedUpdate() // Simply rotate the models on their own axes, as there's nothing else they need to do.
    {
        gameObject.transform.Rotate(rotationVector * Time.deltaTime);
    }

    // Generates and clamp a random degree step (degrees per second)
    // We pick a random value first, because just clamping alone will cause the float value to be the closest in the ranges.
    // For capsules, the Z rotation is faster as it's long, narrow and thin.
    float RandomDegreeStep(bool angleIsCylinderZ)
    {
        float rotationAngle = Random.Range(-90.0f, 90.0f);

        if (rotationAngle < 0.0f && !angleIsCylinderZ)
            return Mathf.Clamp(rotationAngle, -90.0f, -30.0f);
        else if (rotationAngle >= 0.0f && !angleIsCylinderZ)
            return Mathf.Clamp(rotationAngle, 30.0f, 90.0f);
        else if (rotationAngle < 0.0f && angleIsCylinderZ)
            return Mathf.Clamp(rotationAngle, 45.0f, 75.0f);
        else
            return Mathf.Clamp(rotationAngle, -75.0f, -45.0f);
    }
}
