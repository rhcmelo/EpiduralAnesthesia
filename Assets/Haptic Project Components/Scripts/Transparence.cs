using UnityEngine;
using System.Collections;

public class Transparence : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision colisao)
    {
        Debug.Log("colisao tecido");
        // Tornando tecido semi-transparente
        this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

    }
}
