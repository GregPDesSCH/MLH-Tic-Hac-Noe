/* 

*/

using UnityEngine;
using System.Collections;

public class Token : MonoBehaviour {

    private Vector3 rotationVector;


	// Use this for initialization
	void Start ()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Cube Token"))
            rotationVector = new Vector3(RandomDegreeStep(false), RandomDegreeStep(false), RandomDegreeStep(false));
        else
            rotationVector = new Vector3(RandomDegreeStep(false), RandomDegreeStep(false), RandomDegreeStep(true));
    }

	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        gameObject.transform.Rotate(rotationVector * Time.deltaTime);
    }

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
