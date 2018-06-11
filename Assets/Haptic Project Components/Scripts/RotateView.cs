using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RotateView : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float move = 2.0f;


        float y = transform.eulerAngles.y;

        Debug.Log("Angulos y: " + transform.eulerAngles.y.ToString());        

        if (Input.GetKey(KeyCode.RightArrow) && ((y <50) || (y > 320)))
        {
            transform.Rotate(0.0f, -move, 0.0f);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && ((y < 40) || (y>310)))
        {
            transform.Rotate(0.0f, move, 0.0f);
        }
    }
}
