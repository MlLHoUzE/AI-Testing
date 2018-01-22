using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUpdate : MonoBehaviour {

    [SerializeField]
    private float time;
    [SerializeField]
    private float timeBetweenChange;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float lowTime;
    [SerializeField]
    private float highTime;


    private bool bIsRotating;
    private Quaternion newRotation;
    private Quaternion lastRotation;

	// Use this for initialization
	void Start () {
        bIsRotating = false;
	}
	
	// Update is called once per frame
	void Update () {


        if (bIsRotating)
        {
            updateRotation();
            return;
        }

        time += Time.deltaTime;
        if (time >= timeBetweenChange)
        {
            bIsRotating = true;
            lastRotation = transform.rotation;
            newRotation = Quaternion.Euler(90f, Random.Range(30, 315), 0);
            StartCoroutine(updateRotation());
            //transform.Rotate(new Vector3(0, 90, 0), Space.World);
            time = 0.0f;

            timeBetweenChange = Random.Range(lowTime, highTime);
        }
	}
    

    private IEnumerator updateRotation()
    {
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, t);
            yield return null;
        }

        transform.rotation = newRotation;
        bIsRotating = false;
        yield return null;
    }
}
