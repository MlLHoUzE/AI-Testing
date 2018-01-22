using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float movementSpeed = 10;
    private Vector3 force;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        force = new Vector3(0, 0, 0);

        if(Input.anyKey == true)
        {
            if(Input.GetKey(KeyCode.W))
            {
                //transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
                force += Vector3.forward * movementSpeed * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.D))
            {
               // transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
                force += Vector3.right * movementSpeed * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.S))
            {
               // transform.Translate(Vector3.forward * -movementSpeed * Time.deltaTime);
                force += Vector3.forward * -movementSpeed * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.A))
            {
               // transform.Translate(Vector3.right * -movementSpeed * Time.deltaTime);
                force += Vector3.right * -movementSpeed * Time.deltaTime;
            }
        }

        GetComponent<Rigidbody>().velocity += force;
	}
}
