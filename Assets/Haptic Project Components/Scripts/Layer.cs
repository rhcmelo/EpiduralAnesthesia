using UnityEngine;
using System.Collections;

public class Layer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision colisao)
    {
        this.GetComponent<Renderer>().material.color = new Color (1f,1f,1f,0.5f); 
    }
    void OnCollisionExit(Collision colisao)
    {
        this.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
    }
}
